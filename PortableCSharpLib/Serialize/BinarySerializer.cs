using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.Serialize
{
    //public enum SerializeTypes : byte
    //{
    //    Invalid,
    //    IsHyperSerialize,
    //    IsCollection,
    //    IsDictionary,
    //    IsList,
    //    IsDataTable,
    //    IsTuple,
    //    IsEnumerable,
    //    IsValueType,
    //    IsSupported,
    //    IsDateTime,
    //    IsString,
    //    IsKeyValue,
    //    IsRealValueType,
    //    IsArray,
    //    IsNullable,
    //    IsClass,
    //    IsOther,
    //    IsGeneric,
    //}

    ////[AttributeUsage(AttributeTargets.Class)]
    ////public class HiPerComparable : Attribute
    ////{
    ////}

    ////[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    ////public class HiPerCompareMember : Attribute
    ////{
    ////    public int ID { get; set; }
    ////    public HiPerCompareMember(int id) { ID = id; }
    ////}

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Enum)]
    //public class IgnoreMember : Attribute
    //{
    //}

    //[AttributeUsage(AttributeTargets.Assembly)]
    //public class HiPerfTraceAssembly : Attribute
    //{
    //}

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Enum)]
    //public class HiPerfSerializable : Attribute
    //{
    //}

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Enum)]
    //public class HiPerfMember : Attribute
    //{
    //    public int ID { get; set; }
    //    public HiPerfMember(int id) { ID = id; }
    //}

    //public class BinarySerializer
    //{
    //    static private object _lockWriteFile = new object();
    //    #region facility
    //    static private bool IsHiPerfSerializable(Type type)
    //    {
    //        return (type.GetCustomAttributes(true).Any(att => (Attribute)att is HiPerfSerializable));
    //    }
    //    static private bool IsRealValueType(Type type)
    //    {
    //        return (type.IsValueType && !type.IsGenericType && type != typeof(DateTime) && type != typeof(Decimal));
    //    }
    //    static private bool IsKeyValuePair(Type type)
    //    {
    //        if (!type.IsGenericType) return false;
    //        return (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));
    //    }
    //    static private bool IsTypeNullable(Type type)
    //    {
    //        return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
    //    }
    //    static private bool IsTypeSuppoted(Type type)
    //    {
    //        if (type == typeof(string))
    //            return false;

    //        if (type.IsArray ||
    //            type.GetCustomAttributes(true).Any(att => (Attribute)att is HiPerfSerializable) ||
    //            type == typeof(DataTable))
    //            return true;

    //        if (!type.IsGenericType)
    //            return false;

    //        var ifs = type.GetInterfaces();
    //        //type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
    //        //type.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
    //        //type.GetGenericTypeDefinition() == typeof(IList<>) ||
    //        //type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
    //        if (ifs.Contains(typeof(IList)) ||
    //             ifs.Contains(typeof(IDictionary)) ||
    //             ifs.Contains(typeof(IEnumerable)))
    //            return true;

    //        return false;
    //    }
    //    /// <summary>
    //    /// Categorize type into enum type that needs to be handled properly
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    static public SerializeTypes GetSerializaionType(Type type)
    //    {
    //        var typeCode = Type.GetTypeCode(type);
    //        switch (typeCode) {
    //            case TypeCode.String:
    //                return SerializeTypes.IsString;
    //            case TypeCode.DateTime:
    //                return SerializeTypes.IsDateTime;
    //            default:
    //                break;
    //        }

    //        if (IsKeyValuePair(type))
    //            return SerializeTypes.IsKeyValue;

    //        if (type.IsValueType)
    //            return SerializeTypes.IsValueType;

    //        if (type.IsArray)
    //            return SerializeTypes.IsArray;

    //        if (type == typeof(DataTable))
    //            return SerializeTypes.IsDataTable;

    //        if (!type.IsGenericType)
    //            //return EnumSerializeType.Invalid;
    //            return SerializeTypes.IsGeneric;

    //        var ifs = type.GetInterfaces();
    //        if (ifs.Contains(typeof(IDictionary)))
    //            return SerializeTypes.IsDictionary;

    //        if (ifs.Contains(typeof(IList)))
    //            return SerializeTypes.IsList;

    //        if (ifs.Contains(typeof(ICollection)))
    //            return SerializeTypes.IsCollection;

    //        if (ifs.Contains(typeof(IEnumerable)))
    //            return SerializeTypes.IsEnumerable;

    //        if (type.FullName.Contains("System.Tuple"))
    //            return SerializeTypes.IsTuple;

    //        return SerializeTypes.Invalid;
    //    }
    //    #endregion

    //    #region Serialize/Deserialize

    //    static private void CreateMapForKeyValue(BinaryWriter writer, Type type, dynamic value)
    //    {
    //        /////////////////////////////////////////////////////////////////////////////
    //        var gTypes = type.GetGenericArguments();
    //        var kType = gTypes[0];
    //        var vType = gTypes[1];

    //        CreateMapAllTypes(writer, kType, value.Key);
    //        CreateMapAllTypes(writer, vType, value.Value);
    //    }
    //    static private void WriteKeyValueWithMap(BinaryWriter writer, Type type, dynamic value, BinaryReader rmap)
    //    {
    //        var gTypes = type.GetGenericArguments();
    //        var kType = gTypes[0];
    //        var vType = gTypes[1];

    //        WriteAllTypesWithMap(writer, kType, value.Key, rmap);
    //        WriteAllTypesWithMap(writer, vType, value.Value, rmap);
    //    }
    //    static private void WriteKeyValue(BinaryWriter writer, Type type, dynamic value)
    //    {
    //        /////////////////////////////////////////////////////////////////////////////
    //        var gTypes = type.GetGenericArguments();
    //        var kType = gTypes[0];
    //        var vType = gTypes[1];

    //        WriteAllTypes(writer, kType, value.Key);
    //        WriteAllTypes(writer, vType, value.Value);
    //    }
    //    static private dynamic ReadKeyValue(BinaryReader reader, Type type)
    //    {
    //        var gTypes = type.GetGenericArguments();
    //        var kType = gTypes[0];
    //        var vType = gTypes[1];

    //        var key = ReadAllTypes(reader, kType);
    //        var value = ReadAllTypes(reader, vType);

    //        var cstr = type.GetConstructor(new[] { kType, vType });                   //get constructor
    //        var pair = cstr.Invoke(new object[] { key, value });                      //create object
    //        return pair;
    //    }

    //    static private void CreateMapAllTypes(BinaryWriter writer, Type type, dynamic value)
    //    {
    //        //we need to recorde the number of bytes for this level. in case some child elements return prematurely (e.g. when List is null of Table has no row)
    //        //we can still seek the stream position to next sibing. 
    //        var position = writer.BaseStream.Position;
    //        long numbytes = 0;
    //        writer.Write(numbytes);  //write 0 since we do not know the length yet

    //        var typeCode = Type.GetTypeCode(type);
    //        switch (typeCode) {
    //            case TypeCode.Decimal:
    //            case TypeCode.Double:
    //            case TypeCode.Single:
    //            case TypeCode.Int64:
    //            case TypeCode.UInt64:
    //            case TypeCode.Int32:
    //            case TypeCode.UInt32:
    //            case TypeCode.Int16:
    //            case TypeCode.UInt16:
    //            case TypeCode.SByte:
    //            case TypeCode.Byte:
    //            case TypeCode.Boolean:
    //            case TypeCode.Char:
    //            case TypeCode.String:
    //            case TypeCode.DateTime:
    //                break;
    //            default:
    //                var etype = GetSerializaionType(type);
    //                Action<BinaryWriter, Type, dynamic> writeFunc;
    //                switch (etype) {
    //                    case SerializeTypes.IsDataTable:
    //                        if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                        writeFunc = CreateMapDataTable2;
    //                        break;
    //                    case SerializeTypes.IsArray:
    //                        if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                        writeFunc = CreateMapArray;
    //                        break;
    //                    case SerializeTypes.IsList:
    //                        if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                        writeFunc = CreateMapList;
    //                        break;
    //                    case SerializeTypes.IsDictionary:
    //                        if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                        writeFunc = CreateMapDictionary;
    //                        break;
    //                    case SerializeTypes.IsEnumerable:
    //                        if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                        writeFunc = CreateMapEnumerable;
    //                        break;
    //                    case SerializeTypes.IsKeyValue:
    //                        writeFunc = CreateMapForKeyValue;
    //                        break;
    //                    case SerializeTypes.IsTuple:
    //                        if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                        writeFunc = CreateMapTuple;
    //                        break;
    //                    default:
    //                        if (IsHiPerfSerializable(type)) {
    //                            if (value == null) throw new InvalidOperationException(string.Format("value is null and we cannot create class mapStream in this case"));
    //                            etype = SerializeTypes.IsHyperSerialize;
    //                            writeFunc = CreateMapClass;
    //                        }
    //                        else {
    //                            etype = SerializeTypes.Invalid;
    //                            var str = string.Format("Type {0} is not supported in CreateMapAllTypes", typeCode);
    //                            throw new InvalidOperationException(str);
    //                        }
    //                        break;
    //                }

    //                writer.Write((byte)etype);
    //                writeFunc(writer, type, value);
    //                break;
    //        }
    //        numbytes = writer.BaseStream.Position - position;  //total number of bytes for this level
    //        //write the number of bytes to the reserved position, i.e, the first 16 bytes (long)
    //        writer.BaseStream.Position = position;
    //        writer.Write(numbytes);
    //        //after writting the number of bytes, set cursor back to the end.
    //        writer.BaseStream.Seek(0, SeekOrigin.End);
    //    }
    //    static private void WriteAllTypesWithMap(BinaryWriter writer, Type type, dynamic value, BinaryReader rmap)
    //    {
    //        var typeCode = Type.GetTypeCode(type);
    //        var position = rmap.BaseStream.Position;  //record the starting position for this node
    //        var numbytes = rmap.ReadInt64();          //get the number of bytes for this node    
    //        switch (typeCode) {
    //            case TypeCode.Decimal:
    //            case TypeCode.Double:
    //            case TypeCode.Single:
    //            case TypeCode.Int64:
    //            case TypeCode.UInt64:
    //            case TypeCode.Int32:
    //            case TypeCode.UInt32:
    //            case TypeCode.Int16:
    //            case TypeCode.UInt16:
    //            case TypeCode.SByte:
    //            case TypeCode.Byte:
    //            case TypeCode.Boolean:
    //            case TypeCode.Char:
    //                writer.Write(value);
    //                break;
    //            case TypeCode.String:
    //                writer.Write(value == null);
    //                if (value == null) return;
    //                writer.Write(value);
    //                break;
    //            case TypeCode.DateTime:
    //                var time = ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
    //                writer.Write(time);
    //                break;
    //            default:
    //                var etype = (SerializeTypes)rmap.ReadByte();
    //                writer.Write((byte)etype);
    //                Action<BinaryWriter, Type, dynamic, BinaryReader> writeFunc;
    //                switch (etype) {
    //                    case SerializeTypes.IsDataTable:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteDataTableWithMap2;
    //                        break;
    //                    case SerializeTypes.IsArray:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteArrayWithMap;
    //                        break;
    //                    case SerializeTypes.IsList:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteListWithMap;
    //                        break;
    //                    case SerializeTypes.IsDictionary:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteDictionaryWithMap;
    //                        break;
    //                    case SerializeTypes.IsEnumerable:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteEnumerableWithMap;
    //                        break;
    //                    case SerializeTypes.IsKeyValue:
    //                        writeFunc = WriteKeyValueWithMap;
    //                        break;
    //                    case SerializeTypes.IsTuple:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteTupleWithMap;
    //                        break;
    //                    case SerializeTypes.IsHyperSerialize:
    //                        writer.Write(value == null);
    //                        if (value == null) return;
    //                        writeFunc = WriteClassWithMap;
    //                        break;
    //                    default:
    //                        throw new InvalidOperationException(string.Format("Type {0} is not supported", typeCode));
    //                }

    //                writeFunc(writer, type, value, rmap);
    //                break;
    //        }
    //        //we should make sure the cursor is set to the next node, no matter if the child nodes returns prematurely or not.
    //        rmap.BaseStream.Position = position + numbytes;
    //    }
    //    static private void WriteAllTypes(BinaryWriter writer, Type type, dynamic value)
    //    {
    //        var typeCode = Type.GetTypeCode(type);
    //        switch (typeCode) {
    //            case TypeCode.Decimal:
    //            case TypeCode.Double:
    //            case TypeCode.Single:
    //            case TypeCode.Int64:
    //            case TypeCode.UInt64:
    //            case TypeCode.UInt32:
    //            case TypeCode.Int16:
    //            case TypeCode.UInt16:
    //            case TypeCode.SByte:
    //            case TypeCode.Boolean:
    //            case TypeCode.Char:
    //                writer.Write(value);
    //                break;
    //            case TypeCode.Byte:
    //                writer.Write((Byte)value);   //in case type if Enum
    //                break;
    //            case TypeCode.Int32:
    //                writer.Write((int)value);    //in case type if Enum
    //                break;
    //            case TypeCode.String:
    //                writer.Write(value == null);
    //                if (value == null) return;
    //                writer.Write(value);
    //                break;
    //            case TypeCode.DateTime:
    //                var time = ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
    //                writer.Write(time);
    //                break;

    //            default:
    //                var etype = GetSerializaionType(type);
    //                Action<BinaryWriter, Type, dynamic> writeFunc;
    //                switch (etype) {
    //                    case SerializeTypes.IsDataTable:
    //                        writeFunc = WriteDataTable2;
    //                        break;
    //                    case SerializeTypes.IsArray:
    //                        writeFunc = WriteArray;
    //                        break;
    //                    case SerializeTypes.IsList:
    //                        writeFunc = WriteList;
    //                        break;
    //                    case SerializeTypes.IsDictionary:
    //                        writeFunc = WriteDictionary;
    //                        break;
    //                    case SerializeTypes.IsEnumerable:
    //                        writeFunc = WriteEnumerable;
    //                        break;
    //                    case SerializeTypes.IsKeyValue:
    //                        writeFunc = WriteKeyValue;
    //                        break;
    //                    case SerializeTypes.IsTuple:
    //                        writeFunc = WriteTuple;
    //                        break;
    //                    default:
    //                        if (IsHiPerfSerializable(type)) {
    //                            etype = SerializeTypes.IsHyperSerialize;
    //                            writeFunc = WriteClass;
    //                        }
    //                        else {
    //                            etype = SerializeTypes.Invalid;
    //                            throw new InvalidOperationException(string.Format("Type {0} is not supported", typeCode));
    //                        }
    //                        break;
    //                }

    //                writer.Write((byte)etype);
    //                if (etype != SerializeTypes.IsKeyValue) {
    //                    writer.Write(value == null);
    //                    if (value == null) return;
    //                }

    //                writeFunc(writer, type, value);
    //                break;
    //        }
    //    }
    //    static private dynamic ReadAllTypes(BinaryReader reader, Type type)
    //    {
    //        var typeCode = Type.GetTypeCode(type);
    //        switch (typeCode) {
    //            case TypeCode.Decimal:
    //                return reader.ReadDecimal();
    //            case TypeCode.Double:
    //                return reader.ReadDouble();
    //            case TypeCode.Single:
    //                return reader.ReadSingle();
    //            case TypeCode.Int64:
    //                return reader.ReadInt64();
    //            case TypeCode.UInt64:
    //                return reader.ReadUInt64();
    //            case TypeCode.Int32:
    //                return reader.ReadInt32();
    //            case TypeCode.UInt32:
    //                return reader.ReadUInt32();
    //            case TypeCode.Int16:
    //                return reader.ReadInt16();
    //            case TypeCode.UInt16:
    //                return reader.ReadUInt16();
    //            case TypeCode.SByte:
    //                return reader.ReadSByte();
    //            case TypeCode.Byte:
    //                return reader.ReadByte();
    //            case TypeCode.Boolean:
    //                return reader.ReadBoolean();
    //            case TypeCode.Char:
    //                return reader.ReadChar();
    //            case TypeCode.String:
    //                if (reader.ReadBoolean()) return null;
    //                else return reader.ReadString();
    //            case TypeCode.DateTime:
    //                var timeStr = reader.ReadString();
    //                DateTime time;
    //                DateTime.TryParseExact(timeStr, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
    //                return time;
    //            default:
    //                var etype = (SerializeTypes)reader.ReadByte();
    //                Func<BinaryReader, Type, dynamic> readFunc;
    //                switch (etype) {
    //                    case SerializeTypes.IsDataTable:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadDataTable2;
    //                        break;
    //                    case SerializeTypes.IsArray:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadArray;
    //                        break;
    //                    case SerializeTypes.IsList:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadList;
    //                        break;
    //                    case SerializeTypes.IsDictionary:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadDictionary;
    //                        break;
    //                    case SerializeTypes.IsEnumerable:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadEnumerable;
    //                        break;
    //                    case SerializeTypes.IsKeyValue:
    //                        readFunc = ReadKeyValue;
    //                        break;
    //                    case SerializeTypes.IsTuple:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadTuple;
    //                        break;
    //                    case SerializeTypes.IsHyperSerialize:
    //                        if (reader.ReadBoolean()) return null;
    //                        readFunc = ReadClass;
    //                        break;
    //                    default:
    //                        throw new InvalidOperationException(string.Format("Type {0} is not supported", typeCode));
    //                }

    //                return readFunc(reader, type);
    //        }
    //    }

    //    #region DataTable
    //    //static private void CreateMapDataTable<T>(BinaryWriter writer, Type objType, T obj)
    //    //{
    //    //    DataTable table = (DataTable)Convert.ChangeType(obj, typeof(DataTable));
    //    //    if (table.Columns.Count == 0)
    //    //        throw new InvalidOperationException("DataTable must have at least one column to create map.");

    //    //    var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    //    //    var columnTypes = table.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();
    //    //    if (table.Rows.Count == 0)
    //    //        throw new InvalidOperationException("DataTable must have at least one one row to create map.");

    //    //    for (int i = 0; i < columnNames.Count; i++) {
    //    //        var type = columnTypes[i].MakeArrayType();
    //    //        var typeCode = Type.GetTypeCode(columnTypes[i]);
    //    //        switch (typeCode) {
    //    //            case TypeCode.Double: {
    //    //                    var col = (from DataRow row in table.Rows select (Double)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Single: {
    //    //                    var col = (from DataRow row in table.Rows select (Single)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int64: {
    //    //                    var col = (from DataRow row in table.Rows select (Int64)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt64: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt64)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int32: {
    //    //                    var col = (from DataRow row in table.Rows select (Int32)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt32: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt32)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int16: {
    //    //                    var col = (from DataRow row in table.Rows select (Int16)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt16: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt16)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.SByte: {
    //    //                    var col = (from DataRow row in table.Rows select (SByte)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Byte: {
    //    //                    var col = (from DataRow row in table.Rows select (Byte)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Boolean: {
    //    //                    var col = (from DataRow row in table.Rows select (Boolean)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Char: {
    //    //                    var col = (from DataRow row in table.Rows select (Char)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.DateTime: {
    //    //                    var col = (from DataRow row in table.Rows select (DateTime)row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            default: {
    //    //                    var col = (from DataRow row in table.Rows select row[i]).ToArray();
    //    //                    CreateMapAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //        }
    //    //    }
    //    //}
    //    //static private void WriteDataTableWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    //{
    //    //    DataTable table = (DataTable)Convert.ChangeType(obj, typeof(DataTable));
    //    //    writer.Write(table.TableName);
    //    //    writer.Write(table.Columns.Count);
    //    //    if (table.Columns.Count == 0)
    //    //        return;

    //    //    var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    //    //    var columnTypes = table.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();
    //    //    foreach (DataColumn col in table.Columns) {
    //    //        writer.Write(col.ColumnName);
    //    //        writer.Write(col.DataType.FullName);
    //    //    }

    //    //    writer.Write(table.Rows.Count);
    //    //    if (table.Rows.Count == 0)
    //    //        return;

    //    //    //foreach (DataRow row in table.Rows)
    //    //    //{
    //    //    //    for (int i = 0; i < columnNames.Count; i++)
    //    //    //        WriteAllTypes(writer, columnTypes[i], row[columnNames[i]]);
    //    //    //}
    //    //    for (int i = 0; i < columnNames.Count; i++) {
    //    //        var type = columnTypes[i].MakeArrayType();
    //    //        var typeCode = Type.GetTypeCode(columnTypes[i]);
    //    //        switch (typeCode) {
    //    //            //case TypeCode.Decimal:
    //    //            //    {
    //    //            //        var col = (from DataRow row in table.Rows select (Decimal)row[i]).ToArray();
    //    //            //        WriteAllTypesWithMap(writer, type, col, rmap);
    //    //            //        break;
    //    //            //    }
    //    //            case TypeCode.Double: {
    //    //                    var col = (from DataRow row in table.Rows select (Double)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Single: {
    //    //                    var col = (from DataRow row in table.Rows select (Single)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int64: {
    //    //                    var col = (from DataRow row in table.Rows select (Int64)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt64: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt64)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int32: {
    //    //                    var col = (from DataRow row in table.Rows select (Int32)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt32: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt32)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int16: {
    //    //                    var col = (from DataRow row in table.Rows select (Int16)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt16: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt16)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.SByte: {
    //    //                    var col = (from DataRow row in table.Rows select (SByte)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Byte: {
    //    //                    var col = (from DataRow row in table.Rows select (Byte)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Boolean: {
    //    //                    var col = (from DataRow row in table.Rows select (Boolean)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Char: {
    //    //                    var col = (from DataRow row in table.Rows select (Char)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.DateTime: {
    //    //                    var col = (from DataRow row in table.Rows select (DateTime)row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //            default: {
    //    //                    var col = (from DataRow row in table.Rows select row[i]).ToArray();
    //    //                    WriteAllTypesWithMap(writer, type, col, rmap);
    //    //                    break;
    //    //                }
    //    //        }
    //    //    }
    //    //}
    //    //static private void WriteDataTable<T>(BinaryWriter writer, Type objType, T obj)
    //    //{
    //    //    DataTable table = (DataTable)Convert.ChangeType(obj, typeof(DataTable));
    //    //    writer.Write(table.TableName);
    //    //    writer.Write(table.Columns.Count);
    //    //    if (table.Columns.Count == 0) return;

    //    //    var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    //    //    var columnTypes = table.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();
    //    //    foreach (DataColumn col in table.Columns) {
    //    //        writer.Write(col.ColumnName);
    //    //        writer.Write(col.DataType.FullName);
    //    //    }

    //    //    writer.Write(table.Rows.Count);
    //    //    if (table.Rows.Count == 0) return;

    //    //    //foreach (DataRow row in table.Rows)
    //    //    //{
    //    //    //    for (int i = 0; i < columnNames.Count; i++)
    //    //    //        WriteAllTypes(writer, columnTypes[i], row[columnNames[i]]);
    //    //    //}
    //    //    for (int i = 0; i < columnNames.Count; i++) {
    //    //        var type = columnTypes[i].MakeArrayType();
    //    //        var typeCode = Type.GetTypeCode(columnTypes[i]);
    //    //        switch (typeCode) {
    //    //            //case TypeCode.Decimal:
    //    //            //    {
    //    //            //        var col = (from DataRow row in table.Rows select (Decimal)row[i]).ToArray();
    //    //            //        WriteAllTypes(writer, type, col);
    //    //            //        break;
    //    //            //    }
    //    //            case TypeCode.Double: {
    //    //                    var col = (from DataRow row in table.Rows select (Double)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Single: {
    //    //                    var col = (from DataRow row in table.Rows select (Single)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int64: {
    //    //                    var col = (from DataRow row in table.Rows select (Int64)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt64: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt64)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int32: {
    //    //                    var col = (from DataRow row in table.Rows select (Int32)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt32: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt32)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Int16: {
    //    //                    var col = (from DataRow row in table.Rows select (Int16)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.UInt16: {
    //    //                    var col = (from DataRow row in table.Rows select (UInt16)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.SByte: {
    //    //                    var col = (from DataRow row in table.Rows select (SByte)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Byte: {
    //    //                    var col = (from DataRow row in table.Rows select (Byte)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Boolean: {
    //    //                    var col = (from DataRow row in table.Rows select (Boolean)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.Char: {
    //    //                    var col = (from DataRow row in table.Rows select (Char)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            case TypeCode.DateTime: {
    //    //                    var col = (from DataRow row in table.Rows select (DateTime)row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //            default: {
    //    //                    var col = (from DataRow row in table.Rows select row[i]).ToArray();
    //    //                    WriteAllTypes(writer, type, col);
    //    //                    break;
    //    //                }
    //    //        }
    //    //    }
    //    //}
    //    //static private dynamic ReadDataTable(BinaryReader reader, Type objType)
    //    //{
    //    //    dynamic obj = Activator.CreateInstance(objType);   //given type must support zero constructor

    //    //    DataTable table = (DataTable)Convert.ChangeType(obj, typeof(DataTable));

    //    //    table.TableName = reader.ReadString();

    //    //    var numColumn = reader.ReadInt32();
    //    //    if (numColumn <= 0) return obj;

    //    //    var columnNames = new List<string>();
    //    //    var columnTypes = new List<Type>();

    //    //    for (int i = 0; i < numColumn; i++) {
    //    //        var cn = reader.ReadString();
    //    //        var ct = Type.GetType(reader.ReadString());
    //    //        columnNames.Add(cn);
    //    //        columnTypes.Add(ct);
    //    //        table.Columns.Add(cn, ct);
    //    //    }

    //    //    var numRow = reader.ReadInt32();
    //    //    if (numRow <= 0) return obj;

    //    //    for (int i = 0; i < numRow; i++) table.Rows.Add();

    //    //    for (int i = 0; i < numColumn; i++) {
    //    //        var type = columnTypes[i].MakeArrayType();
    //    //        var column = ReadAllTypes(reader, type);
    //    //        table.Rows.Cast<DataRow>().Each((row, index) => row[i] = column[index]);
    //    //    }

    //    //    //DataColumn column = new DataColumn("name", typeof(int));
    //    //    //table.Columns.Add()
    //    //    //for (int i = 0; i < numRow; i++)
    //    //    //{
    //    //    //    var row = table.NewRow();
    //    //    //    for (int j = 0; j < numColumn; j++)
    //    //    //        row[columnNames[j]] = ReadAllTypes(reader, columnTypes[j]);
    //    //    //    table.Rows.Add(row);
    //    //    //}

    //    //    return obj;
    //    //}
    //    //static private void CreateMapDataTable2<T>(BinaryWriter writer, Type objType, T obj)
    //    //{
    //    //}
    //    //static private void WriteDataTableWithMap2<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    //{
    //    //    WriteDataTable2(writer, objType, obj);
    //    //}
    //    //static private void WriteDataTable2<T>(BinaryWriter writer, Type objType, T obj)
    //    //{
    //    //    DataTable table = (DataTable)Convert.ChangeType(obj, typeof(DataTable));
    //    //    //writer.Write(table.Columns.Count);
    //    //    //writer.Write(table.Rows.Count);
    //    //    //if (table.Columns.Count == 0 || table.Rows.Count == 0) return;
    //    //    //var position = writer.BaseStream.Position;
    //    //    //int numBytes = 0;
    //    //    //writer.Write(numBytes);
    //    //    using (var stream = new MemoryStream()) {
    //    //        DataSerializer.Serialize(stream, table);
    //    //        //stream.Seek(0, SeekOrigin.Begin);
    //    //        var buffer = stream.ToArray();
    //    //        writer.Write(table.TableName);
    //    //        writer.Write(buffer.Length);
    //    //        writer.Write(buffer, 0, buffer.Length);
    //    //    }
    //    //}
    //    //static private dynamic ReadDataTable2(BinaryReader reader, Type objType)
    //    //{
    //    //    var table = (DataTable)Activator.CreateInstance(objType);   //given type must support zero constructor
    //    //    table.TableName = reader.ReadString();
    //    //    var length = reader.ReadInt32();
    //    //    var buffer = reader.ReadBytes(length);
    //    //    using (var stream = new MemoryStream(buffer)) {
    //    //        var tbReader = DataSerializer.Deserialize(stream);
    //    //        table.Load(tbReader);
    //    //    }
    //    //    return table;
    //    //}
    //    #endregion

    //    static private void CreateMapArray<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        int count = ((IList)obj).Count;
    //        if (count == 0)
    //            throw new ArgumentException("Array is of zero length. We cannot create map for it");

    //        dynamic arr = obj;
    //        Type gType = arr[0].GetType();

    //        var isRealValueType = IsRealValueType(gType);
    //        var isDateTime = (gType == typeof(DateTime));
    //        if (isRealValueType)
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //        else if (isDateTime)
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //        else
    //            writer.Write((byte)SerializeTypes.IsOther);

    //        if (!isRealValueType && !isDateTime)
    //            CreateMapAllTypes(writer, gType, arr[0]);

    //    }
    //    static private void WriteArrayWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    {
    //        int count = ((IList)obj).Count;
    //        writer.Write(count);
    //        if (count == 0) return;               //nothing more to serialize

    //        dynamic arr = obj;
    //        Type gType = arr[0].GetType();
    //        writer.Write(gType.FullName);         //for array time we have to save the type information, because it cannot be obtained from objType: tocheck 

    //        var type = (SerializeTypes)rmap.ReadByte();  //array of RealValueType and DateTime can be serialized at one time
    //        writer.Write((byte)type);
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    int typeSize = Marshal.SizeOf(gType);
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var oaDate = new double[count];
    //                    for (int i = 0; i < count; i++)
    //                        oaDate[i] = arr[i].ToOADate();

    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(oaDate, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            default: {
    //                    long position = rmap.BaseStream.Position;   //because we generate the map only for the first element, we have to rewind the position for each element
    //                    foreach (var value in arr) {
    //                        rmap.BaseStream.Position = position;
    //                        WriteAllTypesWithMap(writer, gType, value, rmap);
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //    static private void WriteArray<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        int count = ((IList)obj).Count;
    //        writer.Write(count);
    //        if (count == 0) return;

    //        dynamic arr = obj;
    //        Type gType = arr[0].GetType();
    //        writer.Write(gType.FullName);
    //        var isRealValueType = IsRealValueType(gType);

    //        if (isRealValueType) {
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //            int typeSize = Marshal.SizeOf(gType);
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else if (gType == typeof(DateTime)) {
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //            var oaDate = new double[count];
    //            for (int i = 0; i < count; i++)
    //                oaDate[i] = arr[i].ToOADate();

    //            int typeSize = Marshal.SizeOf(typeof(double));
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(oaDate, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else {
    //            writer.Write((byte)SerializeTypes.IsOther);
    //            foreach (var value in arr)
    //                WriteAllTypes(writer, gType, value);
    //        }
    //    }
    //    static private dynamic ReadArray(BinaryReader reader, Type objType)
    //    {
    //        var count = reader.ReadInt32();
    //        if (count == 0) return null;

    //        dynamic obj = Activator.CreateInstance(objType, new object[] { count });

    //        //var lst = (IList)Activator.CreateInstance((typeof(List<>).MakeGenericType(gType)));

    //        var gTypeName = reader.ReadString();
    //        var gType = Type.GetType(gTypeName);

    //        var type = (SerializeTypes)reader.ReadByte();
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    int typeSize = Marshal.SizeOf(gType);
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, obj, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    for (int i = 0; i < count; i++)
    //                        obj[i] = DateTime.FromOADate(arr[i]);
    //                    break;
    //                }
    //            default: {
    //                    for (int i = 0; i < count; i++)
    //                        obj[i] = ReadAllTypes(reader, gType);
    //                    break;
    //                }
    //        }

    //        return obj;
    //    }

    //    static private void CreateMapList<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var count = ((IList)obj).Count;
    //        if (count == 0)
    //            throw new ArgumentException("List is of zero length. We cannot create mapStream for it");

    //        if (objType.GenericTypeArguments.ToList().Count != 1)
    //            throw new InvalidOperationException(string.Format("Generic IList {0} must have one GenericTypeArguments.", objType.Name));

    //        Type gType = objType.GenericTypeArguments[0];

    //        var isRealValueType = IsRealValueType(gType);
    //        var isDateTime = (gType == typeof(DateTime));
    //        if (isRealValueType)
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //        else if (isDateTime)
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //        else
    //            writer.Write((byte)SerializeTypes.IsOther);

    //        if (!isRealValueType && !isDateTime)
    //            CreateMapAllTypes(writer, gType, ((dynamic)obj)[0]);
    //    }
    //    static private void WriteListWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    {
    //        dynamic listObj = (IList)obj;
    //        var count = ((IList)obj).Count;
    //        writer.Write(count);
    //        if (count == 0) return;

    //        Type gType = objType.GenericTypeArguments[0];

    //        var type = (SerializeTypes)rmap.ReadByte();
    //        writer.Write((byte)type);
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    var arr = listObj.ToArray();
    //                    int typeSize = Marshal.SizeOf(gType);
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    for (int i = 0; i < count; i++)
    //                        arr[i] = listObj[i].ToOADate();

    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            default: {
    //                    long position = rmap.BaseStream.Position;
    //                    foreach (var value in listObj) {
    //                        rmap.BaseStream.Position = position;
    //                        WriteAllTypesWithMap(writer, gType, value, rmap);
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //    static private void WriteList<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        dynamic listObj = (IList)obj;
    //        var count = listObj.Count;
    //        writer.Write(count);
    //        if (count == 0) return;

    //        if (objType.GenericTypeArguments.ToList().Count != 1)
    //            throw new InvalidOperationException(string.Format("Generic IList {0} must have one GenericTypeArguments.", objType.Name));

    //        Type gType = objType.GenericTypeArguments[0];
    //        var isRealValueType = IsRealValueType(gType);

    //        if (isRealValueType) {
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //            var arr = listObj.ToArray();
    //            //dynamic arr = Activator.CreateInstance(gType.MakeArrayType(), new object[] { count });
    //            //listObj.CopyTo(arr, 0);

    //            int typeSize = Marshal.SizeOf(gType);
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else if (gType == typeof(DateTime)) {
    //            //ppForAllStocks.ToArray
    //            //var selectMethod = typeof(Enumerable).GetMethods().ToList().Find(m => m.Name == "Select" && m.GetParameters().ToList().Count == 2).MakeGenericMethod(new Type[] { gType, typeof(double) });
    //            //Func<DateTime, double> selector = (d) =>
    //            //    {
    //            //        return d.ToOADate();
    //            //    };
    //            //var arr = (IList)selectMethod.Invoke(listObj, new object[] { listObj, selector });
    //            //var arr2 = arr.ToArray();
    //            //var toList = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(new Type[] { gType });
    //            //var rrr = toList.Invoke(rr, new object[] { rr });
    //            //Type funcType = typeof(Func<,>).MakeGenericType(gType, typeof(double));
    //            //var del = Delegate.CreateDelegate(funcType, selectMethod);
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //            var arr = new double[count];
    //            for (int i = 0; i < count; i++) arr[i] = listObj[i].ToOADate();

    //            int typeSize = Marshal.SizeOf(typeof(double));
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else {
    //            writer.Write((byte)SerializeTypes.IsOther);

    //            foreach (var value in listObj)
    //                WriteAllTypes(writer, gType, value);
    //        }
    //    }
    //    static private dynamic ReadList(BinaryReader reader, Type objType)
    //    {
    //        var count = reader.ReadInt32();
    //        if (count == 0) return Activator.CreateInstance(objType);

    //        dynamic obj = null;
    //        var gType = objType.GenericTypeArguments[0];
    //        var type = (SerializeTypes)reader.ReadByte();
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    dynamic arr = Activator.CreateInstance(gType.MakeArrayType(), new object[] { count });
    //                    int typeSize = Marshal.SizeOf(gType);
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    var toList = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(new Type[] { gType });
    //                    obj = toList.Invoke(arr, new object[] { arr });
    //                    //obj = arr.ToList();
    //                    //obj.AddRange(arr);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    obj = arr.Select(d => DateTime.FromOADate(d)).ToList();
    //                    //obj.AddRange(arr.Select(d => DateTime.FromOADate(d)).ToList());
    //                    break;
    //                }
    //            default: {
    //                    obj = Activator.CreateInstance(objType, new object[] { count });
    //                    for (int i = 0; i < count; i++) {
    //                        var value = ReadAllTypes(reader, gType);
    //                        obj.Add(value);
    //                    }
    //                    break;
    //                }
    //        }
    //        return obj;
    //    }

    //    static private void CreateMapDictionary<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        dynamic dictObj = (IDictionary)obj;
    //        int count = ((IDictionary)obj).Count;
    //        if (count == 0)
    //            throw new ArgumentException("Dictionary is of zero length. We cannot create mapStream for it");

    //        if (objType.GenericTypeArguments.ToList().Count != 2)
    //            throw new InvalidOperationException(string.Format("Generic IDictionary {0} must have two generic type argumments.", objType.Name));

    //        var kType = objType.GenericTypeArguments[0];
    //        var vType = objType.GenericTypeArguments[1];

    //        //Type type = dictObj.Keys.GetType();
    //        //var keyCopyToMethod = type.GetMethods().ToList().Find(m => m.Name == "CopyTo");
    //        //type = dictObj.Values.GetType();
    //        //var valueCopyToMethod = type.GetMethods().ToList().Find(m => m.Name == "CopyTo");

    //        var isRealValueType = IsRealValueType(kType);
    //        var isDateTime = (kType == typeof(DateTime));
    //        if (isRealValueType)
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //        else if (isDateTime)
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //        else
    //            writer.Write((byte)SerializeTypes.IsOther);

    //        if (!isRealValueType && !isDateTime) {
    //            var method = typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(kType);
    //            var element = method.Invoke(dictObj.Keys, new object[] { dictObj.Keys, 0 });
    //            CreateMapAllTypes(writer, kType, element);
    //        }

    //        isRealValueType = IsRealValueType(vType);
    //        isDateTime = (vType == typeof(DateTime));
    //        if (isRealValueType)
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //        else if (isDateTime)
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //        else
    //            writer.Write((byte)SerializeTypes.IsOther);

    //        if (!isRealValueType && !isDateTime) {
    //            var method = typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(vType);
    //            var element = method.Invoke(dictObj.Values, new object[] { dictObj.Values, 0 });
    //            CreateMapAllTypes(writer, vType, element);
    //        }
    //    }
    //    static private void WriteDictionaryWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    {
    //        var dictObj = (IDictionary)obj;
    //        int count = ((IDictionary)obj).Count;
    //        writer.Write(count);
    //        if (count == 0) return;

    //        var kType = objType.GenericTypeArguments[0];
    //        var vType = objType.GenericTypeArguments[1];

    //        var type = (SerializeTypes)rmap.ReadByte();
    //        writer.Write((byte)type);
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    dynamic keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //                    dictObj.Keys.CopyTo(keys, 0);
    //                    int typeSize = Marshal.SizeOf(kType);
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(keys, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    dynamic keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //                    dictObj.Keys.CopyTo(keys, 0);

    //                    var arr = new double[count];
    //                    for (int i = 0; i < count; i++)
    //                        arr[i] = keys[i].ToOADate();

    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            default: {
    //                    long position = rmap.BaseStream.Position;
    //                    foreach (var key in dictObj.Keys) {
    //                        rmap.BaseStream.Position = position;
    //                        WriteAllTypesWithMap(writer, kType, key, rmap);
    //                    }
    //                    break;
    //                }
    //        }

    //        type = (SerializeTypes)rmap.ReadByte();
    //        writer.Write((byte)type);
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    dynamic values = Activator.CreateInstance(vType.MakeArrayType(), new object[] { count });
    //                    dictObj.Values.CopyTo(values, 0);
    //                    int typeSize = Marshal.SizeOf(vType);
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(values, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    dynamic values = Activator.CreateInstance(vType.MakeArrayType(), new object[] { count });
    //                    dictObj.Values.CopyTo(values, 0);

    //                    var arr = new double[count];
    //                    for (int i = 0; i < count; i++)
    //                        arr[i] = values[i].ToOADate();

    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            default: {
    //                    long position = rmap.BaseStream.Position;
    //                    foreach (dynamic value in dictObj.Values) {
    //                        rmap.BaseStream.Position = position;
    //                        WriteAllTypesWithMap(writer, vType, value, rmap);
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //    static private void WriteDictionary<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var dictObj = (IDictionary)obj;

    //        int count = dictObj.Count;
    //        writer.Write(count);
    //        if (count == 0) return;

    //        if (objType.GenericTypeArguments.ToList().Count != 2)
    //            throw new InvalidOperationException(string.Format("Generic IDictionary {0} must have two generic type argumments.", objType.Name));

    //        var kType = objType.GenericTypeArguments[0];
    //        var vType = objType.GenericTypeArguments[1];
    //        //var keyValuePair = Activator.CreateInstance(type, new[] { key, val });
    //        //var gType = typeof(KeyValuePair<,>).MakeGenericType(new Type[] { kType, vType });
    //        //Type type = dictObj.Keys.GetType();
    //        //var keyCopyToMethod = type.GetMethods().ToList().Find(m => m.Name == "CopyTo");
    //        //type = dictObj.Values.GetType();
    //        //var valueCopyToMethod = type.GetMethods().ToList().Find(m => m.Name == "CopyTo");

    //        /////////////////////////////////////////////////////////////////////////////////////
    //        var isRealValueType = IsRealValueType(kType);
    //        if (isRealValueType) {
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //            dynamic keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //            dictObj.Keys.CopyTo(keys, 0);
    //            //keyCopyToMethod.Invoke(dictObj.Keys, new object[] { keys, 0 });
    //            //var keys = dictObj.Keys.ToArray();
    //            int typeSize = Marshal.SizeOf(kType);
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(keys, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else if (kType == typeof(DateTime)) {
    //            writer.Write((byte)SerializeTypes.IsDateTime);

    //            dynamic keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //            dictObj.Keys.CopyTo(keys, 0);

    //            var arr = new double[count];
    //            for (int i = 0; i < count; i++)
    //                arr[i] = keys[i].ToOADate();

    //            int typeSize = Marshal.SizeOf(typeof(double));
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else {
    //            writer.Write((byte)SerializeTypes.IsOther);
    //            foreach (var key in dictObj.Keys)
    //                WriteAllTypes(writer, kType, key);
    //        }

    //        ////////////////////////////////////////////////////////////////////////////////
    //        isRealValueType = IsRealValueType(vType);
    //        if (isRealValueType) {
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //            dynamic values = Activator.CreateInstance(vType.MakeArrayType(), new object[] { count });
    //            dictObj.Values.CopyTo(values, 0);
    //            int typeSize = Marshal.SizeOf(vType);
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(values, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else if (vType == typeof(DateTime)) {
    //            writer.Write((byte)SerializeTypes.IsDateTime);

    //            dynamic values = Activator.CreateInstance(vType.MakeArrayType(), new object[] { count });
    //            dictObj.Values.CopyTo(values, 0);

    //            var arr = new double[count];
    //            for (int i = 0; i < count; i++)
    //                arr[i] = values[i].ToOADate();

    //            int typeSize = Marshal.SizeOf(typeof(double));
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else {
    //            writer.Write((byte)SerializeTypes.IsOther);
    //            foreach (var value in dictObj.Values)
    //                WriteAllTypes(writer, vType, value);
    //        }
    //    }
    //    static private dynamic ReadDictionary(BinaryReader reader, Type objType)
    //    {
    //        var count = reader.ReadInt32();
    //        if (count == 0) return Activator.CreateInstance(objType);

    //        //var obj = Activator.CreateInstance(objType, new object[] { count });
    //        dynamic obj = Activator.CreateInstance(objType);

    //        //var dictObj = (IDictionary)obj;
    //        var kType = objType.GenericTypeArguments[0];
    //        var vType = objType.GenericTypeArguments[1];
    //        //var kType = typeof(string).Assembly.GetType(kTypeName);
    //        //var vType = typeof(string).Assembly.GetType(vTypeName);
    //        //var gType = typeof(KeyValuePair<,>).MakeGenericType(new Type[] { kType, vType });

    //        dynamic keys = null;
    //        dynamic values = null;

    //        //key
    //        var type = (SerializeTypes)reader.ReadByte();
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //                    int typeSize = Marshal.SizeOf(kType);
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, keys, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    keys = arr.Select(d => DateTime.FromOADate(d)).ToArray();
    //                    break;
    //                }
    //            default: {
    //                    keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //                    for (int i = 0; i < count; i++)
    //                        keys[i] = ReadAllTypes(reader, kType);
    //                    break;
    //                }
    //        }

    //        //value
    //        type = (SerializeTypes)reader.ReadByte();
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    values = Activator.CreateInstance(vType.MakeArrayType(), new object[] { count });
    //                    int typeSize = Marshal.SizeOf(vType);
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    values = arr.Select(d => DateTime.FromOADate(d)).ToArray();
    //                    break;
    //                }
    //            default: {
    //                    values = Activator.CreateInstance(vType.MakeArrayType(), new object[] { count });
    //                    for (int i = 0; i < count; i++)
    //                        values[i] = ReadAllTypes(reader, vType);
    //                    break;
    //                }
    //        }

    //        for (int i = 0; i < count; i++)
    //            obj.Add(keys[i], values[i]);

    //        return obj;
    //    }

    //    static private void CreateMapEnumerable<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var enumObj = (IEnumerable)obj;

    //        int count = 0;
    //        if (objType.GenericTypeArguments.ToList().Count != 1)
    //            throw new InvalidOperationException(string.Format("Generic IEnumerable {0} must have one generic type argumments.", objType.Name));

    //        var gType = objType.GenericTypeArguments[0];
    //        var listType = typeof(List<>).MakeGenericType(new Type[] { gType });
    //        var list = (IList)Activator.CreateInstance(listType);
    //        foreach (var item in enumObj) {
    //            list.Add(item);
    //            ++count;
    //        }

    //        if (count == 0)
    //            throw new ArgumentException("IEnumerable is of zero length. We cannot create mapStream for it");

    //        var isRealValueType = IsRealValueType(gType);
    //        var isDateTime = (gType == typeof(DateTime));
    //        if (isRealValueType)
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //        else if (isDateTime)
    //            writer.Write((byte)SerializeTypes.IsDateTime);
    //        else
    //            writer.Write((byte)SerializeTypes.IsOther);

    //        if (!isRealValueType && !isDateTime)
    //            WriteAllTypes(writer, gType, list[0]);
    //    }
    //    static private void WriteEnumerableWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    {
    //        var enumObj = (IEnumerable)obj;
    //        int count = 0;

    //        var gType = objType.GenericTypeArguments[0];
    //        var listType = typeof(List<>).MakeGenericType(new Type[] { gType });
    //        var list = (IList)Activator.CreateInstance(listType);
    //        foreach (var item in enumObj) {
    //            list.Add(item);
    //            ++count;
    //        }
    //        writer.Write(count);
    //        if (count == 0) return;

    //        var type = (SerializeTypes)rmap.ReadByte();
    //        writer.Write((byte)type);
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    var toArrayMethod = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(gType);
    //                    dynamic arr = toArrayMethod.Invoke(list, new object[] { list });

    //                    int typeSize = Marshal.SizeOf(gType);
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    for (int i = 0; i < count; i++)
    //                        arr[i] = ((DateTime)list[i]).ToOADate();

    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = new byte[count * typeSize];
    //                    Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //                    writer.Write(bytes, 0, bytes.Length);
    //                    break;
    //                }
    //            default: {
    //                    long position = rmap.BaseStream.Position;
    //                    foreach (var value in list) {
    //                        rmap.BaseStream.Position = position;
    //                        WriteAllTypesWithMap(writer, gType, value, rmap);
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //    static private void WriteEnumerable<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        if (objType.GenericTypeArguments.ToList().Count != 1)
    //            throw new InvalidOperationException(string.Format("Generic IEnumerable {0} must have one generic type argumments.", objType.Name));

    //        var gType = objType.GenericTypeArguments[0];

    //        var addMethod = objType.GetMethods().ToList().Find(m => m.Name.Contains("Add") && m.GetParameters().ToList().Count == 1 && m.GetParameters()[0].ParameterType == gType);
    //        if (addMethod == null)
    //            throw new InvalidOperationException(string.Format("Generic IEnumerable {0} must contain Add function.", objType.Name));

    //        var enumObj = (IEnumerable)obj;
    //        int count = 0;
    //        var listType = typeof(List<>).MakeGenericType(new Type[] { gType });
    //        var list = (IList)Activator.CreateInstance(listType);
    //        foreach (var item in enumObj) {
    //            list.Add(item);
    //            ++count;
    //        }

    //        writer.Write(count);
    //        if (count == 0) return;

    //        var isRealValueType = IsRealValueType(gType);
    //        if (isRealValueType) {
    //            writer.Write((byte)SerializeTypes.IsRealValueType);
    //            //var newObj = Convert.ChangeType(obj, type);
    //            //var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(gType);
    //            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(gType);
    //            //var args = toArrayMethod.GetParameters().ToList();
    //            //var arr = toArrayMethod.Invoke(newObj, new object[] { newObj });
    //            //dynamic keys = Activator.CreateInstance(kType.MakeArrayType(), new object[] { count });
    //            dynamic arr = toArrayMethod.Invoke(list, new object[] { list });

    //            int typeSize = Marshal.SizeOf(gType);
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else if (gType == typeof(DateTime)) {
    //            writer.Write((byte)SerializeTypes.IsDateTime);

    //            var arr = new double[count];
    //            for (int i = 0; i < count; i++)
    //                arr[i] = ((DateTime)list[i]).ToOADate();

    //            int typeSize = Marshal.SizeOf(typeof(double));
    //            byte[] bytes = new byte[count * typeSize];
    //            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
    //            writer.Write(bytes, 0, bytes.Length);
    //        }
    //        else {
    //            writer.Write((byte)SerializeTypes.IsOther);
    //            foreach (var value in list)
    //                WriteAllTypes(writer, gType, value);
    //        }
    //    }
    //    static private dynamic ReadEnumerable(BinaryReader reader, Type objType)
    //    {
    //        var obj = Activator.CreateInstance(objType);   //given type must support zero constructor

    //        var count = reader.ReadInt32();
    //        if (count == 0) return obj;

    //        var enumObj = (IEnumerable)obj;

    //        var gType = objType.GenericTypeArguments[0];
    //        var addMethod = objType.GetMethods().ToList().Find(m => m.Name.Contains("Add") && m.GetParameters().ToList().Count == 1 && m.GetParameters()[0].ParameterType == gType);
    //        //if (addMethod == null)
    //        //    throw new InvalidOperationException(string.Format("Generic IEnumerable {0} must contain Add function.", type.Name));

    //        //var mlist = type.GetMethods().Where(m=>m.Name.Contains("Add")).ToList();
    //        //if (mlist == null || mlist.Count <= 0)
    //        //    throw new InvalidOperationException(string.Format("Generic IEnumerable {0} must contain Add function.", type.Name));
    //        //var addMethod = mlist.Find(m => m.GetParameters().ToList().Count == 1 && m.GetParameters()[0].ParameterType == gType);
    //        //if (addMethod == null)
    //        //    throw new InvalidOperationException(string.Format("Generic IEnumerable {0} must contain Add function.", type.Name));
    //        //var m = type.GetMethod("AddElement");
    //        //var ps = m.GetParameters();
    //        //var tt = ps[0].ParameterType;
    //        //Console.WriteLine(m.Name);
    //        var type = (SerializeTypes)reader.ReadByte();
    //        switch (type) {
    //            case SerializeTypes.IsRealValueType: {
    //                    dynamic arr = Activator.CreateInstance(gType.MakeArrayType(), new object[] { count });
    //                    int typeSize = Marshal.SizeOf(gType);
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    for (int i = 0; i < count; i++)
    //                        addMethod.Invoke(enumObj, new object[] { arr[i] });
    //                    break;
    //                }
    //            case SerializeTypes.IsDateTime: {
    //                    var arr = new double[count];
    //                    int typeSize = Marshal.SizeOf(typeof(double));
    //                    byte[] bytes = reader.ReadBytes(count * typeSize);
    //                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
    //                    arr.ToList().ForEach(d => addMethod.Invoke(enumObj, new object[] { DateTime.FromOADate(d) }));
    //                    break;
    //                }
    //            default: {
    //                    for (int i = 0; i < count; i++) {
    //                        var value = ReadAllTypes(reader, gType);
    //                        addMethod.Invoke(enumObj, new object[] { value });
    //                    }
    //                    break;
    //                }
    //        }
    //        return obj;
    //    }

    //    static private void CreateMapClass<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var properties = objType.GetRuntimeProperties().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //        var fields = objType.GetRuntimeFields().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //        var hasHiPerfProperty = (properties != null && properties.Count > 0);
    //        var hasHiPerfField = (fields != null && fields.Count > 0);
    //        writer.Write(hasHiPerfProperty);
    //        writer.Write(hasHiPerfField);

    //        var hasHiPerfMember = (hasHiPerfProperty || hasHiPerfField);
    //        if (!hasHiPerfMember)                     //=> this is error case
    //            throw new Exception(string.Format("HiPerfSerializable specified however no HiPerfMember defined for {0}", objType.Name));

    //        if (hasHiPerfProperty) {
    //            //Serialize members one by one
    //            var propertiesList = properties.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();
    //            if (propertiesList.Count != propertiesList.Distinct().ToList().Count)
    //                throw new InvalidOperationException("ID is not unique HiPerfMember. We cannot proceed to serialize.");

    //            foreach (var p in propertiesList) {
    //                var value = p.GetValue(obj);
    //                CreateMapAllTypes(writer, p.PropertyType, value);
    //            }
    //        }
    //        if (fields != null && fields.Count > 0) {
    //            var fieldsList = fields.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();
    //            if (fieldsList.Count != fieldsList.Distinct().ToList().Count)
    //                throw new Exception("ID is not unique HiPerfMember. We cannot proceed to serialize.");

    //            foreach (var f in fieldsList) {
    //                var value = f.GetValue(obj);
    //                CreateMapAllTypes(writer, f.FieldType, value);
    //            }
    //        }
    //    }
    //    static private void WriteClassWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    {
    //        var hasHiPerfProperty = rmap.ReadBoolean();
    //        var hasHiPerfField = rmap.ReadBoolean();
    //        writer.Write(hasHiPerfProperty);
    //        writer.Write(hasHiPerfField);

    //        if (hasHiPerfProperty) {
    //            //Serialize members one by one
    //            var properties = objType.GetRuntimeProperties().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //            var propertiesList = properties.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();

    //            foreach (var p in propertiesList) {
    //                var value = p.GetValue(obj);
    //                WriteAllTypesWithMap(writer, p.PropertyType, value, rmap);
    //            }
    //        }
    //        if (hasHiPerfField) {
    //            var fields = objType.GetRuntimeFields().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //            var fieldsList = fields.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();

    //            foreach (var f in fieldsList) {
    //                var value = f.GetValue(obj);
    //                WriteAllTypesWithMap(writer, f.FieldType, value, rmap);
    //            }
    //        }
    //    }
    //    static private void WriteClass<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var properties = objType.GetRuntimeProperties().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //        var fields = objType.GetRuntimeFields().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //        var hasHiPerfProperty = (properties != null && properties.Count > 0);
    //        var hasHiPerfField = (fields != null && fields.Count > 0);
    //        writer.Write(hasHiPerfProperty);
    //        writer.Write(hasHiPerfField);

    //        var hasHiPerfMember = (hasHiPerfProperty || hasHiPerfField);
    //        if (!hasHiPerfMember)
    //            throw new Exception(string.Format("HiPerfSerializable specified however no HiPerfMember defined for {0}", objType.Name));

    //        if (hasHiPerfProperty) {
    //            //Serialize members one by one
    //            var propertiesList = properties.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();
    //            if (propertiesList.Count != propertiesList.Distinct().ToList().Count)
    //                throw new InvalidOperationException("ID is not unique HiPerfMember. We cannot proceed to serialize.");

    //            foreach (var p in propertiesList) {
    //                var value = p.GetValue(obj);
    //                WriteAllTypes(writer, p.PropertyType, value);
    //            }
    //        }
    //        if (hasHiPerfField) {
    //            var fieldsList = fields.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();
    //            if (fieldsList.Count != fieldsList.Distinct().ToList().Count)
    //                throw new Exception("ID is not unique HiPerfMember. We cannot proceed to serialize.");

    //            foreach (var f in fieldsList) {
    //                var value = f.GetValue(obj);
    //                WriteAllTypes(writer, f.FieldType, value);
    //            }
    //        }
    //    }
    //    static private dynamic ReadClass(BinaryReader reader, Type objType)
    //    {
    //        var obj = Activator.CreateInstance(objType);   //given type must support zero constructor

    //        var hasHiPerfProperty = reader.ReadBoolean();
    //        var hasHiPerfField = reader.ReadBoolean();

    //        if (hasHiPerfProperty) {
    //            var properties = objType.GetRuntimeProperties().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //            properties = properties.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();

    //            foreach (var p in properties) {
    //                var value = ReadAllTypes(reader, p.PropertyType);
    //                p.SetValue(obj, value);
    //            }
    //        }
    //        if (hasHiPerfField) {
    //            var fields = objType.GetRuntimeFields().Where(p => p.GetCustomAttributes(true).Any(att => att is HiPerfMember)).ToList();
    //            fields = fields.OrderBy(p => ((HiPerfMember)p.GetCustomAttribute(typeof(HiPerfMember), true)).ID).ToList();

    //            foreach (var f in fields) {
    //                var value = ReadAllTypes(reader, f.FieldType);
    //                f.SetValue(obj, value);
    //            }
    //        }

    //        return obj;
    //    }

    //    static private void CreateMapTuple<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var props = objType.GetProperties().ToList();
    //        props.ForEach(p => CreateMapAllTypes(writer, p.PropertyType, p.GetValue(obj)));
    //    }
    //    static private void WriteTupleWithMap<T>(BinaryWriter writer, Type objType, T obj, BinaryReader rmap)
    //    {
    //        var props = objType.GetProperties().ToList();
    //        props.ForEach(p => WriteAllTypesWithMap(writer, p.PropertyType, p.GetValue(obj), rmap));
    //    }
    //    static private void WriteTuple<T>(BinaryWriter writer, Type objType, T obj)
    //    {
    //        var props = objType.GetProperties().ToList();
    //        props.ForEach(p => WriteAllTypes(writer, p.PropertyType, p.GetValue(obj)));
    //    }
    //    static private dynamic ReadTuple(BinaryReader reader, Type objType)
    //    {
    //        var args = new List<object>();
    //        var gTypes = objType.GenericTypeArguments.ToList();
    //        gTypes.ForEach(t => args.Add(ReadAllTypes(reader, t)));
    //        var obj = Activator.CreateInstance(objType, args.ToArray());
    //        return obj;
    //    }

    //    #endregion
    //    #region public serialize/deserialize functions
    //    static private MemoryStream CreateSerializationMap(Type type, dynamic obj)
    //    {
    //        try {
    //            var stream = new MemoryStream();
    //            var writer = new BinaryWriter(stream);
    //            CreateMapAllTypes(writer, type, obj);
    //            return stream;
    //        }
    //        catch {
    //            return null;
    //        }
    //    }

    //    static public void SerializeObject<T>(string filename, T obj, bool isShowTime = false)
    //    {
    //        long length = 0;
    //        var stopwatch = new Stopwatch();
    //        stopwatch.Start();
    //        {
    //            using (MemoryStream stream = new MemoryStream()) {
    //                SerializeObject(stream, obj);
    //                WriteMemoryStreamToFile(filename, stream);
    //                length = stream.Length;
    //            }
    //        }
    //        stopwatch.Stop();

    //        if (isShowTime)
    //            Console.WriteLine("Serialize:   time = {0}, length = {1}", stopwatch.ElapsedMilliseconds, length);
    //    }
    //    static public void SerializeObject<T>(MemoryStream stream, T obj, bool isShowTime = false)
    //    {
    //        var stopwatch = new Stopwatch();
    //        stopwatch.Start();
    //        {
    //            var writer = new BinaryWriter(stream);
    //            writer.Write(obj == null);
    //            if (obj != null) {
    //                var objType = obj.GetType();
    //                var ifCreateMap = true;
    //                var eType = GetSerializaionType(objType);
    //                switch (eType) {
    //                    case SerializeTypes.IsArray:
    //                    case SerializeTypes.IsList:
    //                    case SerializeTypes.IsDictionary:
    //                    case SerializeTypes.IsEnumerable:
    //                        ifCreateMap = true;
    //                        break;
    //                    default:
    //                        ifCreateMap = false;
    //                        break;
    //                }

    //                var isDone = false;
    //                //if it is worthwhile to create serialization mapStream
    //                if (ifCreateMap) {
    //                    using (MemoryStream mapStream = CreateSerializationMap(objType, obj))   //create mapStream
    //                    {
    //                        if (mapStream != null) {
    //                            mapStream.Position = 0;
    //                            var serializationMap = new BinaryReader(mapStream);
    //                            WriteAllTypesWithMap(writer, objType, obj, serializationMap);
    //                            isDone = true;
    //                        }
    //                    }
    //                }

    //                if (!isDone)
    //                    WriteAllTypes(writer, obj.GetType(), obj);
    //            }

    //            writer.Flush();
    //            //var eType = GetSerializaionType(type);
    //            //object  firstElement = null;
    //            //int count = 0;
    //            //Type gType = null;
    //            //check if it is worthwhile to create serialization mapStream
    //            //switch (eType)
    //            //{
    //            //    case EnumSerializeType.IsArray:
    //            //        {
    //            //            count = value.Length;
    //            //            if (count <= 0) return;
    //            //            gType = type.GenericTypeArguments[0];
    //            //            if (!IsRealValueType(gType) && gType != typeof(string) && gType != typeof(DateTime))
    //            //            {
    //            //                ifCreateMap = true;
    //            //                firstElement = value[0];
    //            //            }
    //            //            break;
    //            //        }
    //            //    case EnumSerializeType.IsList:
    //            //        {
    //            //            count = value.Count;
    //            //            if (count <= 0) return;
    //            //            gType = type.GenericTypeArguments[0];
    //            //            if (!IsRealValueType(gType) && gType != typeof(string) && gType != typeof(DateTime))
    //            //            {
    //            //                ifCreateMap = true;
    //            //                firstElement = value[0];
    //            //            }
    //            //            break;
    //            //        }
    //            //    case EnumSerializeType.IsDictionary:
    //            //        {
    //            //            count = value.Count;
    //            //            if (count <= 0) return;
    //            //            var kType = type.GenericTypeArguments[0];
    //            //            var vType = type.GenericTypeArguments[1];
    //            //            if ((!IsRealValueType(kType) && kType != typeof(string) && kType != typeof(DateTime)) ||
    //            //                (!IsRealValueType(vType) && vType != typeof(string) && vType != typeof(DateTime)))
    //            //            {
    //            //                ifCreateMap = true;
    //            //                gType = typeof(KeyValuePair<,>).MakeGenericType(new Type[] { kType, vType });
    //            //                var method = typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(gType);
    //            //                firstElement = method.Invoke(value, new object[] { value, 0 });
    //            //            }
    //            //            break;
    //            //        }
    //            //    case EnumSerializeType.IsEnumerable:
    //            //        {
    //            //            count = value.Count;
    //            //            if (count <= 0) return;
    //            //            gType = type.GenericTypeArguments[0];
    //            //            if (!IsRealValueType(gType) && gType != typeof(string) && gType != typeof(DateTime))
    //            //            {
    //            //                ifCreateMap = true;
    //            //                var method = typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(gType);
    //            //                firstElement = method.Invoke(value, new object[] { value, 0 });
    //            //            }
    //            //            break;
    //            //        }
    //            //    default:
    //            //        break;
    //            //}
    //        }
    //        stopwatch.Stop();

    //        if (isShowTime)
    //            Console.WriteLine("Serialize:   time = {0}, length = {1}", stopwatch.ElapsedMilliseconds, stream.Length);
    //    }
    //    static public dynamic DeSerializeObject(string filename, Type type, bool isShowTime = false)
    //    {
    //        dynamic obj = null;

    //        long length = 0;
    //        var stopwatch = new Stopwatch();
    //        stopwatch.Start();
    //        {
    //            using (var stream = ReadFileIntoMemoryStream(filename)) {
    //                obj = DeSerializeObject(stream, type);
    //                length = stream.Length;
    //            }
    //        }
    //        stopwatch.Stop();

    //        if (isShowTime)
    //            Console.WriteLine("Deserialize: time = {0}, length = {1}", stopwatch.ElapsedMilliseconds, length);

    //        return obj;
    //    }
    //    static public dynamic DeSerializeObject(MemoryStream stream, Type type, bool isShowTime = false)
    //    {
    //        dynamic obj = null;

    //        long length = 0;
    //        var stopwatch = new Stopwatch();
    //        stopwatch.Start();
    //        {
    //            var breader = new BinaryReader(stream);
    //            if (breader.ReadBoolean() == true) {
    //                obj = null;
    //                length = stream.Length;
    //            }
    //            else {
    //                obj = ReadAllTypes(breader, type);
    //                length = stream.Length;
    //            }
    //        }
    //        stopwatch.Stop();

    //        if (isShowTime)
    //            Console.WriteLine("Deserialize: time = {0}, length = {1}", stopwatch.ElapsedMilliseconds, length);

    //        return obj;
    //    }
    //    #endregion
    //    #region Compare two objects
    //    //static public bool CompareObjects(DataTable o1, T o2)
    //    static public bool CompareObjects<T>(T o1, T o2)
    //    {
    //        return CompareAllTypes(o1, o2, o1.GetType());
    //    }
    //    static private bool CompareAllTypes(dynamic o1, dynamic o2, Type type)
    //    {
    //        var typeCode = Type.GetTypeCode(type);
    //        switch (typeCode) {
    //            case TypeCode.Decimal:
    //            case TypeCode.Double:
    //            case TypeCode.Single:
    //            case TypeCode.Int64:
    //            case TypeCode.UInt64:
    //            case TypeCode.Int32:
    //            case TypeCode.UInt32:
    //            case TypeCode.Int16:
    //            case TypeCode.UInt16:
    //            case TypeCode.SByte:
    //            case TypeCode.Byte:
    //            case TypeCode.Boolean:
    //            case TypeCode.Char:
    //            case TypeCode.String:
    //                return o1.Equals(o2);
    //            case TypeCode.DateTime:
    //                return o1.ToOADate().Equals(o2.ToOADate());
    //            default:
    //                //var type1 = o1.GetType();
    //                //var type2 = o2.GetType();
    //                //if (type1 != type2 || type1 != type) return false;
    //                if (IsTypeNullable(type)) {
    //                    if (o1 == null && o2 == null)
    //                        return true;
    //                    if (o1 == null || o2 == null)
    //                        return false;
    //                }

    //                var etype = GetSerializaionType(type);
    //                switch (etype) {
    //                    case SerializeTypes.IsDataTable:
    //                        return CompareDataTable(o1, o2, type);
    //                    case SerializeTypes.IsArray:
    //                        return CompareArray(o1, o2, type);
    //                    case SerializeTypes.IsList:
    //                        return CompareList(o1, o2, type);
    //                    case SerializeTypes.IsDictionary:
    //                        return CompareDictionary(o1, o2, type);
    //                    case SerializeTypes.IsEnumerable:
    //                        return CompareEumerable(o1, o2, type);
    //                    case SerializeTypes.IsKeyValue:
    //                        return CompareKeyValue(o1, o2, type);
    //                    default:
    //                        if (type == typeof(Type))
    //                            return (o1.FullName == o2.FullName);
    //                        else
    //                            return CompareClass(o1, o2, type);

    //                        //if (IsHiPerComparable(type))
    //                        //    return CompareClass(o1, o2, type);
    //                        //else
    //                        //    throw new InvalidOperationException(string.Format("Type {0} is not supported", typeCode));
    //                }
    //        }
    //    }
    //    static private bool CompareClass(dynamic o1, dynamic o2, Type objType)
    //    {
    //        var properties = objType.GetProperties().Where(p => p.CanWrite).ToList();
    //        var fields = objType.GetFields().ToList();
    //        var hasHiPerfProperty = (properties != null && properties.Count > 0);
    //        var hasHiPerfField = (fields != null && fields.Count > 0);

    //        if (hasHiPerfProperty) {
    //            //Serialize members one by one
    //            foreach (var p in properties) {
    //                var value1 = p.GetValue(o1);
    //                var value2 = p.GetValue(o2);
    //                if (!CompareAllTypes(value1, value2, p.PropertyType))
    //                    return false;
    //            }
    //        }
    //        if (hasHiPerfField) {
    //            foreach (var f in fields) {
    //                var value1 = f.GetValue(o1);
    //                var value2 = f.GetValue(o2);
    //                if (!CompareAllTypes(value1, value2, f.FieldType))
    //                    return false;
    //            }
    //        }
    //        return true;
    //    }
    //    static private bool CompareKeyValue(dynamic o1, dynamic o2, Type type)
    //    {
    //        var kType = type.GenericTypeArguments[0];
    //        var vType = type.GenericTypeArguments[1];

    //        if (!CompareAllTypes(o1.Key, o2.Key, kType)) return false;
    //        if (!CompareAllTypes(o1.Value, o2.Value, vType)) return false;

    //        return true;
    //    }
    //    static private bool CompareDataTable(dynamic o1, dynamic o2, Type type)
    //    {
    //        var table1 = (DataTable)Convert.ChangeType(o1, typeof(DataTable));
    //        var table2 = (DataTable)Convert.ChangeType(o2, typeof(DataTable));

    //        if (table1.TableName != table2.TableName) return false;

    //        if (table1.Columns.Count != table2.Columns.Count || table1.Rows.Count != table2.Rows.Count) return false;
    //        if (table1.Columns.Count == 0) return true;

    //        var columnNames1 = table1.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    //        var columnNames2 = table1.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    //        if (!CompareAllTypes(columnNames1, columnNames2, columnNames1.GetType())) return false;

    //        var columnTypes1 = table1.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();
    //        var columnTypes2 = table2.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();
    //        if (!CompareAllTypes(columnTypes1, columnTypes2, columnTypes1.GetType())) return false;

    //        for (int i = 0; i < table1.Rows.Count; i++) {
    //            for (int j = 0; j < table1.Columns.Count; j++) {
    //                var v1 = table1.Rows[i][j];
    //                var v2 = table2.Rows[i][j];
    //                if (!CompareAllTypes(v1, v2, columnTypes1[j])) return false;
    //            }
    //        }
    //        return true;
    //    }
    //    static private bool CompareDictionary(dynamic o1, dynamic o2, Type type)
    //    {
    //        int count1 = ((IDictionary)o1).Count;
    //        int count2 = ((IDictionary)o2).Count;

    //        if (count1 != count2) return false;
    //        if (count1 == 0) return true;

    //        var kType = type.GenericTypeArguments[0];
    //        var vType = type.GenericTypeArguments[1];
    //        var gType = typeof(KeyValuePair<,>).MakeGenericType(new Type[] { kType, vType });
    //        var method = typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(gType);

    //        for (int i = 0; i < count1; i++) {
    //            var value1 = method.Invoke(o1, new object[] { o1, i });
    //            var value2 = method.Invoke(o1, new object[] { o2, i });
    //            if (!CompareAllTypes(value1, value2, gType)) return false;
    //        }
    //        return true;
    //    }
    //    static private bool CompareList(dynamic o1, dynamic o2, Type type)
    //    {
    //        int count1 = ((IList)o1).Count;
    //        int count2 = ((IList)o2).Count;

    //        if (count1 != count2) return false;
    //        if (count1 == 0) return true;

    //        var gType = type.GenericTypeArguments[0];
    //        for (int i = 0; i < count1; i++) {
    //            if (!CompareAllTypes(o1[i], o2[i], gType)) return false;
    //        }

    //        return true;
    //    }
    //    static private bool CompareArray(dynamic o1, dynamic o2, Type type)
    //    {
    //        int count1 = o1.Length;
    //        int count2 = o2.Length;

    //        if (count1 != count2) return false;
    //        if (count1 == 0) return true;

    //        //var gType = type.GenericTypeArguments[0];
    //        for (int i = 0; i < count1; i++) {
    //            if (!CompareAllTypes(o1[i], o2[i], o1[i].GetType())) return false;
    //        }
    //        return true;
    //    }
    //    static private bool CompareEumerable(dynamic o1, dynamic o2, Type type)
    //    {
    //        var gType = type.GenericTypeArguments[0];
    //        var listType = typeof(List<>).MakeGenericType(new Type[] { gType });
    //        var list1 = (IList)Activator.CreateInstance(listType);
    //        var list2 = (IList)Activator.CreateInstance(listType);

    //        int count1 = 0;
    //        foreach (var item in o1) {
    //            list1.Add(item);
    //            ++count1;
    //        }
    //        int count2 = 0;
    //        foreach (var item in o2) {
    //            list1.Add(item);
    //            ++count2;
    //        }

    //        if (count1 != count2) return false;
    //        if (count1 == 0) return true;

    //        for (int i = 0; i < count1; i++) {
    //            if (!CompareAllTypes(list1[i], list2[i], gType)) return false;
    //        }
    //        return true;
    //    }
    //    #endregion
    //}
}
