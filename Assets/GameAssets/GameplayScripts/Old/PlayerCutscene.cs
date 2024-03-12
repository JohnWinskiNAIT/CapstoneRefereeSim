using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static PlayerControl;

public class PlayerCutscene : MonoBehaviour
{
    Vector3[] waypoints, cameraPoints;
    private Vector3 autoskateDestination;

    PlayerControl controls;
    PlayerCamera cam;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        GameplayEvents.CutsceneTrigger.AddListener(CutsceneListener);
        GameplayEvents.LoadCutscene.AddListener(LoadWaypoints);
        controls = GetComponent<PlayerControl>();
        cam = GetComponent<PlayerCamera>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controls.CurrentPlayerState == PlayerState.Autoskate)
        {
            AutoskateCheck();
        }
    }

    private void FixedUpdate()
    {
        if (controls.CurrentPlayerState == PlayerState.Autoskate)
        {
            //Logic for auto movement goes here
            AutoskateMovement();
        }
    }

    #region Cutscene Camera Methods
    private void ActivateCamera()
    {
        cam.SetState(PlayerCamera.CameraModes.FocusingOnPoint, cameraPoints);
    }
    #endregion

    #region Cutscene Movement Methods
    private void AutoskateCheck()
    {
        if ((autoskateDestination - transform.position).magnitude < 4f)
        {
            GameplayManager.Instance.moveDone = true;
            transform.position = autoskateDestination;
            rb.isKinematic = true;
        }
    }

    //Add force towards the autoskate destination and avoid exceeding speed limit.
    private void AutoskateMovement()
    {
        Vector3 direction = autoskateDestination - transform.position;

        if ((autoskateDestination - transform.position).magnitude > 2f)
        {
            rb.AddForce(direction.normalized * controls.accelerationSpeed * Time.fixedDeltaTime, ForceMode.Force);
        }


        Vector2 capTest = new(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > controls.maxSpeed || (autoskateDestination - transform.position).magnitude < 4f)
        {
            rb.velocity = new Vector3(rb.velocity.x * controls.breakingModifier, rb.velocity.y, rb.velocity.z * controls.breakingModifier);
        }
    }
    #endregion

    private void ResetPlayer()
    {
        //Reset camera and rb/transform here.
    }

    private void CutsceneListener(int progress)
    {
        rb.isKinematic = false;

        if (progress < waypoints.Length)
        {
            autoskateDestination = waypoints[progress];
        }
        if (controls.CurrentPlayerState != PlayerState.Autoskate)
        {
            controls.SetPlayerControl(PlayerState.Autoskate);
        }
    }

    private void LoadWaypoints(CutsceneData cutsceneData)
    {
        waypoints = cutsceneData.waypoints;
        cameraPoints = cutsceneData.cameraPoints;
    }
}