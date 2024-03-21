using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Oculus.Interaction.DebugTree;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using System.IO;

public class PauseManager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider SFXSlider;
    public Slider ambientSlider;
    [SerializeField] AudioMixer audioMixer;
    public TMP_InputField screenModeText;
    public TMP_Text muteText;
    public Image controllerImage;
    public Image keyBoardImage;
    int screenMode;
    public int keyLayout;
    bool mute;
    public SettingsData mySettings;
    string filePath;
    const string RootPath = "SaveData\\";

    [SerializeField]
    GameObject[] panels;
    Transform offScreen, onScreen;
    public Button[] onScreenButtons;
    Slider[] onScreenSliders;
    Toggle[] onScreenToggles;
    Button[] offScreenButtons;
    Slider[] offScreenSliders;
    Toggle[] offScreenToggles;
    GameObject offScreenPanel, onScreenPanel;
    List<GameObject> previousOnScreen;
    bool fade, fadeOn, canEscape;
    int menu;
    int previousPanel;
    float time;
    Vector3 velocity;

    [SerializeField]
    float pauseTransitionTime;

    [SerializeField]
    RectTransform pauseMenu;

    public static PauseManager instance;

    float speed = 0;
    bool paused;
    float pauseTimer;
    //Reference UI parent this needs to enable and disable along with individual parts it needs to control.

    void Start()
    {
        offScreen = GameObject.Find("OffScreen").transform;
        onScreen = GameObject.Find("OnScreen").transform;
        previousOnScreen = new List<GameObject>();
        canEscape = false;
        offScreenPanel = panels[menu + 1];
        onScreenPanel = panels[menu];
        offScreenPanel.transform.position = offScreen.position;
        time = 1.5f;
        keyLayout = 1;
        mySettings = Settings.mySettings;
        filePath = RootPath + "settingsData\\settings.dat";
        if (File.Exists("SaveData\\settingsData\\settings.dat"))
        {
            LoadSettings();
            masterSlider.value = mySettings.masterVolume;
            SFXSlider.value = mySettings.SFXvolume;
            ambientSlider.value = mySettings.ambientVolume;
            if (mySettings.screenMode != 0)
            {
                screenMode = mySettings.screenMode;
                if (mySettings.screenMode == 0)
                {
                    screenModeText.text = "Full Screen";
                    Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
                }
                else
                {
                    screenModeText.text = "Windowed";
                    Screen.SetResolution(1280, 720, false);
                }
            }
        }
        else
        {
            masterSlider.value = 1;
            SFXSlider.value = 1;
            ambientSlider.value = 1;

            SaveSettings();
        }
        ButtonUpdater();
        foreach (Button button in offScreenButtons)
        {
            button.interactable = false;
        }
        foreach (Slider slider in offScreenSliders)
        {
            slider.interactable = false;
        }
        foreach (Toggle toggle in offScreenToggles)
        {
            toggle.interactable = false;
        }
        for (int i = 0; i < onScreenButtons.Length; i++)
        {
            onScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
        }
        onScreenButtons[0].Select();
    }
    // Declares itself instance. Do *NOT* give this DontDestroyOnLoad.
    // This should be destroyed and recreated when swapping from main menu to gameplay.
    // Game data and settings should be stored in common scripts.
    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<PauseManager>();
        }
        else
        {
            Destroy(gameObject);
        }

        GameplayEvents.SetPause.AddListener(PauseGame);
    }

    private void Update()
    {
        if (paused)
        {
            pauseTimer = Mathf.SmoothDamp(pauseTimer, 1, ref speed, pauseTransitionTime);
            if (pauseTimer > 0.998f)
            {
                pauseTimer = 1;
            }
        }

        if (!paused && pauseTimer > 0)
        {
            pauseTimer = Mathf.SmoothDamp(pauseTimer, 0, ref speed, pauseTransitionTime);
            if (pauseTimer <= 0.002f)
            {
                pauseTimer = 0;
                GameplayEvents.SetPause.Invoke(false);
            }
        }

        pauseMenu.anchorMax = Vector3.Lerp(new Vector3(0, 1), new Vector3(1, 1), pauseTimer);
        pauseMenu.anchorMin = Vector3.Lerp(new Vector3(-1, 0), new Vector3(0, 0), pauseTimer);
        if (fade)
        {
            canEscape = false;
            fadeOn = false;
            for (int i = 0; i < onScreenButtons.Length; i++)
            {
                onScreenButtons[i].interactable = false;
            }
            for (int i = 0; i < onScreenSliders.Length; i++)
            {
                onScreenSliders[i].interactable = false;
            }
            for (int i = 0; i < onScreenToggles.Length; i++)
            {
                onScreenToggles[i].interactable = false;
            }
            MenuSwitcher(menu);
            if (time > 0 && !fadeOn)
            {
                time -= Time.deltaTime;
            }
        }
        if (!fade && fadeOn)
        {
            onScreenPanel = GameObject.FindGameObjectWithTag("MenuPanel");
            if (previousOnScreen.Count > 0)
            {
                offScreenPanel = previousOnScreen[previousOnScreen.Count - 1];
            }
            else
            {
                int index = Array.IndexOf(panels, onScreenPanel);
                offScreenPanel = panels[index + 1];
            }
            ButtonUpdater();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && canEscape && onScreenPanel != panels[0])
        {
            Previous();
        }
        if (masterSlider.value > 0.0001f || SFXSlider.value > 0.0001f || ambientSlider.value > 0.0001f)
        {
            muteText.text = "Mute";
            mute = false;
        }
        else
        {
            muteText.text = "Unmute";
            mute = true;
        }
    }
    public void MenuSelector(int thismenu)
    {
        if (!fade)
        {
            canEscape = false;
            if (thismenu == 0)
            {
                previousPanel = 1;
                previousOnScreen.Add(onScreenPanel);
            }
            if (thismenu == 1)
            {
                previousPanel = 0;
                previousOnScreen.Add(onScreenPanel);
            }
            else if (thismenu >= 2)
            {
                previousPanel = 1;
                previousOnScreen.Add(onScreenPanel);
            }
            menu = thismenu;
            fade = true;
        }
    }
    public void MenuSwitcher(int switchToMenu)
    {
        for (int j = 0; j < panels.Length; j++)
        {
            if (j == switchToMenu)
            {
                offScreenPanel = panels[j];
                offScreenPanel.SetActive(true);
                if (time <= 0)
                {
                    for (int i = 0; i < offScreenButtons.Length; i++)
                    {
                        offScreenButtons[i].interactable = true;
                    }
                    for (int i = 0; i < offScreenSliders.Length; i++)
                    {
                        offScreenSliders[i].interactable = true;
                    }
                    for (int i = 0; i < offScreenToggles.Length; i++)
                    {
                        offScreenToggles[i].interactable = true;
                    }
                    onScreenPanel.SetActive(false);
                    fadeOn = true;
                    time = 1f;
                    fade = false;
                }
                onScreenPanel.transform.position = Vector3.Lerp(onScreenPanel.transform.position, offScreen.position, 0.095f);
                offScreenPanel.transform.position = Vector3.SmoothDamp(offScreenPanel.transform.position, onScreen.position, ref velocity, 0.1f);
            }
        }
    }
    public void Previous()
    {
        MenuSelector(previousPanel);
        previousOnScreen.Remove(offScreenPanel);
    }
    public void PauseGame(bool pausing)
    {
        paused = pausing;
        if (pausing)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void Glow(GameObject callingObject)
    {
        callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.3f);
        callingObject.GetComponent<Button>().Select();
    }
    public void NoGlow(GameObject callingObject)
    {
        callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
    }
    void ButtonUpdater()
    {
        onScreenButtons = onScreenPanel.GetComponentsInChildren<Button>();
        onScreenSliders = onScreenPanel.GetComponentsInChildren<Slider>();
        onScreenToggles = onScreenPanel.GetComponentsInChildren<Toggle>();
        offScreenButtons = offScreenPanel.GetComponentsInChildren<Button>();
        offScreenSliders = offScreenPanel.GetComponentsInChildren<Slider>();
        offScreenToggles = offScreenPanel.GetComponentsInChildren<Toggle>();
        canEscape = true;
    }
    public void Mute()
    {
        mute = !mute;
        if (mute)
        {
            if (masterSlider.value > 0.0001f || SFXSlider.value > 0.0001f || ambientSlider.value > 0.0001f)
            {
                mySettings.lastMasterVolume = masterSlider.value;
                mySettings.lastSFXvolume = SFXSlider.value;
                mySettings.lastAmbientVolume = ambientSlider.value;
            }
            masterSlider.value = 0.0001f;
            ChangeMasterVolume();
            SFXSlider.value = 0.0001f;
            ChangeSFXVolume();
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
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }
        else if (screenModeNum == 1)
        {
            screenModeText.text = "Windowed";
            Screen.SetResolution(1280, 720, false);
        }
        screenMode = screenModeNum;
        mySettings.screenMode = screenMode;
        SaveSettings();
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MenuScene");
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
        SettingsHolder.mySettings = mySettings;
    }
    public void LoadSettings()
    {
        SaveManager.LoadData(filePath, ref mySettings);
    }
}
