using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SaveManager
{
    public static void SaveData(string path, ref SettingsData mySettings)
    {
        if(path != null)
        {
            Stream stream = File.Open(path, FileMode.Create);
            XmlSerializer serializer = new(typeof(SettingsData));
            serializer.Serialize(stream, mySettings);
            stream.Close();
        }
    }
    public static void LoadData(string path, ref SettingsData mySettings)
    {
        if (File.Exists(path))
        {
            Stream stream = File.Open(path, FileMode.Open);
            XmlSerializer serializer = new(typeof(SettingsData));
            mySettings = (SettingsData)serializer.Deserialize(stream);
            stream.Close();
        }
    }
}
