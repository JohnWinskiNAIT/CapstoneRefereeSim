using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Oculus.Interaction.DebugTree;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Collections.Generic;
using System;

public class MenuControls : MonoBehaviour
{
    [SerializeField]
    GameObject[] panels;
    [SerializeField] Transform offScreen, onScreen;
    Button[] onScreenButtons;
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
    private void Start()
    {
        previousOnScreen = new List<GameObject>();
        canEscape = false;
        offScreenPanel = panels[menu + 1];
        onScreenPanel = panels[menu];
        offScreenPanel.transform.position = offScreen.position;
        time = 1.5f;
        ButtonUpdater();
        foreach (Button button in offScreenButtons)
        {
            button.interactable = false;
        }
        foreach(Slider slider in offScreenSliders)
        {
            slider.interactable = false;
        }
        foreach(Toggle toggle in offScreenToggles)
        {
            toggle.interactable = false;
        }
        for (int i = 0; i < onScreenButtons.Length; i++)
        {
            onScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
        }
        onScreenButtons[0].Select();
    }
    private void Update()
    {
        if (fade)
        {
            canEscape = false;
            fadeOn = false;
            for (int i = 0; i < onScreenButtons.Length; i++)
            {
                onScreenButtons[i].interactable = false;
            }
            for(int i = 0; i < onScreenSliders.Length; i++)
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
                    for(int i = 0; i < offScreenSliders.Length; i++)
                    {
                        offScreenSliders[i].interactable = true;
                    }
                    for(int i = 0; i < offScreenToggles.Length; i++)
                    {
                        offScreenToggles[i].interactable = true;
                    }
                    onScreenPanel.SetActive(false);
                    fadeOn = true;
                    time = 1f;
                    fade = false;
                }
                onScreenPanel.transform.position = Vector3.Lerp(onScreenPanel.transform.position, offScreen.position, 0.095f);
                offScreenPanel.transform.position = Vector3.Lerp(offScreenPanel.transform.position, onScreen.position, 0.1f);
            }
        }
    }
    public void Previous()
    {
        MenuSelector(previousPanel);
        previousOnScreen.Remove(offScreenPanel);
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
    public void Glow(GameObject callingObject)
    {
        callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.3f);
        callingObject.GetComponent<Button>().Select();
    }
    public void NoGlow(GameObject callingObject)
    {
        callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
    }
    public void Exit()
    {
        Application.Quit(0);
    }
    public void Play()
    {
        SceneManager.LoadScene("TestSceneA");
    }
}