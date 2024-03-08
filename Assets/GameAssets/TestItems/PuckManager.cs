using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckManager : MonoBehaviour
{
    public GameObject Owner { get; private set; }
    private Rigidbody rb;
    public ZoneAIController.AITeam? OwnerTeam {get; private set;}
    Vector3 savedVelocity;
    Vector3 lastPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        savedVelocity = rb.velocity;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ArenaWall"))
        {
            rb.velocity = savedVelocity;
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
        }
    }
}
