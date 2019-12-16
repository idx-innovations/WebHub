//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDX.WebHub.Typing
{
    public class HubRouteMetadata
    {

        #region "public variable(s)"

        public string Name { get; set; }
        public String Description { get; set; }

        public Dictionary<string,Type> RequestArguments { get; set; }
        public Type ResponseType { get; set; }

        #endregion

    }

}
