using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    GameObject playerCam;

    PlayerControl playerControls;
    Vector3[] focusPoints;

    [SerializeField]
    float camSpeed;

    float turnTimer;
    int currentPoint;
    public CameraModes CurrentMode { get; private set; }

    public enum CameraModes
    {
        Normal,
        FocusingOnPoint
    }

    void Start()
    {
        playerControls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        GameplayEvents.LoadCutscene.AddListener(LoadCameraPoints);
        GameplayEvents.CutsceneTrigger.AddListener(CutsceneCallback);
        GameplayEvents.InitializePlay.AddListener(ResetCamera);
        GameplayEvents.EndCutscene.AddListener(CutsceneEndCallback);

        CurrentMode = CameraModes.Normal;
    }

    private void LateUpdate()
    {
        if (CurrentMode == CameraModes.Normal)
        {
            playerCam.transform.rotation = Quaternion.Euler(playerControls.CameraAngle.x, playerControls.CameraAngle.y, 0);
        }
        if (CurrentMode == CameraModes.FocusingOnPoint)
        {
            FocusCamera();
        }

    }

    private void LoadCameraPoints(CutsceneData cutsceneData)
    {
        focusPoints = cutsceneData.cameraPoints;
        playerCam.transform.rotation = Quaternion.LookRotation(GameplayManager.Instance.CurrentFaceoff.unscaledOffset - playerCam.transform.position);
    }

    private void CutsceneCallback(int progress)
    {
        CameraToPoint(progress);
    }

    private void CutsceneEndCallback()
    {
        playerControls.SetCamAngles(playerCam.transform.rotation.eulerAngles);
        CurrentMode = CameraModes.Normal;
    }

    private void CameraToPoint(int intendedPoint)
    {
        if (CurrentMode != CameraModes.FocusingOnPoint)
        {
            CurrentMode = CameraModes.FocusingOnPoint;
        }
        currentPoint = intendedPoint;
    }
        
    public void SetState(CameraModes newMode)
    {
        CurrentMode = newMode;
    }

    public void SetState(CameraModes newMode, Vector3[] cameraPoints)
    {
        CurrentMode = newMode;
        focusPoints = cameraPoints;
    }

    public void ResetCamera()
    {
        //CurrentMode = CameraModes.Normal;
    }

    private void FocusCamera()
    {
        Quaternion lerpTest1 = playerCam.transform.rotation;
        Quaternion lerpTest2 = Quaternion.LookRotation(focusPoints[currentPoint] - playerCam.transform.position);

        if (!GameUtilities.VREnabled())
        {
            if (Quaternion.Angle(lerpTest1, lerpTest2) > 1f)
            {
                float step = camSpeed * Time.deltaTime;

                //transform.rotation = Quaternion.Euler(currentAngle);
                playerCam.transform.rotation = Quaternion.RotateTowards(lerpTest1, lerpTest2, step);

                //nextAngle = Quaternion.LookRotation(focusPoints[currentPoint] - transform.position, Vector3.up).eulerAngles;
                turnTimer += Time.deltaTime;
            }
            else
            {
                playerCam.transform.rotation = lerpTest2;
                GameplayManager.Instance.cameraDone = true;
            }
        }
        else
        {
            GameplayManager.Instance.cameraDone = true;
        }
    }
}
