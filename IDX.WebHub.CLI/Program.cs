//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//import namespace(s) required from 'IDX.MEF' library
using IDX.MEF.Typing;

// import namespace(s) from 'IDX.REST' library required
using IDX.WebHub.Typing.Configuration;

namespace IDX.WebHub.CLI
{
    class Program
    {

        #region the main method

        /// <summary>
        /// the main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //parse the command line arguments
            Dictionary<string, string> parsedArgs = ParseArgs(args);

            //create the host arguments instance
            HostArguments hostArgs = new HostArguments();

            if (parsedArgs.ContainsKey("settings"))
            {

                if (File.Exists(parsedArgs["settings"]))
                {

                    try
                    {

                        hostArgs.Read(parsedArgs["settings"]);

                        PromptMessage("Settings loaded from '" + parsedArgs["settings"] + "'", "DEBUG");

                    }
                    catch (Exception ex)
                    {

                        PromptMessage("Unable to load settings from file at '" + parsedArgs["settings"] + "': " + ex.Message, "WARNING");

                    }

                }
                else
                {

                    PromptMessage("Unable to find settings file at '" + parsedArgs["settings"] + "'", "WARNING");

                }

            }
            else
            {

                PromptMessage("No settings defined: using defaults", "DEBUG");

            }

            //override the host arguments/ settings (for dedicated use case(s))
            hostArgs = OverrideHostSettings(hostArgs, parsedArgs);

            //start-up the host
            Host host = new Host(hostArgs);
            host.MessageReceived += new EventHandler<HostMessageEventArgs>(HandleMessage); //add an event handler for the 'MessageReceived' event
            host.Start();

            //prevent console from closing
            Console.ReadLine();

        }

        #endregion

        #region (general) private method(s)

        /// <summary>
        /// method for parsing the (CLI) argument(s)
        /// </summary>
        private static Dictionary<string, string> ParseArgs(string[] args)
        {

            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (string arg in args)
            {

                //check for the 'settings' arguments
                if (arg.ToLower().StartsWith("-settings:"))
                {

                    output.Add("settings", arg.Substring(10));

                }

                //check for the (create) 'examplesettings' argument
                if (arg.ToLower().StartsWith("-examplesettings"))
                {

                    output.Add("examplesettings", true.ToString());

                }

            }

            return output;

        }

        /// <summary>
        /// event handler for receiving host messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void HandleMessage(object sender, HostMessageEventArgs args)
        {

            string message = args.Timestamp.ToString("yyyy-MM-dd HH.mm.ss") + " | " + ResizeString(args.Type.ToString(), 10) + " | " + args.Message;

            if (!(args.Details == null))
            {

                if (args.Details.GetType().Equals(typeof(Exception)))
                {

                    Exception ex = (Exception)args.Details;

                    message += Environment.NewLine + ex.StackTrace.ToString();

                }

            }

            Console.WriteLine(message);

        }

        /// <summary>
        /// method for prompting a message 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgType"></param>
        private static void PromptMessage(string message, string msgType = "INFO")
        {

            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + " | " + ResizeString(msgType, 10) + " | " + message);

        }

        /// <summary>
        /// method filling up a string with whitespaces to respect the presentation style
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string ResizeString(string value, int length)
        {

            string output = value;

            while (output.Length < length)
            {

                output += " ";

            }

            return output;

        }

        #endregion

        #region use case-related private method(s)

        /// <summary>
        /// method overriding the host arguments/ settings, fol
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static HostArguments OverrideHostSettings(HostArguments settings, Dictionary<string,string> args)
        {

            //create a set of default settings
            if(args.ContainsKey("examplesettings"))
            {

                //check for an output file path
                if(!args.ContainsKey("settings"))
                {

                    PromptMessage("No settings file path has been specified; the example settings will be applied to the current session only", "WARNING");

                }

                //create a new instance of settings
                HostArguments output = new HostArguments();

                //set the port
                output.Port = 11833;

                //add the extension settings
                List<ExtensionSettings> extensions = new List<ExtensionSettings>();
                extensions.Add(new ExtensionSettings() { Source = "%ROOT%\\..\\..\\..\\IDX.WebHub.APIDemo\\bin\\Debug\\*.dll" });

                output.Extensions = extensions.ToArray();

                //add the configuration(s)
                List<Configuration> configs = new List<Configuration>();

                configs.Add(new StaticContentConfiguration() { ID = "Demo Static Content", Path = "D:\\Path\\to\\my\\files\\", Route = "demo" });

                ServiceConfiguration serviceConfig = new ServiceConfiguration() { ID = "mySimpleService", Extension = "APIDemo_SimpleService" };
                serviceConfig.Parameters.Add("answer", "think twice!");
                configs.Add(serviceConfig);

                PropertiesConfiguration propsConfig1 = new PropertiesConfiguration() { ID = "democonfig" };
                propsConfig1.Parameters.Add("myKey", "myValue");
                configs.Add(propsConfig1);

                output.Configurations = configs.ToArray();

                //try to write the settings to file
                if (args.ContainsKey("settings"))
                {

                    if(!File.Exists(args["settings"])) //the output file does not already exist
                    {

                        try
                        {

                            output.Write(args["settings"]);

                            PromptMessage("The example settings have been written to '" + args["settings"] + "'", "DEBUG");

                        }
                        catch (Exception ex)
                        {

                            PromptMessage("Writing the settings file to '" + args["settings"] + "' failed: " + ex.Message + "; the esample settings will be applied to the current session only", "WARNING");

                        }

                    } else //there is already an output file existing
                    {

                        PromptMessage("There is already a settings file existing at '" + args["settings"] + "'; the example settings will be applied to the current session only", "WARNING");

                    }

                }

                //return the output
                return output;

            }

            //return the default output value
            return settings;

        }

        #endregion

    }

}
