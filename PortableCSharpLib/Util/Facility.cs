using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

namespace PortableCSharpLib.Util
{
    /// <summary>
    /// class for converting table, list of stock quotes into printable strings
    /// </summary>
    public class Facility : General
    {
        static Facility() { PortableCSharpLib.General.CheckDateTime(); }

        /// <summary>
        /// convert datatable to string for print
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string ConvertDataTableToString(DataTable dataTable)
        {
            var output = new StringBuilder();

            var columnsWidths = new int[dataTable.Columns.Count];

            // Get column widths
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    //var length = row[i].ToString().Length;
                    var length = string.Format("{0:0.##}", row[i]).Length;
                    if (columnsWidths[i] < length)
                        columnsWidths[i] = length;
                }
            }

            // Get Column Titles
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var length = dataTable.Columns[i].ColumnName.Length;
                if (columnsWidths[i] < length)
                    columnsWidths[i] = length;
            }

            // Write Column titles
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var text = dataTable.Columns[i].ColumnName;
                output.Append("|" + PadCenter(text, columnsWidths[i] + 2));
            }
            output.Append("|\n" + new string('=', output.Length) + "\n");

            // Write Rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    //var text = row[i].ToString();
                    var text = string.Format("{0:0.###}", row[i]);
                    output.Append("|" + PadCenter(text, columnsWidths[i] + 2));
                }
                output.Append("|\n");
            }
            return output.ToString();
        }
        public static void ConsoleClearCurrentLine(int left = 0)
        {
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write(new String(' ', Console.BufferWidth - left));
            Console.SetCursorPosition(left, Console.CursorTop - 1);
        }
        public static void ConsoleClearLines(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new String(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
        }

        #region debug/trace information
        // function to display its name
        public static List<string> GetStackTrace()
        {
            var stackNames = new List<string>();
            StackTrace stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
                stackNames.Add(stackTrace.GetFrame(i).GetMethod().Name);
            return stackNames;

            //StackFrame stackFrame = new StackFrame();
            //MethodBase methodBase = stackFrame.GetMethod();
            //return methodBase.Name;
        }
        // function to display its name
        public static string WhatsMyName()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            return methodBase.Name;
            //StackFrame stackFrame = new StackFrame();
            //MethodBase methodBase = stackFrame.GetMethod();
            //return methodBase.Name;
        }
        // Function to display parent function
        public static string WhoCalledMe()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(2);
            MethodBase methodBase = stackFrame.GetMethod();
            return methodBase.Name;
        }
        public static string GetCallerInfo([CallerMemberName] string callerName = null,
                                           [CallerFilePath] string callerFilePath = null,
                                           [CallerLineNumber] int callerLine = -1)
        {
            //Console.WriteLine("Caller Name: {0}", callerName);
            //Console.WriteLine("Caller FilePath: {0}", callerFilePath);
            //Console.WriteLine("Caller Line number: {0}", callerLine);

            return string.Format("{0}@{1}:{2}", callerName, Path.GetFileName(callerFilePath), callerLine);
        }
        #endregion

        public static bool TestUrlReachable(string url, int timeout = 15000)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = timeout;
            request.Method = "HEAD";
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
