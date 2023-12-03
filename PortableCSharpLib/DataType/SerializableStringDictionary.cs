using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PortableCSharpLib.DataType
{
    public class SerializableStringDictionary : StringDictionary, IXmlSerializable
    {
        static SerializableStringDictionary() { PortableCSharpLib.General.CheckDateTime(); }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Clear();
            while (reader.Read() &&
                !(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == this.GetType().Name))
            {
                var name = reader["Name"];
                if (name == null)
                    throw new FormatException();

                var value = reader["Value"];
                this[name] = value;                   //add to dictionary
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in Keys)
            {
                writer.WriteStartElement("Pair");
                writer.WriteAttributeString("Name", (string)key);
                writer.WriteAttributeString("Value", this[(string)key]);
                writer.WriteEndElement();
            }
        }

        override public string ToString()
        {
            if (this.Count <= 0) return string.Empty;

            var strBuilder = new StringBuilder();
            foreach (string key in this.Keys)
            {
                var value = this[key];
                strBuilder.Append(string.Format("{0,30}: {1,10}\n", key, value));
            }
            return strBuilder.ToString();
        }

        public Dictionary<string,string> ToDictionary()
        {
            var dic = new Dictionary<string, string>();
            foreach (string k in this.Keys)
                dic.Add(k, this[k]);

            return dic;
        }
    }
}
