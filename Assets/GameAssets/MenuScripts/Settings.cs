using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
static class Settings
{
    [SerializeField] static Slider volumeSlider;

    public static void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
