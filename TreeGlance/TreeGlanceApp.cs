using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TreeGlance
{
    class LineDirectoryInfo
    {
        public string Path { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime WroteOn { get; set; }
        public DateTime AccessedOn { get; set; }
        public LineDirectoryInfo(string path)
        {
            Path = path;
            DirectoryInfo directory = new DirectoryInfo(Path);
            CreatedOn = directory.CreationTime;
            WroteOn = directory.LastWriteTime;
            AccessedOn = directory.LastAccessTime;
        }
    }
    static class FileInfoExtension
    {
        public static double GetFileSize(this FileInfo file, bool inMB = true)
        {
            double size = file.Length;
            if (inMB) size /= 1048576;
            return size;
        }
    }
    class TreeGlanceApp
    {
        public string dirPathsFile = "dirpaths.txt";
        public string filePathsFile = "filepaths.txt";
        private string[] RemoveEmpties(string[] lines)
        {
            List<string> noEmpties = new List<string>();
            foreach (string l in lines)
                if (l.Trim(' ').Length > 0)
                    noEmpties.Add(l);
            return noEmpties.ToArray();
        }
        private LineDirectoryInfo[] GetLDIs(string[] paths)
        {
            List<LineDirectoryInfo> noEmpties = new List<LineDirectoryInfo>();
            foreach (string p in paths)
                if (p.Trim(' ').Length > 0)
                    noEmpties.Add(new LineDirectoryInfo(p));
            return noEmpties.ToArray();
        }
        public void WriteSubpathsSafe(string path, bool recursive = false)
        {
            File.Create(dirPathsFile).Close();
            WriteSubpaths(path, recursive);
        }
        /// <summary>
        /// Writes paths (and subpaths) of directories included
        /// </summary>
        /// <param name="path">Paths, where we need to hold a search</param>
        /// <param name="recursive">Do you want subpaths to be included?</param>
        public void WriteSubpaths(string path, bool recursive = false)
        {
            var innerDirectories = Directory.GetDirectories(path);
            foreach (string d in innerDirectories)
            {
                File.AppendAllText(dirPathsFile, $"{new LineDirectoryInfo(d).Path}\n");
                if (recursive)
                    WriteSubpaths(d, recursive);
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
                File.AppendAllText(filePathsFile, $"{f}\n{new FileInfo(f).GetFileSize()}\n{new FileInfo(f).CreationTime}\n{new FileInfo(f).LastWriteTime}\n{new FileInfo(f).LastAccessTime}\n\n");
        }
        public void WriteFilesData()
        {
            File.Create(filePathsFile).Close();
            var ldis = GetLDIs(RemoveEmpties(ReadDirJSONs()));
            foreach (LineDirectoryInfo ldi in ldis)
                AppendFilesData(ldi.Path);
        }
        public string[] ReadDirJSONs()
        {
            return File.ReadAllLines(dirPathsFile);
        }
    }
}
