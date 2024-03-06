using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LandingBehav : MonoBehaviour
{
    [SerializeField] InputActionAsset keyboardInputs;
    [SerializeField] InputActionAsset controllerInputs;
    [SerializeField] InputActionAsset vrLeftInputs;
    [SerializeField] InputActionAsset vrRightInputs;
    

    void Start()
    {
        GameUtilities.SetInputActions(keyboardInputs, controllerInputs, vrLeftInputs, vrRightInputs);
    }

    void Update()
    {
        if (GameUtilities.VREnabled())
        {
            SceneManager.LoadScene(""); //MenuSceneVR
        }
        else
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
