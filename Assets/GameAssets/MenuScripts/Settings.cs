using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System.Xml.Serialization;

public class Settings : MonoBehaviour
{
    public Slider masterSlider;
    public Slider SFXSlider;
    public Slider ambientSlider;
    [SerializeField] AudioMixer audioMixer;
    public GameObject penaltyUIContainer;
    Toggle[] myPenaltyToggles;
    public static SettingsData mySettings;
    string filePath;
    string rootPath = "SaveData\\";
    private void Start()
    {
        mySettings = new SettingsData();
        myPenaltyToggles = penaltyUIContainer.GetComponentsInChildren<Toggle>();
        mySettings.penalties = new PenaltyData[myPenaltyToggles.Length];
        for (int i = 0; i < mySettings.penalties.Length; i++)
        {
            mySettings.penalties[i].PenaltyName = myPenaltyToggles[i].transform.parent.gameObject.name;
        }
        filePath = rootPath + "settingsData\\settings.dat";
        LoadSettings();
        masterSlider.value = mySettings.masterVolume;
        SFXSlider.value = mySettings.SFXvolume;
        ambientSlider.value = mySettings.ambientVolume;
        if(mySettings.penalties.Length != 0)
        {
            for (int i = 0; i < myPenaltyToggles.Length; i++)
            {
                myPenaltyToggles[i].isOn = mySettings.penalties[i].isEnabled;
            }
        }
    }
    public void TogglePenalty(int index)
    {
        mySettings.penalties[index].isEnabled = myPenaltyToggles[index].isOn;
        SaveSettings();
    }
    public void ChangeMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        SaveSettings();
    }
    public void ChangeSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        SaveSettings();
    }
    public void ChangeAmbientVolume()
    {
        float volume = ambientSlider.value;
        audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
        SaveSettings();
    }
    public void SaveSettings()
    {
        mySettings.masterVolume = masterSlider.value;
        mySettings.SFXvolume = SFXSlider.value;
        mySettings.ambientVolume = ambientSlider.value;
        if (!Directory.Exists("SaveData\\settingsData"))
        {
            Directory.CreateDirectory("SaveData\\settingsData");
        }
        SaveManager.SaveData(filePath, ref mySettings);
    }
    public void LoadSettings()
    {
        SaveManager.LoadData(filePath, ref mySettings);
    }
}
