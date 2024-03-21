using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettingsContainer SettingsContainer { get; private set; }
    public SettingsData mySettings;

    [Serializable]
    public struct GameSettingsContainer
    {
        //Number of players. The remainder will be distributed randomly and the rest are split evenly.
        public int numberOfPlayers;
        //Bool list to be read that will detemrine which types of penalties are disabled.
        public PenaltyData[] enabledPenalties;
        public int numberOfscenarios;

        public GameSettingsContainer(int playerNumber, PenaltyData[] penaltyList, int scenarios)
        {
            numberOfPlayers = playerNumber;
            enabledPenalties = penaltyList;
            numberOfscenarios = scenarios;
        }
    }


    private void Awake()
    {
        DefaultSettings();
    }

    private void DefaultSettings()
    {
        //Set values to defaults then save a settings file.

        //Setting values.
        SettingsContainer = new GameSettingsContainer(10, new PenaltyData[9], 1);
    }

    private void LoadSettings()
    {
        GameSettingsContainer loadedSettings;
        loadedSettings = new GameSettingsContainer();
        //Insert file loading logic here. This is placeholder so it compiles.
        int playersCount = 0;
        loadedSettings.numberOfPlayers = playersCount;
        loadedSettings.enabledPenalties = mySettings.penalties;
        loadedSettings.numberOfscenarios = mySettings.scenarios;
        SettingsContainer = loadedSettings;
    }
}
