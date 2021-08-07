using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace CommonCSharpLibary.Facility
{
    using System.Drawing.Imaging;
    using System.Reflection.Emit;

    /// <summary>
    /// class for converting table, list of stock quotes into printable strings
    /// </summary>
    public class CFacility : PortableCSharpLib.Util.Facility
    {
        static CFacility() { PortableCSharpLib.General.CheckDateTime(); }

        public static DataTable RemoveRedundantRows(string columnName, DataTable table)
        {
            try
            {
                ////_sector_financial.AsEnumerable().Distinct();
                //List<string> distinct_symbol = new List<string>();
                ////_stock_financial.DefaultView.ToTable(true, "Symbol").AsEnumerable().Select(row => distinct_symbol.Add((string)row["Symbol"]));
                //_stock_financial.AsEnumerable().Distinct();
                ////_stock_financial = _stock_financial.AsEnumerable().Where(r => listSymbol.Contains(r.Field<string>("Symbol"))).CopyToDataTable();
                ////_stock_financial = _stock_financial.Select("SELECT DISTINCT Symbol").CopyToDataTable();
                //var vw = new DataView(_stock_financial);
                //vw.RowFilter = "SELECT DISTINCT Symbol";
                //var x = vw.ToTable(true, new string [] {"Symbol"});
                var listSymbol1 = (from r in table.AsEnumerable() select r[columnName]).ToList();               //get all symbols
                var symbols = listSymbol1.Distinct().ToList();    //get distinc symbol
                if (symbols.Count == listSymbol1.Count)
                    return table;

                var newtable = table.Clone();
                foreach (DataRow row in table.Rows){
                    if (symbols.Contains(row[columnName])) {
                        var newrow = newtable.NewRow();
                        newrow.ItemArray = (object[])row.ItemArray.Clone();
                        newtable.Rows.Add(newrow);
                        symbols.Remove(row[columnName]);
                    }
                }
                return newtable;
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap CaptureScreenWindow(Rectangle rc)
        {
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                    g.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            return bitmap;
        }

        public static TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        public static void CreateAutoImplementedProperty(
            TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                              propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
    }
}

