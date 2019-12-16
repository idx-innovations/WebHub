//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'Nancy' library
using Nancy.Hosting.Self;
using Nancy.Bootstrapper;

//import internal namespace(s) required
using IDX.WebHub.Nancy;

namespace IDX.WebHub
{

    /// <summary>
    /// hosting main class
    /// </summary>
    /// <remarks>based on the article found at 'http://www.codeguru.com/columns/dotnet/creating-microservices-using-nancyfx.html'</remarks>
    public class Host
    {

        #region "public event(s)"

        public event EventHandler<HostMessageEventArgs> MessageReceived;

        #endregion

        #region "private variable(s)"

        private HostArguments _args = null;
        private NancyHost _host = null;

        #endregion

        #region "public properties"

        public HostArguments Arguments
        {
            get
            {
                return _args;
            }
        }

        #endregion

        #region "public new instance method(s)"

        public Host(HostArguments args = null)
        {

            //set the variable(s)
            if (args == null)
            {

                _args = new HostArguments();

            }
            else
            {

                _args = args;

            }

        }

        #endregion

        #region "public method(s)"

        /// <summary>
        /// method for starting the server host/ port listensing
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        public void Start()
        {

            //create a new ('Nancy') bootstrapper
            Host me = this;
            INancyBootstrapper bootstrapper = new RuntimeBootstrapper(ref me);

            //create a new host and start
            Uri listenTo = new Uri(_args.Address + ":" + _args.Port.ToString());

            _host = new NancyHost(bootstrapper, listenTo);
            _host.Start();

            //prompt a message
            AcceptMessage("Host started listening on '" + listenTo.ToString() + "'");

        }

        /// <summary>
        /// method for stopping the server host/ port listening
        /// </summary>
        public void Stop()
        {

            //stop the host
            if (_host != null)
            {

                //stop the host
                _host.Stop();
                _host = null;

                //prompt a message
                AcceptMessage("Host stopped listening");

            }
            else
            {

                //prompt a message
                AcceptMessage("No host stop required: No active host was listening", HostMessageEventArgs.Types.DEBUG);

            }

        }

        /// <summary>
        /// overloaded method accepting a message, raising the 'MessageReceived' event
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="details"></param>
        public void AcceptMessage(string message, HostMessageEventArgs.Types type = HostMessageEventArgs.Types.INFO, object details = null)
        {

            AcceptMessage(new HostMessageEventArgs() { Message = message, Type = type, Details = details });

        }

        #endregion

        #region "private method(s)"

        /// <summary>
        /// method accepting a message, raising the 'MessageReceived' event
        /// </summary>
        /// <param name="args"></param>
        private void AcceptMessage(HostMessageEventArgs args)
        {

            EventHandler<HostMessageEventArgs> handler = MessageReceived;

            if (handler != null)
            {

                handler(this, args);

            }

        }

        #endregion

    }

}
