using System.Collections.Generic;
using System.Reflection;

namespace PortableCSharpLib.Interface
{
    public interface IWriteFieldAndPropertyUsingRelection
    {
        List<FieldInfo> Fields { get; }
        List<PropertyInfo> Properties { get; }
        dynamic GetFieldPropertyValueByName(string propertyName);
        bool SetPropertyFieldValueByName(string paramName, dynamic value);
        //find all fields containing the the string
        List<string> FindFieldsContainStr(string fieldName);
        List<string> FindPropertiesContainStr(string fieldName);
    }
}
