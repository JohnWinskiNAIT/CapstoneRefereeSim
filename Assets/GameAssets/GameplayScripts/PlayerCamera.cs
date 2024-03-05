using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    PlayerControl playerControls;
    [SerializeField]
    GameObject focusPointParent;

    [SerializeField]
    float turnDuration;

    float turnTimer;
    int currentPoint;
    Vector3 savedAngle, nextAngle;
    CameraModes currentMode;

    private enum CameraModes
    {
        Normal,
        FocusingOnPoint
    }
    // Start is called before the first frame update
    void Start()
    {
        playerControls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        GameplayEvents.LoadCutscene.AddListener(LoadCameraPoints);
        GameplayEvents.CutsceneTrigger.AddListener(CutsceneCallback);
        GameplayEvents.InitializePlay.AddListener(ResetCamera);

        currentMode = CameraModes.Normal;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (GameUtilities.VREnabled())
        {
            //Does not need to include anything? If we want a method of turning aside from turning 180 degrees, however, we will put it here.
        }
        else
        {
            switch (currentMode)
            {
                case CameraModes.Normal:
                    // == ROTATION ==
                    //Determine new values to rotate to.
                    //float xChange = transform.rotation.eulerAngles.x - playerControls.cameraAngle.x;
                    //float yChange = transform.rotation.eulerAngles.y - playerControls.cameraAngle.y;

                    //Debug.Log($"Angle 1: {transform.rotation.eulerAngles.x} Angle 2: {playerControls.cameraAngle.y}");

                    //Setting the rotation from euler atm.
                    //transform.rotation = Quaternion.Euler(playerControls.cameraAngle.x, playerControls.cameraAngle.y, 0);

                    transform.rotation = Quaternion.Euler(playerControls.cameraAngle.x, playerControls.cameraAngle.y, 0);
                    break;
                case CameraModes.FocusingOnPoint:
                    FocusCamera();
                    break;
                default:
                    break;
            }
        }
    }

    private void LoadCameraPoints(CutsceneData cutsceneData)
    {
        focusPointParent = cutsceneData.cameraParent;
    }

    private void CutsceneCallback(int progress)
    {
        CameraToPoint(progress);
    }

    private void CameraToPoint(int intendedPoint)
    {
        if (currentMode != CameraModes.FocusingOnPoint)
        {
            currentMode = CameraModes.FocusingOnPoint;
        }

        savedAngle = transform.rotation.eulerAngles;
        currentPoint = intendedPoint;
        nextAngle = Quaternion.LookRotation(focusPointParent.transform.GetChild(currentPoint).position - transform.position, Vector3.up).eulerAngles;

        Debug.Log(nextAngle);

        if (savedAngle.x > 180)
        {
            savedAngle.x -= 360;
        }
        if (savedAngle.y > 180)
        {
            savedAngle.y -= 360;
        }
        turnTimer = 0;
    }

    private void ResetCamera()
    {
        currentMode = CameraModes.Normal;
    }

    private void FocusCamera()
    {
        float turnProgress = turnTimer / turnDuration;
        if (turnProgress < 1)
        {
            Vector3 currentAngle = Vector3.Lerp(savedAngle, nextAngle, turnProgress);
            transform.rotation = Quaternion.Euler(currentAngle);

            nextAngle = Quaternion.LookRotation(focusPointParent.transform.GetChild(currentPoint).position - transform.position, Vector3.up).eulerAngles;
            turnTimer += Time.deltaTime;
        }
        else
        {
            turnTimer = turnDuration;
            transform.rotation = Quaternion.Euler(nextAngle);
            GameplayManager.Instance.cameraDone = true;
        }
    }
}
