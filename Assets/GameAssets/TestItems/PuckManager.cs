using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckManager : MonoBehaviour
{
    public GameObject Owner { get; private set; }

    public void ChangePosession(GameObject posession)
    {
        if (Owner != null)
        {
            Owner.GetComponent<ZoneAIController>().LosePuck();
        }
        Owner = posession;
    }

    public void LoseOwner()
    {
        Owner = null;
    }
}
