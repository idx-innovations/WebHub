//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import internal namespae(s) required
using IDX.WebHub.Typing;
using IDX.WebHub.Typing.Configuration;

namespace IDX.WebHub.API.Services
{
    public interface IService
    {

        /// <summary>
        /// method returning a new instance of the implementing class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        IService GetInstance(ref HubContext context, ServiceConfiguration config);

    }
}
