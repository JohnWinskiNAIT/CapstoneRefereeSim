using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField]
    PlayerControl tempPlayerControl;

    [SerializeField]
    Image inputStoreRadial;

    private void Update()
    {
        inputStoreRadial.fillAmount = tempPlayerControl.StoredActionStatus();
    }
}
