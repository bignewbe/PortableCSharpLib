using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


namespace PortableCSharpLib.Facility
{
    public class FileOperation
    {
        static FileOperation() { PortableCSharpLib.General.CheckDateTime(); }

        static public List<String> FindAllFiles(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (var f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(FindAllFiles(d));
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return files;
        }

        static public List<string> FindFilesContainingStr(string folder, string str)
        {
            if (!Directory.Exists(folder)) return null;

            var files = FindAllFiles(folder);
            if (str == "*") return files;

            if (files != null && files.Count > 0)
                return files.FindAll(f => f.Contains(str));
            else
                return null;
        }

        static public void RenameFiles()
        {
            //var fn = @"1426_0629235959_1427_0704235959_EUR.USD_5s.rat";

            var path = @"F:\Code\IBTrader\Data\EUR.USD";
            var dirInfo = new DirectoryInfo(path);
            var pathern1 = @"(?<=_)(\d{10})(?=_\d)";
            var pathern2 = @"(?<=_)(\d{10})(?=_EUR)";
            var pathern3 = @"(?<=_)(\d{4})(?=_\d+_EUR)";
            var pathern4 = @"(?<=\d{10}_\d{4}_\d{10}?)(.*?)$";
            foreach (var f in dirInfo.GetFiles())
            {
                var fn = f.Name;
                var sDate = Regex.Match(fn, pathern1);
                var eDate = Regex.Match(fn, pathern2);
                var eWeek = Regex.Match(fn, pathern3);
                var ext = Regex.Match(fn, pathern4);
                if (sDate.Success && eDate.Success && eWeek.Success)
                {
                    var startDate = sDate.Value.Substring(0, 4);
                    var endDate = eDate.Value.Substring(0, 4);
                    var endWeek = eWeek.Value;

                    //var oldFile = Path.Combine(path, fn);
                    var oldFile = f.FullName;
                    var newFile = endWeek + "_" + startDate + "_" + endDate + ext;
                    newFile = Path.Combine(path, newFile);

                    Console.WriteLine(oldFile + " => " + newFile);
                    File.Move(oldFile, newFile);
                }
            }
        }
    }
}
