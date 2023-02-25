using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGlance
{
    class TreeGlance
    {
        string pathsFile = "paths.txt";
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
