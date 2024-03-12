using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System.Xml.Serialization;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider ambientSlider;
    [SerializeField] AudioMixer audioMixer;
    public VolumeData myVolumes;
    string filePath;
    string rootPath = "SaveData\\";
    private void Start()
    {
        myVolumes = new VolumeData();
        filePath = rootPath + "volumeData\\volumes.dat";
        LoadVolumes();
        masterSlider.value = myVolumes.masterVolume;
        SFXSlider.value = myVolumes.SFXvolume;
        ambientSlider.value = myVolumes.ambientVolume;
    }
    public void ChangeMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        SaveVolume();
    }
    public void ChangeSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        SaveVolume();
    }
    public void ChangeAmbientVolume()
    {
        float volume = ambientSlider.value;
        audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
        SaveVolume();
    }
    public void SaveVolume()
    {
        myVolumes.masterVolume = masterSlider.value;
        myVolumes.SFXvolume = SFXSlider.value;
        myVolumes.ambientVolume = ambientSlider.value;
        if (!Directory.Exists("SaveData\\volumeData"))
        {
            Directory.CreateDirectory("SaveData\\volumeData");
        }
        SaveManager.SaveData(filePath, ref myVolumes);
    }
    public void LoadVolumes()
    {
        SaveManager.LoadData(filePath, ref myVolumes);
    }
}
