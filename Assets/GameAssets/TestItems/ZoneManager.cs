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
        
    }

    void SetupZones()
    {
        zones = new GameObject[zoneParent.transform.childCount];
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i] = zoneParent.transform.GetChild(i).gameObject;
            zoneOffsets[i] = zones[i].transform.position - puckObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
