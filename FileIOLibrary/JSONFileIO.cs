using System.IO;
using System.Text.Json;

namespace JSONFileIOLibrary
{
    public static class JSONFileIO
    {
        public static void SaveToJSON<T>(this T model, string filePath, string fileName)
        {
            DirectoryHelper.CreateDirectoryIfDoesNotExist(filePath);

            string jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText($"{filePath}{fileName}", jsonString);
        }

        public static T LoadFromJSON<T>(this T model, string fullFilePath) where T : new()
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
