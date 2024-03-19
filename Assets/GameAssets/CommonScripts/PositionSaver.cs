using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class PositionSaver
{
    public static void SavePositionData(string path, ref PositionData positionData)
    {
        Stream stream = File.Open(path, FileMode.Create);
        XmlSerializer serializer = new XmlSerializer(typeof(PositionData));
        serializer.Serialize(stream, positionData);
        stream.Close();
    }

    public static void LoadPlayerData(string path, ref PositionData positionData)
    {
        if (File.Exists(path))
        {
            Stream stream = File.Open(path, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(PositionData));
            positionData = (PositionData)serializer.Deserialize(stream);
            stream.Close();
        }
    }
}
