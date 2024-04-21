using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckManager : MonoBehaviour
{
    public GameObject Owner { get; private set; }
    GameObject player;
    private Rigidbody rb;
    public ZoneAIController.AITeam? OwnerTeam { get; private set; }
    Vector3 savedVelocity;

    public float ignoredTime { get; private set; }

    private void Start()
    {
        GameplayEvents.InitializePlay.AddListener(InitializePuck);
        GameplayManager.Instance.DeclarePuck(gameObject);
        rb = GetComponent<Rigidbody>();
        AIManagerCallback();
    }

    private void AIManagerCallback()
    {
        AIManager.Instance.PuckCallback(gameObject);
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameplayManager.Instance.player;
            Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), player.GetComponentInChildren<Collider>(), true);
        }

        savedVelocity = rb.velocity;

        if (Owner == null)
        {
            ignoredTime += Time.deltaTime;
        }
        else
        {
            ignoredTime = 0;
        }
    }

    public void ChangePosession(GameObject posession)
    {
        if (Owner != null)
        {
            Owner.GetComponent<ZoneAIController>().LosePuck();
        }
        Owner = posession;
        OwnerTeam = posession.GetComponent<ZoneAIController>().aiTeam;
    }

    public void LoseOwner()
    {
        Owner = null;
        OwnerTeam = null;
    }

    public void ResetTime()
    {
        ignoredTime = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ArenaWall"))
        {
            rb.velocity = savedVelocity * 0.9f;
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
        }
    }

    private void InitializePuck()
    {
        LoseOwner();
        ResetTime();
        transform.position = GameplayManager.Instance.CurrentFaceoff.unscaledOffset;
    }
}
