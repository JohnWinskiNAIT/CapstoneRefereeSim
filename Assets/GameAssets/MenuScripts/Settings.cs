using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class Settings:MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioMixer audioMixer;

    public void ChangeMasterVolume()
    {
         audioMixer.SetFloat("Master", Mathf.Log10(volumeSlider.value)*20);
    }
    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volumeSlider.value) * 20);
    }
    public void ChangeAmbientVolume()
    {
        audioMixer.SetFloat("Ambient", Mathf.Log10(volumeSlider.value) * 20);
    }
}
