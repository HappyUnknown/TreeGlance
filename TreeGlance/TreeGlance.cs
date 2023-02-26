﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGlance
{
    class ExplorerInfo 
    {
        public double Size { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime WroteOn { get; set; }
        public DateTime AccessedOn { get; set; }
        public string Path { get; set; }
    }
    class LineDirectoryInfo:ExplorerInfo
    {
        public LineDirectoryInfo(string path)
        {
            Size = 0;
            var files =  Directory.GetFiles(path);
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
            CreatedOn = file.CreationTime;
            WroteOn = file.LastWriteTime;
            AccessedOn = file.LastAccessTime;
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
                File.AppendAllText(dirPathsFile, $"{new LineDirectoryInfo(d).Serialize()}\n");
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
        /// <summary>
        /// Recreates filepaths' file, reads LineDirectoryInfo-jsons; then, appends file data for each of the paths
        /// </summary>
        public void WriteFilesData()
        {
            File.Create(filePathsFile).Close();
            var ldis = GetLDIs(RemoveEmpties(ReadDirJSONs()));
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
