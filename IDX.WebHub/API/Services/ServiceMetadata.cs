//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'IDX.MEF' library
using IDX.MEF.API;

namespace IDX.WebHub.API.Services
{
    public class ServiceMetadata : MetadataBase
    {

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        public ServiceMetadata() : base(typeof(IService))
        {
        }

        #endregion

    }
}
