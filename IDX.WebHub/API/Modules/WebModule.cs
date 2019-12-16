//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

//import namespace(s) from 'Nancy' library required
using Nancy;
using Nancy.Extensions;
using Nancy.Helpers;
using Nancy.Bootstrapper;

//import internal namespace(s) required
using IDX.WebHub.Typing;

namespace IDX.WebHub.API.Modules
{
    public abstract class WebModule : NancyModule, IWebModule
    {

        #region "protected properties"

        protected HubContext HubContext
        {
            get
            {
                return Hub.Context;
            }
        }

        #endregion

        /// <summary>
        /// method enabling CORS
        /// </summary>
        /// <remarks>to be called at the very beginning of the constructor method</remarks>
        protected void EnableCORS()
        {

            this.After.AddItemToEndOfPipeline(ctx =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });

        }

        #region "method(s) implementing 'IWebModule'"

        /// <summary>
        /// method returning metadata for the routes defined by this module
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string,HubRouteMetadata> GetRoutesMetadata()
        {

            return new Dictionary<string, HubRouteMetadata>();

        }

        #endregion

        #region "protected request-related method(s)"

        /// <summary>
        /// method trying to get the post data
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <returns></returns>
        protected bool TryGetPostData<t>(ref t value)
        {

            try
            {

                //get the body string
                string data = this.Request.Body.AsString();

                //return a 'string' type value
                if (typeof(string).Equals(typeof(t)))
                {

                    value = (t)Convert.ChangeType(data, typeof(t));
                    return true;

                }

                //try to create a dictionary
                if (typeof(Dictionary<string, string>).Equals(typeof(t)))
                {

                    //try to parse a 'x-www-form-urlencoded' body
                    Regex rx1 = new Regex("([\\d\\D]+=[\\d\\D]+)&?");

                    foreach (Match myMatch in rx1.Matches(data))
                    {
                        if (myMatch.Success)
                        {

                            Dictionary<string, string> output = new Dictionary<string, string>();

                            foreach (string statement in data.Split('&'))
                            {


                                string[] tmp = statement.Split('=');


                                switch (tmp.Length)
                                {

                                    case 1:
                                        output.Add(tmp[0].ToLower(), String.Empty);
                                        break;

                                    case 2:
                                        output.Add(tmp[0].ToLower(), tmp[1]);
                                        break;

                                }

                            }

                            value = (t)Convert.ChangeType(output, typeof(t));
                            return true;

                        }

                    }

                }

                //try to return "any" type
                value = (t)Convert.ChangeType(data, typeof(t));
                return true;

            }
            catch
            {

                return false;

            }

        }

        /// <summary>
        /// method decoding a string from HTML masks
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string HtmlDecode(string text)
        {

            string output = text;

            //output = output.Replace("%5C", "\\");
            output = HttpUtility.UrlDecode(output);

            return output;

        }

        #endregion

        #region "protected response-related method(s)"

        /// <summary>
        /// method returning a valid file response
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected Response GetFileResponse(string path, string dataType = "text/html")
        {

            return Response.AsFile(path, dataType);

        }

        /// <summary>
        /// method returning a valid JSON response
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected Response GetJSONResponse(object data, string error = null)
        {

            HubResponse tmp = new HubResponse(data, error);

            return Response.AsJson(tmp);

        }

        #endregion

    }

}
