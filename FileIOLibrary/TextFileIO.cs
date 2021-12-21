using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileIOLibrary
{
    public static class TextFileIO
    {
        public static void WriteListToFile(string filePath, List<string> listToWrite)
        {
            File.WriteAllLines(filePath, listToWrite);
        }

        public static List<string> ReadListFromFile(string filePath)
        {
            List<string> output = new();

            output = File.ReadAllLines(filePath).ToList();

            return output;
        }
    }
}
