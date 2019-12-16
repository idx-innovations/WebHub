//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import internal namespace(s) required
using IDX.WebHub.Typing;
using IDX.WebHub.Typing.Configuration;

namespace IDX.WebHub.API.Services
{

    public abstract class Service : IService
    {

        #region method(s) implementing the 'IService' interface

        public abstract IService GetInstance(ref HubContext context, ServiceConfiguration config);

        #endregion

    }

}
