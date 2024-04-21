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

    bool paused;
    public float IgnoredTime { get; private set; }

    private void Start()
    {
        GameplayEvents.InitializePlay.AddListener(InitializePuck);
        GameplayManager.Instance.DeclarePuck(gameObject);
        rb = GetComponent<Rigidbody>();
        AIManagerCallback();
        GameplayEvents.SetPause.AddListener(PuckPause);
    }

    private void AIManagerCallback()
    {
        AIManager.Instance.PuckCallback(gameObject);
    }

    private void Update()
    {
        if (!paused)
        {
            if (player == null)
            {
                player = GameplayManager.Instance.player;
                Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), player.GetComponentInChildren<Collider>(), true);
            }

            savedVelocity = rb.velocity;

            if (Owner == null)
            {
                IgnoredTime += Time.deltaTime;
            }
            else
            {
                IgnoredTime = 0;
            }
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
        IgnoredTime = 0;
    }

    public void PuckPause(bool toggle)
    {
        if (toggle)
        {
            savedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
            paused = true;
        }
        else
        {
            rb.velocity = savedVelocity;
            paused = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PuckImpactManager>() != null)
        {
            rb.velocity = savedVelocity * 0.9f;
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            collision.gameObject.GetComponent<PuckImpactManager>().PlayImpactSound(collision.contacts[0].point);
        }
    }

    private void InitializePuck()
    {
        LoseOwner();
        ResetTime();
        transform.position = GameplayManager.Instance.CurrentFaceoff.unscaledOffset;
    }
}
