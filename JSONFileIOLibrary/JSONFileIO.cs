using RotationLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONFileIOLibrary
{
    public static class JSONFileIO
    {
        public static void Save<T>(this T model, string filePath, string fileName)
        {
            DirectoryHelper.CreateDirectoryIfDoesNotExist(filePath);

            string jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText($"{filePath}{fileName}", jsonString);
        }

        public static T Load<T>(this T model, string fullFilePath) where T : new()
        {
            T output = new();

            if (File.Exists(fullFilePath))
            {
                string jsonString = File.ReadAllText(fullFilePath);
                output = JsonSerializer.Deserialize<T>(jsonString);
            }
            return output;
        }
    }
}
