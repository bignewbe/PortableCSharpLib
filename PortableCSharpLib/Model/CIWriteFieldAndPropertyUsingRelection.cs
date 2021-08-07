using PortableCSharpLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PortableCSharpLib.Model
{
    [Serializable]                              //means the class is serializable
    public abstract class CIWriteFieldAndPropertyUsingRelection : IWriteFieldAndPropertyUsingRelection
    {
        static CIWriteFieldAndPropertyUsingRelection() { PortableCSharpLib.General.CheckDateTime(); }

        public List<FieldInfo> Fields { get { return this.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList(); } }
        public List<PropertyInfo> Properties { get { return this.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList(); } }

        //get property or field value by exact match of name
        public dynamic GetFieldPropertyValueByName(string propertyName)
        {
            var fields = Fields.Where(f => f.Name == propertyName).ToList();
            if (fields != null && fields.Count == 1) {
                if (fields.Count == 1)
                    return fields[0].GetValue(this);
            }
            else {
                var properties = Properties.Where(f => f.Name == propertyName).ToList();
                if (properties != null && properties.Count == 1)
                    return properties[0].GetValue(this);
            }

            return null;
        }

        //set property or field value by exact match of name
        public bool SetPropertyFieldValueByName(string paramName, dynamic value)
        {
            var fields = Fields.Where(f => f.Name == paramName).ToList();
            if (fields != null && fields.Count > 0) {
                if (fields.Count == 1) {
                    var f = fields[0];
                    switch (Type.GetTypeCode(f.FieldType)) {
                        case TypeCode.Int32:
                        case TypeCode.Double:
                        case TypeCode.Boolean:
                        case TypeCode.String:
                            f.SetValue(this, value);
                            break;
                        default:
                            throw new NotSupportedException("type not supported in SetPropertyValueByName");
                    }
                    return true;
                }
            }
            else {
                var properties = Properties.Where(f => f.Name == paramName).ToList();
                if (properties != null && properties.Count == 1) {
                    var f = properties[0];
                    switch (Type.GetTypeCode(f.PropertyType)) {
                        case TypeCode.Int32:
                        case TypeCode.Double:
                        case TypeCode.Boolean:
                        case TypeCode.String:
                            f.SetValue(this, value);
                            break;
                        default:
                            throw new NotSupportedException("type not supported in SetPropertyValueByName");
                    }
                    return true;
                }
            }
            return false;
        }

        public List<string> FindFieldOrPropertyContainStr(string fieldOrPropertyName)
        {
            var fields = this.FindFieldsContainStr(fieldOrPropertyName);
            if (fields == null)
                fields = this.FindPropertiesContainStr(fieldOrPropertyName);

            return fields;
        }

        //find all fields containing the the string
        public List<string> FindFieldsContainStr(string fieldName)
        {
            var fields = Fields.ToList(); //Where(f => !f.IsStatic)
            //var fields = this.GetType().GetFields().ToList();
            if (fields == null || fields.Count <= 0) return null;

            fields = fields.Where(f => f.Name.Contains(fieldName)).ToList();
            if (fields != null && fields.Count > 0)
                return fields.Select(f => f.Name).ToList();

            return null;
        }

        public List<string> FindPropertiesContainStr(string fieldName)
        {
            var properties = Properties.ToList();
            if (properties == null || properties.Count <= 0) return null;

            properties = properties.Where(f => f.Name.Contains(fieldName)).ToList();
            if (properties != null && properties.Count > 0)
                return properties.Select(p => p.Name).ToList();

            return null;
        }
    }
}
