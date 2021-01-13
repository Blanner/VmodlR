using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

public class XMLSerializer
{
    private const string fileEnding = ".xme";

    public static bool Serialize(object item, string path)
    {
        if (!path.EndsWith(fileEnding))
        {
            path += fileEnding;
        }

        try
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
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError("\nError in XML Serializer");
            Debug.LogError("\n" + e.ToString());
            Debug.LogError("\n" + e.StackTrace);
            return false;
        }

    }

    public static bool FileExists(string path)
    {
        if(!path.EndsWith(fileEnding))
        {
            path += fileEnding;
        }

#if UNITY_EDITOR
        return File.Exists(Application.dataPath.Replace("Assets", path));
#else
    #if UNITY_ANDROID
        return File.Exists($"/mnt/sdcard/{path}");
    #else
        return File.Exists(Application.dataPath.Replace("Assets", path));
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
