//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

//import namespace(s) required from 'IDX.MEF' library
using IDX.MEF;
using IDX.MEF.Typing;

//import namespace(s) required from 'Nancy' framework
using Nancy;
using Nancy.Routing;

//import internal namespace(s) required
using IDX.WebHub.API.Modules;
using IDX.WebHub.API.Services;
using IDX.WebHub.Typing.Configuration;

namespace IDX.WebHub.Typing
{
    public class HubContext
    {

        #region "private variable(s)"

        private Host _host = null;

        private ExtendedExtensionsManager<IWebModule, IWebModuleMetadata> _modules = null;
        private INancyModule[] _moduleInstances = { };

        private ExtendedExtensionsManager<IService, IServiceMetadata> _services = null;
        private Dictionary<string, IService> _serviceInstances = null;

        #endregion

        #region "public properties"

        public Lazy<IWebModule, IWebModuleMetadata>[] ModuleExtensions
        {
            get
            {
                return _modules.Extensions;
            }
        }

        public Lazy<IService, IServiceMetadata>[] ServiceExtensions
        {
            get
            {
                return _services.Extensions;
            }
        }

        public INancyModule[] Modules
        {
            get
            {
                return _moduleInstances;
            }
            set
            {
                _moduleInstances = value;
            }
        }

        public HostArguments HostArguments
        {
            get
            {
                return _host.Arguments;
            }
        }

        public string RootPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        #endregion

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="host"></param>
        public HubContext(ref Host host)
        {

            //set the variable(s)
            _host = host;

            //initialize the extensions manager(s)
            Dictionary<String, String> masks = new Dictionary<string, string>();
            masks.Add("%Root%", this.RootPath);

            _modules = new ExtendedExtensionsManager<IWebModule, IWebModuleMetadata>();
            _services = new ExtendedExtensionsManager<IService, IServiceMetadata>();

            List<ExtensionSettings> extensionSettings = new List<ExtensionSettings>(host.Arguments.Extensions);
            extensionSettings.Add(new ExtensionSettings() { Source = System.Reflection.Assembly.GetExecutingAssembly().Location }); //add the current assembly to search path

            _modules.Update(extensionSettings.ToArray(), masks);
            _services.Update(extensionSettings.ToArray(), masks);

            //load the service instance(s)
            UpdateServices();

        }

        #endregion

        #region "general public method(s)"

        /// <summary>
        /// method expanding environment variables and masks from path string(s)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="masks"></param>
        /// <returns></returns>
        public string ParsePath(string path, Dictionary<string, string> masks = null)
        {

            //parse the arguments
            if(masks == null)
            {

                masks = new Dictionary<string, string>();

            }

            masks.Add("%Root%", this.RootPath);

            //replace the masks
            foreach(KeyValuePair<string, string> mask in masks)
            {

                string toReplace = mask.Key;

                if (!toReplace.StartsWith("%"))
                {

                    toReplace = "%" + toReplace;

                }

                if (!toReplace.EndsWith("%"))
                {

                    toReplace += "%";

                }

                path = Regex.Replace(path, toReplace, mask.Value, RegexOptions.IgnoreCase);

            }

            //return the output
            return Environment.ExpandEnvironmentVariables(path);

        }

        #endregion

        #region "public module-related method(s)"

        /// <summary>
        /// method returning a list of all routes (incl. metadata) available
        /// </summary>
        /// <returns></returns>
        public HubRoute[] GetRoutes()
        {

            List<HubRoute> output = new List<HubRoute>();

            foreach (INancyModule module in _moduleInstances)
            {

                IWebModule instance = (IWebModule)Activator.CreateInstance(module.GetType());
                Dictionary<string, HubRouteMetadata> meta = instance.GetRoutesMetadata();

                foreach (Route route in module.Routes)
                {

                    HubRouteMetadata metadata = null;

                    if (meta.ContainsKey(route.Description.Path))
                    {

                        metadata = meta[route.Description.Path];

                    }

                    output.Add(new HubRoute(route.Description,metadata));

                }
            }

            return output.ToArray();

        }

        #endregion

        #region "public service-related method(s)"

        /// <summary>
        /// overloaded method trying to get a typed service instance by ID
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="ID"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool TryGetService<t>(string ID, ref t service)
        {

            IService result = null;

            if(TryGetService(ID,ref result))
            {

                if(result.GetType().Equals(typeof(t)))
                {

                    service = (t)Convert.ChangeType(result, typeof(t));

                    return true;

                }

            }

            //return the default output value
            return false;

        }

        /// <summary>
        /// method trying to get a service instance by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool TryGetService(string ID,ref IService service)
        {

            //return the service instance (if found)
            if(_serviceInstances.ContainsKey(ID.ToLower()))
            {

                service = _serviceInstances[ID.ToLower()];
                return true;

            }

            //return the default output value
            return false;

        }

        #endregion

        #region "public message-related method(s)"

        /// <summary>
        /// send an information message to the host instance
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isDebugging"></param>
        /// <param name="details"></param>
        public void Inform(string message, bool isDebugging = false, object details = null)
        {

            if (isDebugging)
            {

                _host.AcceptMessage(message, HostMessageEventArgs.Types.DEBUG, details);

            }
            else
            {

                _host.AcceptMessage(message, HostMessageEventArgs.Types.INFO, details);

            }


        }

        /// <summary>
        /// send a warning message to the host instance
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isException"></param>
        /// <param name="details"></param>
        public void Warn(string message, bool isException = false, object details = null)
        {

            if (isException)
            {

                _host.AcceptMessage(message, HostMessageEventArgs.Types.EXCEPTION,details);

            }
            else
            {

                _host.AcceptMessage(message, HostMessageEventArgs.Types.WARNING,details);

            }


        }

        #endregion

        #region "private method(s)"

        /// <summary>
        /// method trying to instanciate the services configured
        /// </summary>
        private void UpdateServices()
        {

            //clear the instances list
            _serviceInstances = new Dictionary<string, IService>();

            //index the service extensions found (by ID)
            Dictionary<string, Lazy<IService, IServiceMetadata>> extensions = new Dictionary<string, Lazy<IService, IServiceMetadata>>();

            foreach(Lazy<IService,IServiceMetadata> service in ServiceExtensions)
            {

                extensions.Add(service.Metadata.ID.ToLower(), service);

            }

            //try to get service instances
            foreach(ServiceConfiguration config in _host.Arguments.GetConfiguration<ServiceConfiguration>())
            {

                try
                {

                    //check for the extension
                    if(!(extensions.ContainsKey(config.Extension.ToLower())))
                    {

                        Warn("Failed to initialize service '" + config.ID + "' with extension '" + config.Extension + "': Unable to find related extension",false);

                    }

                    //add a new class instance
                    HubContext me = this;
                    _serviceInstances.Add(config.ID.ToLower(), extensions[config.Extension.ToLower()].Value.GetInstance(ref me, config));

                    Inform("Service '" + config.ID + "' ('" + config.Extension + "') added",true);

                }
                catch (Exception ex)
                {

                    Warn("Failed to initialize service '" + config.ID + "' with extension '" + config.Extension + "': " + ex.Message, true, ex);
    
                }

            }
            
        }

        #endregion

    }

}
