//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'Nancy' framework
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Routing;

namespace IDX.WebHub.Typing
{
    public class HubRoute
    {

        #region "private variable(s)"

        private RouteDescription _description = null;
        private HubRouteMetadata _metadata = null;

        #endregion

        #region "public properties"

        public string Path
        {
            get
            {
                return _description.Path;
            }
        }

        public string Method
        {
            get
            {
                return _description.Method;
            }
        }

        public HubRouteMetadata Metadata
        {
            get
            {
                return _metadata;
            }
        }

        #endregion

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="path"></param>
        /// <param name="metadata"></param>
        public HubRoute(RouteDescription description, HubRouteMetadata metadata = null)
        {

            //set the variable(s)
            _description = description;
            _metadata = metadata;

        }

        #endregion

    }
}
