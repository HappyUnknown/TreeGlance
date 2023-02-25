using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGlance
{
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
        /// Writes paths (and subpaths) of directories included
        /// </summary>
        /// <param name="path">Paths, where we need to hold a search</param>
        /// <param name="recursive">Do you want subpaths to be included?</param>
        public void WriteSubpaths(string path, bool recursive = false)
        {
            var innerDirectories = Directory.GetDirectories(path).ToArray();
            foreach (string d in innerDirectories)
            {
                File.AppendAllText(dirPathsFile, $"{d}\n\n");
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
            var dirpaths = ReadDirPaths();
            foreach (string dp in dirpaths)
                AppendFilesData(dp);
        }
        public string[] ReadDirPaths()
        {
            return File.ReadAllLines(dirPathsFile);
        }
    }
}
