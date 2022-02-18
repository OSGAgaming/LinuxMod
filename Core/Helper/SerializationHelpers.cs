using Microsoft.Xna.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace LinuxMod.Core
{
    public static partial class LinuxTechTips
    {
        public static void SerializeToXML<T>(T objects, string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));

            TextWriter txtWriter = new StreamWriter(fileName);

            xs.Serialize(txtWriter, objects);

            txtWriter.Close();
        }


        public static void Serialize(object t, string path)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, t);
            }
        }
        public static T Deserialize<T>(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                return (T)bformatter.Deserialize(stream);
            }
        }
        public static void SerializeToXMLMDA<T>(T[,] objects, string fileName)
        {
            T[] To1DArray(T[,] input)
            {
                int size = input.Length;
                T[] result = new T[size];

                int write = 0;
                for (int i = 0; i <= input.GetUpperBound(0); i++)
                {
                    for (int z = 0; z <= input.GetUpperBound(1); z++)
                    {
                        result[write++] = input[i, z];
                    }
                }
                return result;
            }
            T[] flattenedArray = To1DArray(objects);
            XmlSerializer xs = new XmlSerializer(typeof(T[]));

            TextWriter txtWriter = new StreamWriter(fileName);

            xs.Serialize(txtWriter, flattenedArray);

            txtWriter.Close();
        }


        public static T DeserializeToObject<T>(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(fileName))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }
}
