using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaybackCameraManager : MonoBehaviour
{
    const float cameraSpeed = 3f;

    [SerializeField]
    InputActionAsset actions;

    InputAction cameraMovementAction;
    Vector2 cameraMovementInput;

    Vector3 overheadOffset;

    PlaybackCameraModes cameraMode;
    float cameraXAngle, cameraYAngle, cameraZoom;

    enum PlaybackCameraModes
    {
        Overhead,
        Puck,
        Referee
    }

    private void Awake()
    {
        actions.FindActionMap("Gameplay").FindAction("Move");
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraMode = PlaybackCameraModes.Overhead;
        overheadOffset = transform.position;
        cameraXAngle = 0;
        cameraYAngle = 0;
        cameraZoom = 0;
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
                MoveRotation(cameraMovementInput);
                break;
            case PlaybackCameraModes.Referee:
                break;
        }
    }

    void MoveRotation(Vector2 motionInput)
    {
        cameraXAngle += motionInput.x * cameraSpeed * Time.deltaTime;
        cameraYAngle += motionInput.y * cameraSpeed * Time.deltaTime;

        if (cameraXAngle < 0)
        {
            cameraXAngle += 360;
        }
        if (cameraXAngle > 360)
        {
            cameraXAngle -= 360;
        }
    }

    public void UpdateMode()
    {
        cameraMode = (PlaybackCameraModes)((int)cameraMode + 1);
        if ((int)cameraMode > Enum.GetNames(typeof(PlaybackCameraModes)).Length)
        {
            cameraMode = 0;
        }
        DefaultPositionRotation();
    }

    void DefaultPositionRotation()
    {
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
}
