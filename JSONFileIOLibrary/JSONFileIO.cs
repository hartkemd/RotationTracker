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
        public static void Save<T>(this T model, string fileName)
        {
            string jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(fileName, jsonString);
        }

        public static T Load<T>(this T model, string fileName) where T : new()
        {
            T output = new();

            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                output = JsonSerializer.Deserialize<T>(jsonString);
            }
            return output;
        }
    }
}
