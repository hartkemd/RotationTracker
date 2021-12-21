using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONFileIOLibrary
{
    public static class DirectoryHelper
    {
        public static void CreateDirectoryIfDoesNotExist(string filePath)
        {
            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
        }
    }
}
