using System.IO;

namespace BinaryFileIOLibrary
{
    public static class BinaryFileIO
    {
        const string fileName = @"data\config.dat";

        public static void WriteData()
        {
            using (BinaryWriter writer = new (File.Open(fileName, FileMode.OpenOrCreate)))
            {
                writer.Write(@"sample data");
            }
        }

        public static void ReadData()
        {
            string data;

            if (File.Exists(fileName))
            {
                using (BinaryReader reader = new (File.Open(fileName, FileMode.Open)))
                {
                    data = reader.ReadString();
                }
            }
        }
    }
}
