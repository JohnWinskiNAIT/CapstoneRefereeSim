using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Oculus.Interaction.DebugTree;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class MenuControls : MonoBehaviour
{
    [SerializeField] Button[] onScreenButtons;
    [SerializeField] Button[] offScreenButtons;
    [SerializeField] GameObject offScreenPanel, onScreenPanel;
    [SerializeField] Transform offScreen, onScreen;
    [SerializeField]
    bool fade, fadeOn;
    [SerializeField]
    int menu;
    [SerializeField]
    GameObject[] panels;
    int previousPanel;
    Color col;
    [SerializeField] float time;
    private void Start()
    {
        offScreenPanel.transform.position = offScreen.position;
        time = 1.0f;
        onScreenButtons = onScreenPanel.GetComponentsInChildren<Button>();
        offScreenButtons = offScreenPanel.GetComponentsInChildren<Button>();
        foreach(Button button in offScreenButtons)
        {
            button.interactable = false;
        }
        for (int i = 0; i < onScreenButtons.Length; i++)
        {
            onScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
        }
        onScreenButtons[0].Select();
    }
    private void Update()
    {
        if(fade)
        {
            MenuSwitcher(menu);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            fade = true;
        }

    }
    public void menuSelector(int thismenu)
    {
        menu = thismenu;
        fade = true;
    }
    public void MenuSwitcher(int switchToMenu)
    {
        for(int j = 0; j < panels.Length; j++)
        {
            if (j == switchToMenu)
            {
                offScreenPanel = panels[j];
                offScreenPanel.SetActive(true);
                for (int i = 0; i < onScreenButtons.Length; i++)
                {
                    onScreenButtons[i].interactable = false;
                    if (onScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().color.a > 0)
                    {
                        col = onScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().color;
                        col.a -= Time.deltaTime * 5f;
                        onScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().color = col;
                    }
                }
                for (int i = 0; i < offScreenButtons.Length; i++)
                {
                    if (offScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().color.a < 1)
                    {
                        col = offScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().color;
                        col.a += Time.deltaTime * 0.5f;
                        offScreenButtons[i].GetComponentInChildren<TextMeshProUGUI>().color = col;
                    }
                }

                if (time > 0 && !fadeOn)
                {
                    time -= Time.deltaTime;
                    if (time <= 0)
                    {
                        for (int i = 0; i < offScreenButtons.Length; i++)
                        {
                            offScreenButtons[i].interactable = true;
                        }
                        onScreenPanel.SetActive(false);
                        time = 1.0f;
                        fadeOn = true;
                        fade = false;
                    }
                }
                onScreenPanel.transform.position = Vector3.Lerp(onScreenPanel.transform.position, offScreen.position, 0.01f);
                offScreenPanel.transform.position = Vector3.Lerp(offScreenPanel.transform.position, onScreen.position, 0.1f);
            }
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
    public void Exit()
    {
        Application.Quit(0);
    }
    public void Play()
    {
        SceneManager.LoadScene("TestSceneA");
    }
}
