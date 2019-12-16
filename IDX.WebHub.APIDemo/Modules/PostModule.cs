//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'IDX.WebHub' library
using IDX.WebHub.API.Modules;
using IDX.WebHub.Typing;

namespace IDX.WebHub.APIDemo.Modules
{

    [WebModuleMetadata(ID = "APIDemo_PostModule", Name = "APIDemo: Post Module")]
    public class PostModule : WebModule
    {

        #region "public new instance method(s)"

        /// <summary>
        /// defult new instance module
        /// </summary>
        public PostModule()
        {

            Post("/demo/post", args => //list all values submitted
            {

                //get the POST data
                Dictionary<string, string> postData = null;

                if (TryGetPostData<Dictionary<string, string>>(ref postData))
                {

                    //parse the POST data
                    Dictionary<string, string> returnData = new Dictionary<string, string>();

                    foreach (KeyValuePair<string, string> pair in postData)
                    {

                        returnData.Add(pair.Key, HtmlDecode(pair.Value)); //add the items, parsing HTML masks

                    }

                    //return the output value
                    return GetJSONResponse(returnData);

                }

                //return the default output value
                return GetJSONResponse("Unable to get POST data");

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

            output.Add("/demo/post", new HubRouteMetadata() { Name = "A simple post data mirror", Description = "Returning a list of any key-value-pairs posted", ResponseType = typeof(HubResponse)});

            return output;

        }

        #endregion

    }
}
