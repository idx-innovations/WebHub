//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using System.Reflection;

//import namespace(s) required from 'IDX.MEF' library
using IDX.MEF.Typing;

//import internal namespae(s) required
using IDX.WebHub.Typing.Configuration;

namespace IDX.WebHub
{
    [Serializable]
    public class HostArguments
    {

        #region "private variable(s)"

        private string _address = "http://localhost";
        private int _port = 11833;

        private ExtensionSettings[] _extensions = { };
        private Configuration[] _config = { };

        #endregion

        #region public properties

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        public ExtensionSettings[] Extensions
        {
            get
            {
                return _extensions;
            }
            set
            {
                _extensions = value;
            }
        }

        public Configuration[] Configurations
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        #endregion

        #region public new instace method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <remarks>required for XML serialization</remarks>
        public HostArguments()
        {
        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method writing the data to (XML-based) disk file
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {

            // check for the output directory (and create if required)
            string dirPath = path.Substring(path.LastIndexOf(@"\") + 1);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            // serialize the data to XML file
            XmlSerializer serializer = new XmlSerializer(typeof(HostArguments));
            StreamWriter writer = new StreamWriter(path);

            serializer.Serialize(writer, this);

            writer.Close();

        }

        /// <summary>
        /// method reading the data from (XML-based) disk file
        /// </summary>
        /// <param name="path"></param>
        public void Read(string path)
        {

            // read the file
            if (File.Exists(path))
            {

                // create an intermediate class instance
                HostArguments tmp;

                // de-serialize the data
                XmlSerializer serializer = new XmlSerializer(typeof(HostArguments));
                FileStream reader = new FileStream(path, FileMode.Open);

                tmp = (HostArguments)serializer.Deserialize(reader);

                // set current class' properties
                this.Address = tmp.Address;
                this.Port = tmp.Port;
                this.Extensions = tmp.Extensions;
                this.Configurations = tmp.Configurations;

                reader.Close();

            }

        }

        /// <summary>
        /// method returning the configuration item(s) for a dedicated type
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <returns></returns>
        public t[] GetConfiguration<t>()
        {

            //create the output value
            List<t> output = new List<t>();

            foreach(Configuration config in this.Configurations)
            {

                if (config.GetType().Equals(typeof(t)))
                {

                    output.Add((t)Convert.ChangeType(config, typeof(t)));

                }

            }

            //return the default output
            return output.ToArray();

        }

        /// <summary>
        /// overloaded method returning a dedicated configuration item
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public t GetConfiguration<t>(string id)
        {

            //check for the ID
            foreach(t item in GetConfiguration<t>()) //loop for each configuration (instance) found
            {

                foreach(PropertyInfo info in typeof(t).GetProperties()) //loop for each property
                {

                    if(info.Name == "ID") //check for an 'ID'
                    {

                        if(info.GetValue(item).ToString().ToLower() == id.ToLower()) //validate the 'ID' value
                        {

                            //return the instance
                            return item;

                        }

                    }

                }

            }

            //return the default output value
            return default(t);

        }

        #endregion

    }

}
