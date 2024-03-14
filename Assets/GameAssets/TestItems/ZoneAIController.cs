using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneAIController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    float checkThreshold, positionZResetThreshold, acceleration, maxSpeed, turningSpeed, passDelay, passSpeed, lockoutTime;
    [SerializeField]
    int passChance;
    [SerializeField]
    Material[] teamMaterials;

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

    Vector3 startPosition;
    Vector3 nextPosition;
    Vector3 savedVelocity;
    GameObject carryingPuck;

    float puckTimer, lockoutTimer;

    bool aiActive;

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

    private void Awake()
    {
        GameplayEvents.InitializePlay.AddListener(InitializeForPlay);
        GameplayEvents.LoadCutscene.AddListener(CutsceneStartCallback);
        GameplayEvents.EndCutscene.AddListener(CutsceneEndCallback);
        GameplayEvents.SetPause.AddListener(PauseAI);
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
            if (aiTeam == AITeam.Left)
            {
                advanceZone = zonesParent.transform.GetChild(2).gameObject;
            }
            else
            {
                advanceZone = zonesParent.transform.GetChild(1).gameObject;
            }
        }
        else
        {
            advanceZone = zonesParent.transform.GetChild(0).gameObject;
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

    public void SetupAIAttributes(AIType typeOfPlayer, AITeam playerTeam, GameObject zoneParentObject, Vector3 baseSpawn)
    {
        aiType = typeOfPlayer;
        aiTeam = playerTeam;
        zonesParent = zoneParentObject;
        startPosition = baseSpawn;

        GetComponentInChildren<MeshRenderer>().material = teamMaterials[(int)playerTeam];
    }

    // Update is called once per frame
    void Update()
    {
        if (aiActive)
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

            if (lockoutTimer > 0)
            {
                lockoutTimer -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (aiActive)
        {
            if (nextPosition != null)
            {
                AIMovement();
            }
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

        if (rb.velocity.magnitude != 0 && rb.velocity.normalized != Vector3.forward)
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

        if (Mathf.Abs(nextPosition.z - primaryZone.transform.position.z) > positionZResetThreshold)
        {
            returnValue = true;
        }

        return returnValue;
    }

    void GetNextPosition()
    {
        Transform primaryTransform = primaryZone.transform;
        Transform advanceTransform = advanceZone.transform;

        float newX = 0;
        float newZ = 0;
        if (carryingPuck == null)
        {
            newX = Random.Range(primaryTransform.position.x - primaryTransform.localScale.x / 2, primaryTransform.position.x + primaryTransform.localScale.x / 2);
            newZ = Random.Range(primaryTransform.position.z - primaryTransform.localScale.z / 2, primaryTransform.position.z + primaryTransform.localScale.z / 2);
        }
        else
        {
            if (advanceTransform.position.z > primaryZone.transform.position.x)
            {
                newX = Random.Range(primaryTransform.position.x - primaryTransform.localScale.x / 2, advanceTransform.position.x + advanceTransform.localScale.x / 2);
                newZ = Random.Range(primaryTransform.position.z - primaryTransform.localScale.z / 2, advanceTransform.position.z + advanceTransform.localScale.z / 2);
            }
            else
            {
                newX = Random.Range(advanceTransform.position.x - advanceTransform.localScale.x / 2, primaryTransform.position.x + primaryTransform.localScale.x / 2);
                newZ = Random.Range(advanceTransform.position.z - advanceTransform.localScale.z / 2, primaryTransform.position.z + primaryTransform.localScale.z / 2);
            }
        }

        nextPosition = new Vector3(newX, 0, newZ);
        //Debug.Log(nextPosition);
    }

    public void DeclarePosition(Vector3 newPosition)
    {
        nextPosition = newPosition;
    }

    void PuckCheck()
    {
        puckTimer += Time.deltaTime;
        if (puckTimer >= passDelay)
        {
            if (Random.Range(0, passChance) == 0)
            {
                int target = Random.Range(0, teammates.Length);
                int reps = 0;

                while (AIManager.Instance.leftTeamPlayers[target] == gameObject && reps < 10)
                {
                    target = Random.Range(0, teammates.Length);
                    reps++;
                }
                PassPuck(teammates[target]);
            }
            else
            {
                puckTimer = 0f;
            }
        }
    }

    public void LosePuck()
    {
        if (carryingPuck != null)
        {
            carryingPuck.GetComponent<Rigidbody>().isKinematic = false; 
            carryingPuck = null;
            puckTimer = 0f;
        }
    }

    void PassPuck(GameObject target)
    {
        Vector3 passVector = target.transform.position - carryingPuck.transform.position;
        carryingPuck.GetComponent<Rigidbody>().isKinematic = false;
        carryingPuck.GetComponent<Rigidbody>().AddForce(passVector.normalized * passSpeed, ForceMode.Impulse);
        carryingPuck.GetComponent<PuckManager>().LoseOwner();
        lockoutTimer = lockoutTime;
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

    void InitializeForPlay()
    {
        transform.position = startPosition;
        puckTimer = 0f;
        if (carryingPuck != null)
        {
            carryingPuck = null;
        }
        nextPosition = transform.position;
    }

    void PauseAI(bool pausing)
    {
        if (pausing)
        {
            savedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
            aiActive = false;
        }
        else
        {
            rb.velocity = savedVelocity;
            aiActive = true;
        }
    }

    private void CutsceneStartCallback(CutsceneData data)
    {
        aiActive = false;
    }

    private void CutsceneEndCallback()
    {
        aiActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PuckManager>() != null && other.GetComponentInParent<PuckManager>().OwnerTeam != aiTeam && lockoutTimer <= 0)
        {
            carryingPuck = other.transform.parent.gameObject;
            carryingPuck.GetComponent<PuckManager>().ChangePosession(gameObject);
            other.gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;
        }
    }
}
