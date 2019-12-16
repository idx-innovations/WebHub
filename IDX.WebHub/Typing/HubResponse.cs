//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDX.WebHub.Typing
{
    public class HubResponse
    {

        #region "private variable(s)"

        private object _content = null;
        private string _error = null;

        #endregion

        #region "public properties"

        public object Content
        {
            get
            {

                if (_content == null)
                {

                    return string.Empty;

                }

                return _content;

            }
            set
            {
                _content = value;
            }
        }

        public string Error
        {
            get
            {

                if (_error == null)
                {

                    return string.Empty;

                }

                return _error;

            }
            set
            {
                _error = value;
            }
        }

        public bool IsError
        {
            get
            {

                return !string.IsNullOrEmpty(_error);

            }
        }

        #endregion

        #region "public new instance method(s)"

        //default new instance method
        public HubResponse(object content)
        {

            //set the variable(s)
            _content = content;

        }

        //overloaded method
        public HubResponse(object content, string error)
        {

            //set the variable(s)
            _content = content;
            _error = error;

        }

        #endregion

    }
}
