using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class MenuControls : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    private void Start()
    {
        for(int i  = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
        }
    }
    public void Glow(GameObject callingObject)
    {
        callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.3f);
    }
    public void NoGlow(GameObject callingObject)
    {
        callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0);
    }
    public void Options()
    {

    }
}
