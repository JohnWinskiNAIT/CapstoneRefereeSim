using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public GameSettingsContainer settingsContainer { get; private set; }
    public SettingsData mySettings;
    public string rootPath = "SaveData\\settingsData";
    public enum WindowedSettings
    {
        Fullscreen,
        Windowed
    }

    public enum GameResolutions
    {
        WIDE1920_1080,
        WIDE1280_720
    }

    [Serializable]
    public struct GameSettingsContainer
    {
        //Number of players. The remainder will be distributed randomly and the rest are split evenly.
        public int numberOfPlayers;
        //Fullscreen, windowed, or bordered if we add it.
        public WindowedSettings windowedSettings;
        //Resolutions. Can be added later.
        public GameResolutions resolutions;
        //Bool list to be read that will detemrine which types of penalties are disabled.
        public bool[] enabledPenalties;

        public GameSettingsContainer(int playerNumber, WindowedSettings settings, GameResolutions resolution, bool[] penaltyList)
        {
            numberOfPlayers = playerNumber;
            windowedSettings = settings;
            resolutions = resolution;
            enabledPenalties = penaltyList;
        }
    }


    private void Awake()
    {
        //Logic to look for an external file with settings. If it exists, load them. If not, create a default.
        mySettings = new SettingsData();
        if (Directory.Exists(rootPath))
        {
            SaveManager.LoadData(rootPath + "\\settings.dat", ref mySettings);
            LoadSettings();
        }
    }

    private void DefaultSettings()
    {
        //Set values to defaults then save a settings file.

        //Setting values.
        settingsContainer = new GameSettingsContainer(10, WindowedSettings.Fullscreen, GameResolutions.WIDE1920_1080, new bool[0]);
    }

    private void LoadSettings()
    {
        GameSettingsContainer loadedSettings;

        //Insert file loading logic here. This is placeholder so it compiles.
        loadedSettings = new GameSettingsContainer();
        settingsContainer = loadedSettings;
    }
}
