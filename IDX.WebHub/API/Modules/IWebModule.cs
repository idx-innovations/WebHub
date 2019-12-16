//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import internal namespace(s) required
using IDX.WebHub.Typing;

namespace IDX.WebHub.API.Modules
{
    public interface IWebModule
    {

        Dictionary<string, HubRouteMetadata> GetRoutesMetadata();

    }

}
