//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'IDX.WebHub' library
using IDX.WebHub.API.Services;
using IDX.WebHub.Typing;
using IDX.WebHub.Typing.Configuration;

namespace IDX.WebHub.APIDemo.Services
{

    [ServiceMetadata(ID = "APIDemo_SimpleService")]
    public class SimpleService : Service
    {

        #region "private variable(s)"

        private HubContext _context = null;
        private ServiceConfiguration _config = null;

        #endregion

        #region "public new instance method(s)

        /// <summary>
        /// default constructor
        /// </summary>
        /// <remarks>required for MEF enumeration</remarks>
        public SimpleService()
        {
        }

        public SimpleService(ref HubContext context, ServiceConfiguration config)
        {

            //set the variable(s)
            _context = context;
            _config = config;

        }

        #endregion

        #region "method(s) implementing the 'IService' interface

        public override IService GetInstance(ref HubContext context, ServiceConfiguration config)
        {

            return new SimpleService(ref context, config);

        }

        #endregion

        #region "public method(s)"

        /// <summary>
        /// service-related method returning the answer
        /// </summary>
        /// <returns></returns>
        public string GetAnswer()
        {

            if(_config.Parameters.ContainsKey("answer"))
            {

                return _config.Parameters["answer"];

            }

            return "42";

        }

        #endregion

    }
}
