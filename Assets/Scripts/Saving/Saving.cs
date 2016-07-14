using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace SaveManagement
{
    public static class Saving
    {

        public static void Save<Class>(string path, Class objectToSerialise)
        {
            path = Application.persistentDataPath + path;
            FileStream file = File.Create(path);

            XmlSerializer xs = new XmlSerializer(typeof(Class));

            xs.Serialize(file, objectToSerialise);
            file.Close();
            Debug.Log("Saved to " + path);
        }

        private static BinaryFormatter CreateBinaryFormatter()
        {
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();

            Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
            ss.AddSurrogate(typeof(Vector3),
                            new StreamingContext(StreamingContextStates.All),
                            v3ss);

            // 5. Have the formatter use our surrogate selector
            bf.SurrogateSelector = ss;
            return new BinaryFormatter();
        }

        public static Class Load<Class>(string path)
        {
            path = Application.persistentDataPath + path;
            if (File.Exists(path))
            {
                FileStream file = File.Open(path, FileMode.Open);

                XmlSerializer xs = new XmlSerializer(typeof(Class));
                Class logs = (Class)xs.Deserialize(file);
                file.Close();
                return logs;
            }
            Debug.LogWarning("File doesn't exist");
            return default(Class);
        }

    }
}

