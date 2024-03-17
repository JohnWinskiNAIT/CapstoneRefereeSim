using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Reflection;
using TMPro;

public class Settings : MonoBehaviour
{
    public Slider masterSlider;
    public Slider SFXSlider;
    public Slider ambientSlider;
    [SerializeField] AudioMixer audioMixer;
    public GameObject penaltyUIContainer;
    public GameObject startingPosContainer;
    public TMP_InputField scenarioField;
    public TMP_InputField screenModeText;
    public Image controllerImage;
    public Image keyBoardImage;
    Toggle[] myPenaltyToggles;
    Toggle[] myStartingPosToggles;
    int scenarios;
    int screenMode;
    public int keyLayout;
    bool mute;
    public SettingsData mySettings;
    string filePath;
    string rootPath = "SaveData\\";
    private void Start()
    {
        keyLayout = 1;
        mySettings = new SettingsData();
        myPenaltyToggles = penaltyUIContainer.GetComponentsInChildren<Toggle>();
        mySettings.penalties = new PenaltyData[myPenaltyToggles.Length];
        myStartingPosToggles = startingPosContainer.GetComponentsInChildren<Toggle>();
        mySettings.startingPos = new StartingPosData[myStartingPosToggles.Length];
        for (int i = 0; i < mySettings.penalties.Length; i++)
        {
            mySettings.penalties[i].PenaltyName = myPenaltyToggles[i].transform.parent.gameObject.name;
        }
        for (int i = 0; i < myStartingPosToggles.Length; i++)
        {
            mySettings.startingPos[i].posName = myStartingPosToggles[i].transform.parent.gameObject.name;
        }
        filePath = rootPath + "settingsData\\settings.dat";
        if (Directory.Exists("SaveData\\settingsData"))
        {
            LoadSettings();
            masterSlider.value = mySettings.masterVolume;
            SFXSlider.value = mySettings.SFXvolume;
            ambientSlider.value = mySettings.ambientVolume;
            if (mySettings.penalties.Length != 0)
            {
                for (int i = 0; i < myPenaltyToggles.Length; i++)
                {
                    myPenaltyToggles[i].isOn = mySettings.penalties[i].isEnabled;
                }
            }
            if (mySettings.startingPos.Length != 0)
            {
                for (int i = 0; i < myStartingPosToggles.Length; i++)
                {
                    myStartingPosToggles[i].isOn = mySettings.startingPos[i].isEnabled;
                }
            }
            if (mySettings.scenarios != 0)
            {
                scenarios = mySettings.scenarios;
                scenarioField.text = scenarios.ToString();
            }
            if (mySettings.screenMode != 0)
            {
                screenMode = mySettings.screenMode;
                if (mySettings.screenMode == 0)
                {
                    screenModeText.text = "Full Screen";
                    Screen.fullScreen = true;
                }
                else
                {
                    screenModeText.text = "Windowed";
                    Screen.fullScreen = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < mySettings.penalties.Length; i++)
            {
                mySettings.penalties[i].isEnabled = myPenaltyToggles[i].isOn;
            }
            for (int i = 0; i < myStartingPosToggles.Length; i++)
            {
                mySettings.startingPos[i].isEnabled = myStartingPosToggles[i].isOn;
            }
        }
    }
    public void TogglePenalty(int index)
    {
        mySettings.penalties[index].isEnabled = myPenaltyToggles[index].isOn;
        SaveSettings();
    }
    public void Mute()
    {

        mute = !mute;
        if (mute)
        {
            if (masterSlider.value > 0.0001f)
            {
                mySettings.lastMasterVolume = masterSlider.value;
            }
            masterSlider.value = 0.0001f;
            ChangeMasterVolume();
            if (SFXSlider.value > 0.0001f)
            {
                mySettings.lastSFXvolume = SFXSlider.value;
            }
            SFXSlider.value = 0.0001f;
            ChangeSFXVolume();
            if (ambientSlider.value > 0.0001f)
            {
                mySettings.lastAmbientVolume = ambientSlider.value;
            }
            ambientSlider.value = 0.0001f;
            ChangeAmbientVolume();
            SaveSettings();
        }
        else
        {
            masterSlider.value = mySettings.lastMasterVolume;
            ChangeMasterVolume();
            SFXSlider.value = mySettings.lastSFXvolume;
            ChangeSFXVolume();
            ambientSlider.value = mySettings.lastAmbientVolume;
            ChangeAmbientVolume();
            SaveSettings();
        }
    }
    public void ToggleStartingPos(int index)
    {
        mySettings.startingPos[index].isEnabled = myStartingPosToggles[index].isOn;
        if (!myStartingPosToggles[index].isOn)
        {
            myStartingPosToggles[index].GetComponent<Image>().color = myStartingPosToggles[index].colors.disabledColor;
        }
        else
        {
            myStartingPosToggles[index].GetComponent<Image>().color = Color.white;
        }
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
    public void ChangeScenarioNumber(int num)
    {
        int scenarioNum = int.Parse(scenarioField.text);
        scenarioNum += num;
        if (scenarioNum <= 1)
        {
            scenarioNum = 1;
        }
        else if (scenarioNum >= 99)
        {
            scenarioNum = 99;
        }
        scenarioField.text = scenarioNum.ToString();
        mySettings.scenarios = scenarioNum;
        SaveSettings();
    }
    public void ChangeImage(int num)
    {
        int keyLayoutNum = keyLayout;
        keyLayoutNum += num;
        if (keyLayoutNum < 0)
        {
            keyLayoutNum = 1;
        }
        else if (keyLayoutNum > 1)
        {
            keyLayoutNum = 0;
        }
        keyLayout = keyLayoutNum;
        if (keyLayout == 0)
        {
            controllerImage.enabled = true;
            keyBoardImage.enabled = false;
        }
        else if (keyLayout == 1)
        {
            controllerImage.enabled = false;
            keyBoardImage.enabled = true;
        }
    }
    public void ChangeScreenMode(int num)
    {
        int screenModeNum = screenMode;
        screenModeNum += num;
        if (screenModeNum < 0)
        {
            screenModeNum = 1;
        }
        else if (screenModeNum > 1)
        {
            screenModeNum = 0;
        }
        if (screenModeNum == 0)
        {
            screenModeText.text = "Full Screen";
            Screen.fullScreen = true;
        }
        else if (screenModeNum == 1)
        {
            screenModeText.text = "Windowed";
            Screen.fullScreen = false;
        }
        screenMode = screenModeNum;
        mySettings.screenMode = screenMode;
        SaveSettings();
    }
    public void Update()
    {
        for (int i = 0; i < mySettings.startingPos.Length; i++)
        {
            mySettings.startingPos[i].isEnabled = myStartingPosToggles[i].isOn;
        }
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
