using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlaybackCameraManager : MonoBehaviour
{
    const float cameraSpeed = 75f;

    [SerializeField]
    InputActionAsset actions;

    [SerializeField]
    Vector3 objectOffset;

    [SerializeField]
    Button switchButton;

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
        cameraMovementAction = actions.FindActionMap("Gameplay").FindAction("Move");
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
                transform.position = overheadOffset;
                transform.rotation = Quaternion.Euler(90, 90, 0);
                break;
            case PlaybackCameraModes.Puck:
                MoveRotation(cameraMovementInput);
                CameraPosition(PlaybackManager.Instance.Puck);
                CameraRotation();
                break;
            case PlaybackCameraModes.Referee:
                MoveRotation(cameraMovementInput);
                CameraPosition(PlaybackManager.Instance.Referee);
                CameraRotation();
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
    
    void CameraPosition(GameObject source)
    {
        Vector3 finalOffset = Vector3.zero;
        finalOffset.z = objectOffset.z * Mathf.Cos(cameraXAngle * Mathf.Deg2Rad);
        finalOffset.x = objectOffset.z * Mathf.Sin(cameraXAngle * Mathf.Deg2Rad);
        finalOffset.y = objectOffset.y;

        transform.position = source.transform.position + finalOffset;
    }

    void CameraRotation()
    {
        transform.rotation = Quaternion.Euler(cameraYAngle, cameraXAngle, 0);
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
                switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Mode: Overhead";

                break;
            case PlaybackCameraModes.Puck:
                switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Mode: Puck";
                cameraYAngle = 30;
                break;
            case PlaybackCameraModes.Referee:
                switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Mode: Referee";
                cameraYAngle = 5;
                break;
        }
    }

    private void OnEnable()
    {
        cameraMovementAction.Enable();
    }

    private void OnDisable()
    {
        cameraMovementAction.Disable();
    }
}
