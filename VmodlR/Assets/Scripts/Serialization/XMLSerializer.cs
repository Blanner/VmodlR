using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class XMLSerializer
{

    public static void Serialize(object item, string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
#if UNITY_ANDROID
        StreamWriter writer = new StreamWriter($"/mnt/sdcard/{path}");
#else
        StreamWriter writer = new StreamWriter(path);
#endif
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }

    public static T Deserialize<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }
}
