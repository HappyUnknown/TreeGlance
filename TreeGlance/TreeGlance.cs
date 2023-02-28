using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGlance
{
    static class StringExtension
    {
        public static LineDirectoryInfo DeserializeLineDirInfo(this string ldiStr)
        {
            return JsonConvert.DeserializeObject<LineDirectoryInfo>(ldiStr);
        }
        public static LineFileInfo DeserializeLineFileInfo(this string ldiStr)
        {
            return JsonConvert.DeserializeObject<LineFileInfo>(ldiStr);
        }
    }
    static class LineDirectoryExtensions
    {
        public static LineDirectoryInfo[] ToLineDirectories(this string[] lineDirStrs)
        {
            LineDirectoryInfo[] lineDirs = new LineDirectoryInfo[lineDirStrs.Length];
            for (int i = 0; i < lineDirStrs.Length; i++)
                lineDirs[i] = lineDirStrs[i].DeserializeLineDirInfo();
            return lineDirs;
        }
    }
    class ExplorerInfo
    {
        public double Size { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime WroteOn { get; set; }
        public DateTime AccessedOn { get; set; }
        public string Path { get; set; }
    }
    class LineDirectoryInfo : ExplorerInfo
    {
        public LineDirectoryInfo(string path)
        {
            Size = 0;
            var files = Directory.GetFiles(path);
            foreach (var fpath in files)
                Size += new FileInfo(fpath).GetFileSize();
            Path = path;
            DirectoryInfo directory = new DirectoryInfo(Path);
            CreatedOn = directory.CreationTime;
            WroteOn = directory.LastWriteTime;
            AccessedOn = directory.LastAccessTime;
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    class LineFileInfo : ExplorerInfo
    {
        public LineFileInfo(string path)
        {
            Path = path;
            FileInfo file = new FileInfo(Path);
            Size = file.GetFileSize();
            CreatedOn = file.CreationTime;
            WroteOn = file.LastWriteTime;
            AccessedOn = file.LastAccessTime;
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    static class FileInfoExtension
    {
        /// <summary>
        /// Extension responsible for translating byte size to megabytes
        /// </summary>
        /// <param name="file"></param>
        /// <param name="inMB"></param>
        /// <returns></returns>
        public static double GetFileSize(this FileInfo file, bool inMB = true)
        {
            const int BYTES_IN_MB = 1048576;
            double size = file.Length;
            if (inMB) size /= BYTES_IN_MB;
            return size;
        }
    }
    class TreeGlanceApp
    {
        public string dirPathsFile = "dirpaths.txt";
        public string filePathsFile = "filepaths.txt";
        /// <summary>
        /// Picks only non-empty lines
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private string[] RemoveEmpties(string[] lines)
        {
            List<string> noEmpties = new List<string>();
            foreach (string l in lines)
                if (l.Trim(' ').Length > 0)
                    noEmpties.Add(l);
            return noEmpties.ToArray();
        }
        /// <summary>
        /// Converts paths read to respective LineDirectoryInfo-s
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private LineDirectoryInfo[] GetLDIs(string[] paths)
        {
            List<LineDirectoryInfo> noEmpties = new List<LineDirectoryInfo>();
            foreach (string p in paths)
                if (p.Trim(' ').Length > 0)
                    noEmpties.Add(new LineDirectoryInfo(p));
            return noEmpties.ToArray();
        }
        /// <summary>
        /// Launches WriteSubpath() recursive function, but recreates destination textfile first
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        public void WriteSubpathsSafe(string path, ref System.Windows.Controls.DataGrid grid, bool recursive = false)
        {
            List<LineDirectoryInfo> lineDirInfo = new List<LineDirectoryInfo>();
            File.Create(dirPathsFile).Close();
            WriteSubpaths(path, ref lineDirInfo, recursive);
            grid.ItemsSource = lineDirInfo;
        }
        /// <summary>
        /// Writes paths (and subpaths) of directories included
        /// </summary>
        /// <param name="path">Paths, where we need to hold a search</param>
        /// <param name="recursive">Do you want subpaths to be included?</param>
        public void WriteSubpaths(string path, ref List<LineDirectoryInfo> lineDirInfo, bool recursive = false)
        {
            var innerDirectories = Directory.GetDirectories(path);
            foreach (string d in innerDirectories)
            {
                LineDirectoryInfo theDirInfo = new LineDirectoryInfo(d);
                lineDirInfo.Add(theDirInfo);
                File.AppendAllText(dirPathsFile, $"{theDirInfo.Serialize()}\n");
                if (recursive)
                    WriteSubpaths(d, ref lineDirInfo, recursive);
            }
        }
        /// <summary>
        /// 5 lines per file
        /// </summary>
        /// <param name="dirpath"></param>
        /// <param name="recursive"></param>
        public void AppendFilesData(string dirpath)
        {
            var filePaths = Directory.GetFiles(dirpath).ToArray();
            foreach (string f in filePaths)
                File.AppendAllText(filePathsFile, $"{new LineFileInfo(f).Serialize()}\n");
        }
        /// <summary>
        /// Recreates filepaths' file, reads LineDirectoryInfo-jsons; then, appends file data for each of the paths
        /// </summary>
        public void WriteFilesData()
        {
            File.Create(filePathsFile).Close();
            var ldis = RemoveEmpties(ReadDirJSONs()).ToLineDirectories();
            foreach (LineDirectoryInfo ldi in ldis)
                AppendFilesData(ldi.Path);
        }
        /// <summary>
        /// Reads raw directory paths' file with the file path specified 
        /// </summary>
        /// <returns></returns>
        public string[] ReadDirJSONs()
        {
            return File.ReadAllLines(dirPathsFile);
        }
    }
}
