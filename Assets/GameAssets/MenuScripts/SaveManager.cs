using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SaveManager
{
    public static void SaveData(string path, ref VolumeData myVolumes)
    {
        if(path != null)
        {
            Stream stream = File.Open(path, FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(VolumeData));
            serializer.Serialize(stream, myVolumes);
            stream.Close();
        }
    }
    public static void LoadData(string path, ref VolumeData myVolumes)
    {
        if (File.Exists(path))
        {
            Stream stream = File.Open(path, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(VolumeData));
            myVolumes = (VolumeData)serializer.Deserialize(stream);
            stream.Close();
        }
    }
}
