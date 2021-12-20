using System.IO;

namespace BinaryFileIOLibrary
{
    public static class BinaryFileIO
    {
        const string fileName = @"data\config.dat";

        public static void WriteData()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
            {
                writer.Write(@"sample data");
            }
        }

        public static void ReadData()
        {
            string data;

            if (File.Exists(fileName))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    data = reader.ReadString();
                }
            }
        }
    }
}
