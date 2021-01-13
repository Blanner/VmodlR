using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class XMLSerializer
{

    public static void Serialize(object item, string path)
    {
        //save file
        XmlSerializer serializer = new XmlSerializer(item.GetType());
#if UNITY_EDITOR
        StreamWriter writer = new StreamWriter(path);
#else
    #if UNITY_ANDROID
        StreamWriter writer = new StreamWriter($"/mnt/sdcard/{path}");
    #else
        StreamWriter writer = new StreamWriter(path);
    #endif
#endif

        serializer.Serialize(writer.BaseStream, item);
        writer.Close();

        string[] pathparts = path.Split('/');
        string fileName = pathparts[pathparts.Length - 1];

        //upload to google drive
#if UNITY_EDITOR
        var file = new UnityGoogleDrive.Data.File { Name = fileName, Content = File.ReadAllBytes(Application.dataPath.Replace("Assets", path)) };
        UnityGoogleDrive.GoogleDriveFiles.Create(file).Send();
#else
    #if UNITY_ANDROID
        //var file = new UnityGoogleDrive.Data.File { Name = fileName, Content = File.ReadAllBytes($"/mnt/sdcard/{path}") };
    #else
        var file = new UnityGoogleDrive.Data.File { Name = fileName, Content = File.ReadAllBytes(Application.dataPath.Replace("Assets", path)) };
        UnityGoogleDrive.GoogleDriveFiles.Create(file).Send();
    #endif
#endif

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
