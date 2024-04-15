using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaybackCameraManager : MonoBehaviour
{
    [SerializeField]
    InputActionAsset actions;

    InputAction cameraMovementAction;
    Vector2 cameraMovementInput;

    PlaybackCameraModes cameraMode;
    float cameraXAngle, cameraYAngle, cameraZoom;

    enum PlaybackCameraModes
    {
        Overhead,
        Puck,
        Referee
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraMovementInput = cameraMovementAction.ReadValue<Vector2>();

        switch (cameraMode)
        {
            case PlaybackCameraModes.Overhead:
                break;
            case PlaybackCameraModes.Puck:
                break;
            case PlaybackCameraModes.Referee:
                break;
        }
    }

    void UpdateMode()
    {
        cameraMode = (PlaybackCameraModes)((int)cameraMode + 1);
        if ((int)cameraMode > Enum.GetNames(typeof(PlaybackCameraModes)).Length)
        {
            cameraMode = 0;
        }

    }

    void DefaultPositionRotation()
    {
        
    }
}
