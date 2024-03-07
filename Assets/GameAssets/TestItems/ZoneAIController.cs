using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneAIController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    float checkThreshold, positionXResetThreshold, acceleration, maxSpeed, turningSpeed;
    [SerializeField]
    AIType aiType;
    [SerializeField]
    AITeam aiTeam;

    [SerializeField]
    GameObject zonesParent;
    //Id 0 = center, Id 1 = left, Id 2 = right
    GameObject[] zones;
    GameObject primaryZone;

    Vector3 nextPosition;


    public enum AIType
    {
        Forward,
        Defense
    }

    public enum AITeam
    {
        Left,
        Right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        /*zones = new GameObject[zonesParent.transform.childCount];
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i] = zonesParent.transform.GetChild(i).gameObject;
        }*/
        if (aiType == AIType.Forward)
        {
            primaryZone = zonesParent.transform.GetChild(0).gameObject;
        }
        else
        {
            if (aiTeam == AITeam.Left)
            {
                primaryZone = zonesParent.transform.GetChild(1).gameObject;
            }
            else
            {
                primaryZone = zonesParent.transform.GetChild(2).gameObject;
            }
        }
        GetNextPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (PositionCheck())
        {
            GetNextPosition();
        }
    }

    private void FixedUpdate()
    {
        if (nextPosition != null)
        {
            AIMovement();
        }
    }

    void AIMovement()
    {
        Vector3 moveVector = nextPosition - transform.position;

        rb.AddForce(moveVector.normalized * acceleration * Time.deltaTime, ForceMode.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    bool PositionCheck()
    {
        bool returnValue = false;

        if ((transform.position - nextPosition).magnitude <= checkThreshold)
        {
            returnValue = true;
        }

        if (Mathf.Abs(nextPosition.x - primaryZone.transform.position.x) > positionXResetThreshold)
        {
            returnValue = true;
        }

        return returnValue;
    }

    void GetNextPosition()
    {
        Vector3 primaryTransform = primaryZone.transform.position;

        float newX = Random.Range(primaryTransform.z + primaryTransform.x / 2, primaryTransform.x - primaryTransform.x / 2);
        float newZ = Random.Range(primaryTransform.z + primaryTransform.z / 2, primaryTransform.z - primaryTransform.z / 2);

        nextPosition = new Vector3(newX, 0, newZ);
    }
}
