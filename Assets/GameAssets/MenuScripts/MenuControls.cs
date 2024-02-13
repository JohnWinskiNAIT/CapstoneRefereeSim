using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class MenuControls : MonoBehaviour
{
    [SerializeField] Button[] mainMenuButtons;
    [SerializeField] Button[] optionsButtons;
    [SerializeField] GameObject optionsPanel, mainMenuPanel;
    [SerializeField] Transform offScreen, onScreen;
    [SerializeField]
    bool fade, fadeOn;
    Color col;
    float time;
    private void Start()
    {
        optionsPanel.transform.position = offScreen.position;
        time = 3.0f;
        optionsButtons = optionsPanel.GetComponentsInChildren<Button>();
        mainMenuButtons = mainMenuPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
        }
        mainMenuButtons[0].Select();
    }
    private void Update()
    {
        if (fade)
        {
            time = 3.0f;
            time -= Time.deltaTime;
            optionsPanel.SetActive(true);
            for (int i = 0; i < mainMenuButtons.Length; i++)
            {
                mainMenuButtons[i].interactable = false;
                if (mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().color.a > 0)
                {
                    col = mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().color;
                    col.a -= Time.deltaTime * 5f;
                    mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().color = col;
                }
            }
            for (int i = 0; i < optionsButtons.Length; i++)
            {
                optionsButtons[i].interactable = true;
                if (optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().color.a < 1)
                {
                    col = optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().color;
                    col.a += Time.deltaTime;
                    optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().color = col;
                }
            }

            if (time <= 0)
            {
                mainMenuPanel.SetActive(false);
                time = 6.0f;
            }
            mainMenuPanel.transform.position = Vector3.Lerp(mainMenuPanel.transform.position, offScreen.position, 0.01f);
            optionsPanel.transform.position = Vector3.Lerp(optionsPanel.transform.position, onScreen.position, 0.1f);
            fadeOn = true;
        }
        if (!fade && fadeOn)
        {
            time = 3.0f;
            time -= Time.deltaTime;
            mainMenuPanel.SetActive(true);
            for (int i = 0; i < optionsButtons.Length; i++)
            {
                optionsButtons[i].interactable = false;
                if (optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().color.a > 0)
                {
                    col = optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().color;
                    col.a -= Time.deltaTime * 5f;
                    optionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().color = col;
                }
            }
            for (int i = 0; i < mainMenuButtons.Length; i++)
            {
                mainMenuButtons[i].interactable = true;
                if (mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().color.a < 1)
                {
                    col = mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().color;
                    col.a += Time.deltaTime;
                    mainMenuButtons[i].GetComponentInChildren<TextMeshProUGUI>().color = col;
                }
            }
            if (time <= 0)
            {
                optionsPanel.SetActive(false);
                fadeOn = false;
                time = 3.0f;
            }
            mainMenuPanel.transform.position = Vector3.Lerp(mainMenuPanel.transform.position, onScreen.position, 0.1f);
            optionsPanel.transform.position = Vector3.Lerp(optionsPanel.transform.position, offScreen.position, 0.01f);
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
    public void Options()
    {
        fade = !fade;
    }
}
