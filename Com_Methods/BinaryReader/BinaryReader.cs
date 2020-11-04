using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Com_Methods
{
    public static class BinaryReader
    {
        public static void SaveObject (object obj, string path)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, obj);
            }
        }
        public static object LoadObject (string path)
        {
            object obj = null;
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = File.OpenRead(path))
            {
                obj = binFormat.Deserialize(fStream);
            }
            return obj;
        }
    }
}
