using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealOnlyBttns : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        Button[] Button = FindObjectsOfType<Button>();
        Slider[] Slider = FindObjectsOfType<Slider>();
        Toggle[] Toggle = FindObjectsOfType<Toggle>();
        

        //for (int i = 0; i < Button.Length; i++)
        //{
        //    Button[i].gameObject.name = "Button";
        //}

        for (int i = 0; i < Slider.Length; i++)
        {
            Slider[i].gameObject.name = "Slider";
        }

        for (int i = 0; i < Toggle.Length; i++)
        {
            Toggle[i].gameObject.name = "Toggle";
        }
    }
}
