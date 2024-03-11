using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static PlayerControl;

public class PlayerCutscene : MonoBehaviour
{
    private Vector3 autoskateDestination;

    PlayerControl controls;
    GameObject waypointParent;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        GameplayEvents.CutsceneTrigger.AddListener(CutsceneListener);
        GameplayEvents.LoadCutscene.AddListener(LoadWaypoints);
        controls = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controls.playerState == PlayerState.Autoskate)
        {
            AutoskateCheck();
        }
    }

    private void AutoskateCheck()
    {
        if ((autoskateDestination - transform.position).magnitude < 4f)
        {
            GameplayManager.Instance.moveDone = true;
            transform.position = autoskateDestination;
            rb.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        if (controls.playerState == PlayerState.Autoskate)
        {
            //Logic for auto movement goes here
            AutoskateMovement();
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


        Vector2 capTest = new Vector2(rb.velocity.x, rb.velocity.z);
        if (capTest.magnitude > controls.maxSpeed || (autoskateDestination - transform.position).magnitude < 4f)
        {
            rb.velocity = new Vector3(rb.velocity.x * controls.breakingModifier, rb.velocity.y, rb.velocity.z * controls.breakingModifier);
        }
    }

    private void CutsceneListener(int progress)
    {
        rb.isKinematic = false;

        if (waypointParent.transform.GetChild(progress) != null)
        {
            autoskateDestination = waypointParent.transform.GetChild(progress).position;
        }
        if (controls.playerState != PlayerState.Autoskate)
        {
            controls.PlayerAutoskate(true);
        }
    }

    private void LoadWaypoints(CutsceneData cutsceneData)
    {
        waypointParent = cutsceneData.waypointParent;
    }
}
