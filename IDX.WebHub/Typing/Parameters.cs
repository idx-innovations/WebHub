//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IDX.WebHub.Typing
{
    [Serializable]
    [XmlRoot("Parameters")]
    public class Parameters : Dictionary<string, string>, IXmlSerializable
    {

        #region public interface method(s)

        //method enabling XML reading
        public void ReadXml(XmlReader reader)
        {

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if ((wasEmpty))
                return;

            while ((reader.NodeType != XmlNodeType.EndElement))
            {

                reader.ReadStartElement("Parameter");

                    //read the key
                    reader.ReadStartElement("Name");
                        string key = reader.ReadContentAsString();
                    reader.ReadEndElement();

                    //read the value
                    reader.ReadStartElement("Value");
                        string value = reader.ReadContentAsString();
                    reader.ReadEndElement();

                    //add the dictionary item
                    this.Add(key, value);

                reader.ReadEndElement();

            }

        }

        //method enabling XML writing
        public void WriteXml(XmlWriter writer)
        {

            // write the output
            foreach (string key in this.Keys)
            {

                writer.WriteStartElement("Parameter");

                    // write the key
                    writer.WriteStartElement("Name");
                        writer.WriteString(key);
                    writer.WriteEndElement();

                    // write the value
                    writer.WriteStartElement("Value");
                        writer.WriteString(this[key]);
                    writer.WriteEndElement();

                writer.WriteEndElement();

            }

        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <remarks>required to make dictionary case-insensitive</remarks>
        public Parameters() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method checking the dictionary for missing arguments (keys)
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public string[] GetMissingArguments(string[] names)
        {

            List<string> missing = new List<string>();

            foreach (string name in names)
            {

                if (!(this.ContainsKey(name)))
                {

                    missing.Add(name);

                }

            }

            return missing.ToArray();

        }

        /// <summary>
        /// method returning the value for a named item (incl. type casting)
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>based on the article found at 'https://stackoverflow.com/questions/972636/casting-a-variable-using-a-type-variable'</remarks>
        public t Get<t>(string name)
        {

            if (this.ContainsKey(name))
            {

                try
                {

                    return (t)Convert.ChangeType(this[name], typeof(t));

                }
                catch
                {

                    if (this[name.ToLower()].GetType().Equals(typeof(string)) && typeof(t).Equals(typeof(char))) //check for a string > char conversion
                    {

                        return (t)Convert.ChangeType(this[name.ToLower()].ToString()[0], typeof(t));

                    }

                }

            }

            //return the default output
            return default(t);

        }

        #endregion

    }

}
