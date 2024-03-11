using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class Settings : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider ambientSlider;
    [SerializeField] AudioMixer audioMixer;
    public float test;
    private void Start()
    {
        if(PlayerPrefs.HasKey("MasterVolume"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        if (PlayerPrefs.HasKey("AmbientVolume"))
        {
            ambientSlider.value = PlayerPrefs.GetFloat("AmbientVolume");
        }
    }
    public void ChangeMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
    public void ChangeSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    public void ChangeAmbientVolume()
    {
        float volume = ambientSlider.value;
        audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("AmbientVolume", volume);
        PlayerPrefs.Save();
    }

}
