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
        GameplayEvents.EndPlay.AddListener(EndPlay);
        GameplayEvents.CutsceneTrigger.AddListener(CutsceneCallback);
        currentMode = CameraModes.Normal;
    }

    // Update is called once per frame
    private void LateUpdate()
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

    private void EndPlay()
    {
        currentMode = CameraModes.FocusingOnPoint;
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
        nextAngle = Quaternion.LookRotation(focusPointParent.transform.GetChild(intendedPoint).position - transform.position, Vector3.up).eulerAngles;
        turnTimer = 0;
    }

    private void FocusCamera()
    {
        float turnProgress = turnTimer / turnDuration;
        if (turnProgress < 1)
        {
            Vector3 currentAngle = Vector3.Lerp(savedAngle, nextAngle, turnProgress);
            transform.rotation = Quaternion.Euler(currentAngle);

            turnTimer += Time.deltaTime;
        }
    }
}
