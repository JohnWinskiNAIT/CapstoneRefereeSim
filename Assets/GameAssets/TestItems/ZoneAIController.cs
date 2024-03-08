using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneAIController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    float checkThreshold, positionXResetThreshold, acceleration, maxSpeed, turningSpeed, passDelay, passSpeed;
    [SerializeField]
    int passChance;

    [SerializeField]
    AIType aiType;
    public AITeam aiTeam;
    [SerializeField]
    Vector3 puckOffset;

    [SerializeField]
    GameObject zonesParent;
    //Id 0 = center, Id 1 = left, Id 2 = right
    GameObject primaryZone;
    GameObject advanceZone;

    public GameObject[] teammates;

    Vector3 nextPosition;
    GameObject carryingPuck;

    float puckTimer;

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
        ManagerCallback();
    }

    // Update is called once per frame
    void Update()
    {
        if (teammates.Length == 0)
        {
            if (aiTeam == AITeam.Left && AIManager.Instance.leftTeamPlayers.Count > 0)
            {
                teammates = AIManager.Instance.leftTeamPlayers.ToArray();
            }
            if (aiTeam == AITeam.Right && AIManager.Instance.rightTeamPlayers.Count > 0)
            {
                teammates = AIManager.Instance.rightTeamPlayers.ToArray();
            }
        }

        if (PositionCheck())
        {
            GetNextPosition();
        }

        if (carryingPuck != null)
        {
            Vector3 finalOffset = transform.rotation * puckOffset;
            carryingPuck.transform.position = transform.position + finalOffset;
            PuckCheck();
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

        if (rb.velocity.magnitude != 0)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
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
        Transform primaryTransform = primaryZone.transform;

        float newX = Random.Range(primaryTransform.position.x - primaryTransform.localScale.x / 2, primaryTransform.position.x + primaryTransform.localScale.x / 2);
        float newZ = Random.Range(primaryTransform.position.z - primaryTransform.localScale.z / 2, primaryTransform.position.z + primaryTransform.localScale.z / 2);

        nextPosition = new Vector3(newX, 0, newZ);
        //Debug.Log(nextPosition);
    }

    void PuckCheck()
    {
        puckTimer += Time.deltaTime;
        if (puckTimer >= passDelay)
        {
            if (Random.Range(1, passChance + 1) == 1)
            {
                List<GameObject> teammates;
                if (aiTeam == AITeam.Left)
                {
                    teammates = AIManager.Instance.leftTeamPlayers;
                }
                else
                {
                    teammates = AIManager.Instance.rightTeamPlayers;
                }

                int target = Random.Range(0, teammates.Count);
                int reps = 0;

                while (AIManager.Instance.leftTeamPlayers[target] == gameObject && reps < 10)
                {
                    target = Random.Range(0, AIManager.Instance.leftTeamPlayers.Count);
                    reps++;
                }
                PassPuck(teammates[target]);
            }
        }
    }

    public void LosePuck()
    {
        if (carryingPuck != null)
        {
            carryingPuck.GetComponent<Rigidbody>().isKinematic = false;
            carryingPuck = null;
        }
    }

    void PassPuck(GameObject target)
    {
        Vector3 passVector = target.transform.position - carryingPuck.transform.position;
        carryingPuck.GetComponent<Rigidbody>().isKinematic = false;
        carryingPuck.GetComponent<Rigidbody>().AddForce(passVector.normalized * passSpeed, ForceMode.Impulse);
        carryingPuck.GetComponent<PuckManager>().LoseOwner();
        carryingPuck = null;

        puckTimer = 0f;
    }

    void ManagerCallback()
    {
        if (aiTeam == AITeam.Left)
        {
            AIManager.Instance.leftTeamPlayers.Add(gameObject);
        }
        else
        {
            AIManager.Instance.rightTeamPlayers.Add(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PuckManager>() != null && other.GetComponentInParent<PuckManager>().OwnerTeam != aiTeam)
        {
            carryingPuck = other.transform.parent.gameObject;
            carryingPuck.GetComponent<PuckManager>().ChangePosession(gameObject);
            other.gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;
        }
    }
}
