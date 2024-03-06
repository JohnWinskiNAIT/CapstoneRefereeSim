using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] 
    GameObject puckObject;
    [SerializeField]
    float maxLeft, maxRight;
    [SerializeField]
    GameObject zoneParent;
    GameObject[] zones;
    Vector3[] zoneOffsets;

    // Start is called before the first frame update
    void Start()
    {
        SetupZones();
    }

    void SetupZones()
    {
        zones = new GameObject[zoneParent.transform.childCount];
        zoneOffsets = new Vector3[zones.Length];
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i] = zoneParent.transform.GetChild(i).gameObject;
            zoneOffsets[i] = zones[i].transform.position - puckObject.transform.position;
        }
    }

    void UpdateZonePositions()
    {
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].transform.position = new Vector3(0, 0, puckObject.transform.position.z + zoneOffsets[i].z);
            float scaleBuffer = zones[i].transform.localScale.z / 2;

            if (zones[i].transform.position.z + scaleBuffer > maxLeft)
            {
                zones[i].transform.position = new Vector3(0, 0, maxLeft - scaleBuffer);
            }
            if (zones[i].transform.position.z - scaleBuffer < maxRight)
            {
                zones[i].transform.position = new Vector3(0, 0, maxRight + scaleBuffer);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateZonePositions();
    }
}
