using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PortableCSharpLib.Common
{
    //public class CIXmlSerializable : IXmlSerializable
    //{
    //    public System.Xml.Schema.XmlSchema GetSchema()
    //    {
    //        return null;
    //    }
    //    public void ReadXml(System.Xml.XmlReader reader)
    //    {
    //        ReadXml2(this, reader);
    //    }
    //    public void WriteXml(System.Xml.XmlWriter writer)
    //    {
    //        WriteXml2(this, writer);
    //    }

    //    static void WriteXml2(object obj, System.Xml.XmlWriter writer)
    //    {
    //        var type = obj.GetType();
    //        writer.WriteStartElement(type.Name);

    //        var properties = obj.GetType().GetWritableProperties();
    //        foreach (var prop in properties) {
    //            var value = prop.GetValue(obj, null);
    //            if (prop.PropertyType.IsNumericOrString()) {
    //                //all numeric or string property is written as attributes to current node
    //                if (prop.PropertyType.IsEnum)
    //                    writer.WriteElementString(prop.Name, ((int)value).ToString());
    //                else
    //                    writer.WriteElementString(prop.Name, value == null ? string.Empty : value.ToString());
    //            }
    //            else if (prop.PropertyType.IsDateTime()) {
    //                writer.WriteElementString(prop.Name, ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss"));
    //            }
    //            else if (prop.PropertyType.IsNullable()) {
    //                writer.WriteElementString(prop.Name, value == null ? string.Empty : value.ToString());
    //            }
    //            else if (prop.PropertyType.HasInterface("IXmlSerializable")) {
    //                var p = prop.GetValue(obj, null) as IXmlSerializable;
    //                writer.WriteStartElement(prop.Name);
    //                p.WriteXml(writer);
    //                writer.WriteEndElement();
    //            }
    //            else if (prop.PropertyType.HasInterface("IDictionary")) {
    //                var ifs = prop.PropertyType.GetInterfaces().ToList().FirstOrDefault(i => i..GenericTypeArguments.Length == 2);
    //                var kType = ifs.GenericTypeArguments[0];
    //                var vType = ifs.GenericTypeArguments[1];
    //                if (kType.IsNumericOrString() && (vType.IsNumericOrString() || vType.HasInterface("IXmlSerializable"))) {
    //                    dynamic p = prop.GetValue(obj);
    //                    writer.WriteStartElement(prop.Name);
    //                    if (p != null) {
    //                        writer.WriteAttributeString("Num", p.Count.ToString());
    //                        //writer.WriteAttributeString("Type", "Dictionary");
    //                        if (vType.IsNumericOrString()) {
    //                            var str = new StringBuilder();
    //                            foreach (var key in p.Keys) {
    //                                str.Append(string.Format("{0},{1};", key, p[key]));
    //                            }
    //                            str.Remove(str.Length - 1, 1); //remove last semi column
    //                            writer.WriteString(str.ToString());
    //                        }
    //                        else if (vType.HasInterface("IXmlSerializable")) {
    //                            foreach (var kv in p) {
    //                                writer.WriteStartElement("item");
    //                                writer.WriteAttributeString("key", kv.Key);
    //                                (kv.Value as IXmlSerializable).WriteXml(writer);
    //                            }
    //                        }
    //                    }
    //                    writer.WriteEndElement();
    //                }
    //            }
    //            else if (prop.PropertyType.HasInterface("ICollection")) {
    //                var ifs = prop.PropertyType.GetInterfaces().ToList().Find(i => i.GenericTypeArguments.Length == 1);
    //                var gType = ifs.GenericTypeArguments[0];
    //                if (gType.HasInterface("IXmlSerializable") || gType.IsNumericOrString()) {
    //                    dynamic p = prop.GetValue(obj);
    //                    writer.WriteStartElement(prop.Name);
    //                    if (p != null) {
    //                        writer.WriteAttributeString("Num", p.Count.ToString());
    //                        //writer.WriteAttributeString("Type", "ICollection");
    //                        if (gType.HasInterface("IXmlSerializable")) {
    //                            foreach (var item in p)
    //                                (item as IXmlSerializable).WriteXml(writer);
    //                        }
    //                        else if (gType.IsNumericOrString()) {
    //                            var str = new StringBuilder();
    //                            foreach (var item in p)
    //                                str.Append((item == null ? string.Empty : item.ToString()) + ",");
    //                            str.Remove(str.Length - 1, 1); //remove last comma
    //                            writer.WriteString(str.ToString());
    //                        }
    //                    }
    //                    writer.WriteEndElement();
    //                }
    //            }
    //        }
    //        writer.WriteEndElement();
    //    }
    //    static void ReadXml2(object obj, System.Xml.XmlReader reader)
    //    {
    //        var type = obj.GetType();
    //        var properties = type.GetWritableProperties();

    //        while (reader.Read()) {
    //            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == type.Name)
    //                break;

    //            var prop = properties.FirstOrDefault(p => p.Name == reader.Name);
    //            if (prop == null) continue;

    //            if (reader.IsEmptyElement) continue;

    //            if (prop.PropertyType.IsNumericOrString() || prop.PropertyType.IsDateTime() || prop.PropertyType.IsNullable()) {
    //                var value = reader.ReadString();
    //                if (value != null) {
    //                    if (prop.PropertyType.IsEnum)
    //                        prop.SetValue(obj, int.Parse(value));
    //                    else if (prop.PropertyType.IsNullable()) {
    //                        var gType = prop.PropertyType.GenericTypeArguments[0];
    //                        prop.SetValue(obj, value == "" ? null : Convert.ChangeType(value, gType));
    //                    }
    //                    else
    //                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
    //                }
    //            }
    //            else if (prop.PropertyType.HasInterface("IXmlSerializable")) {
    //                var p = Activator.CreateInstance(prop.PropertyType); ;   //given type must support zero constructor
    //                (p as IXmlSerializable).ReadXml(reader);
    //                prop.SetValue(obj, p, null);
    //                reader.Read();                                           //read EndElement
    //            }
    //            else {
    //                if (prop.PropertyType.HasInterface("IDictionary")) {
    //                    var ifs = prop.PropertyType.GetInterfaces().ToList().FirstOrDefault(i => i.GenericTypeArguments.Length == 2);
    //                    var kType = ifs.GenericTypeArguments[0];
    //                    var vType = ifs.GenericTypeArguments[1];
    //                    dynamic dic = Activator.CreateInstance(prop.PropertyType);
    //                    var num = int.Parse(reader.GetAttribute("Num"));
    //                    if (num > 0) {
    //                        if (vType.IsNumericOrString()) {
    //                            var items = reader.ReadString().Split(';').Select(x => x.Trim()).ToList();
    //                            foreach (var x in items) {
    //                                var kv = x.Split(',');
    //                                dynamic key = Convert.ChangeType(kv[0].Trim(), kType);
    //                                dynamic value = Convert.ChangeType(kv[1].Trim(), vType);
    //                                dic.Add(key, value);
    //                            }

    //                            //for (int i = 0; i < num; i++) {
    //                            //    reader.Read();
    //                            //    dynamic key = Convert.ChangeType(reader.GetAttribute("key"), kType);
    //                            //    dynamic value = Convert.ChangeType(reader.GetAttribute("value"), vType);
    //                            //    dic.Add(key, value);
    //                            //}
    //                            //reader.Read();
    //                            prop.SetValue(obj, dic);
    //                        }
    //                        else if (vType.HasInterface("IXmlSerializable")) {
    //                            for (int i = 0; i < num; i++) {
    //                                reader.Read();
    //                                dynamic key = Convert.ChangeType(reader.GetAttribute("key"), kType);
    //                                dynamic value = Activator.CreateInstance(vType);
    //                                (value as IXmlSerializable).ReadXml(reader);
    //                                dic.Add(key, value);
    //                                reader.Read();
    //                            }
    //                            prop.SetValue(obj, dic);
    //                            reader.Read();
    //                        }
    //                    }
    //                }
    //                else if (prop.PropertyType.HasInterface("ICollection")) {
    //                    var ifs = prop.PropertyType.GetInterfaces().ToList().Find(i => i.GenericTypeArguments.Length == 1);
    //                    var gtype = ifs.GenericTypeArguments[0];
    //                    var lst = Activator.CreateInstance((typeof(List<>).MakeGenericType(gtype))) as IList;
    //                    var num = int.Parse(reader.GetAttribute("Num"));
    //                    if (num > 0) {
    //                        if (gtype.HasInterface("IXmlSerializable")) {
    //                            for (int i = 0; i < num; i++) {
    //                                var item = Activator.CreateInstance(gtype);
    //                                (item as IXmlSerializable).ReadXml(reader);
    //                                lst.Add(item);
    //                            }
    //                            prop.SetValue(obj, lst);
    //                            reader.Read();                                          //read EndElement
    //                        }
    //                        else if (gtype.IsNumericOrString()) {
    //                            var text = reader.ReadString();
    //                            var item = text.Split(',').Select(x => x.Trim()).ToList();
    //                            foreach (var x in item)
    //                                lst.Add(Convert.ChangeType(x, gtype));
    //                            prop.SetValue(obj, lst);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
