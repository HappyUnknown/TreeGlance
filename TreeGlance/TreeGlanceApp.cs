using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGlance
{
    class TreeGlanceApp
    {
        string pathsFile = "paths.txt";

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
                File.AppendAllText(pathsFile, $"{d}\n");
                if (recursive)
                    WriteSubpaths(d, recursive);
            }
        }
    }
}
