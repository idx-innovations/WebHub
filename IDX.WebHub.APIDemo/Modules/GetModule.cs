//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'IDX.WebHub' library
using IDX.WebHub.API.Modules;

//import internal namespace(s) required
using IDX.WebHub.APIDemo.Services;
using IDX.WebHub.Typing;

namespace IDX.WebHub.APIDemo.Modules
{

    [WebModuleMetadata(ID = "APIDemo_GetModule", Name = "APIDemo: Get Module")]
    public class GetModule : WebModule
    {

        #region "public new instance method(s)"

        public GetModule()
        {

            Get("/demo/{message?}", args => //the 'message' argument is optional
            {

                string msg = "Hello World!";

                if (args.message != null)
                {

                    msg = args.message;

                }

                return msg;

            });

            Get("/demo/answer", args =>
            {

                SimpleService service = null;

                if(HubContext.TryGetService<SimpleService>("mySimpleService",ref service)) { //the service was found

                    return service.GetAnswer();

                }
                else
                {

                    return GetJSONResponse(null, "Failed to get 'mySimpleService'");

                }

            });

        }

        #endregion

        #region public method(s), inherited from base-class instance

        /// <summary>
        /// method returning the routing metadata
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, HubRouteMetadata> GetRoutesMetadata()
        {

            Dictionary<string, HubRouteMetadata> output = new Dictionary<string, HubRouteMetadata>();

            output.Add("/demo/{message?}", new HubRouteMetadata() { Name = "A simple message returning module", Description = "Returning the message encoded by URL string. If no answer is given, 'Hello World!' will be returned.", ResponseType = typeof(string) });
            output.Add("/demo/answer", new HubRouteMetadata() { Name = "A simple answering module", Description = "...based on the 'SimpleService' implementation. If no answer is defined, the 'Answer to the Ultimate Question of Life, the Universe, and Everything' is returned", ResponseType = typeof(HubResponse) });

            return output;

        }

        #endregion

    }
}
