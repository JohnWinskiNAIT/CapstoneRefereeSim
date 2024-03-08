using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Settings:MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] float volume = 0;

    public void ChangeVolume()
    {
        volume = volumeSlider.value;
    }
}
