using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private NavMeshAgent agent;
    //public InputAction moveAction;
    //[SerializeField] Vector2 moveValue;
    Vector3 destination, movementVector;
    //[SerializeField] float movementSpeed = 1;

    [SerializeField] List<Vector3> destinationList;
    [SerializeField] int maxDestinations, currentDestination = 0 ;
    [SerializeField] float timeStamp, timewait;
    void Start()
    {
        maxDestinations = destinationList.Count;
        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        //moveValue = moveAction.ReadValue<Vector2>();
        //movementVector = new Vector3(moveValue.x,0,moveValue.y);

        //destination = transform.position + movementVector*movementSpeed;
        
        //agent.destination = destination;
    }
    private void Update()
    {
        // Once the timer is up, The current destination is changed to the next destination in the list. If it is at the last destination in the list it goes back to the first.
        if (Time.time > timeStamp + timewait)
        {
            if (currentDestination < maxDestinations)
            {
                currentDestination++;
                timeStamp = Time.time;
            }
            else
            {
                currentDestination = 0;
                timeStamp = Time.time;
            }
        }
        agent.destination = destinationList[currentDestination];
    }
    //private void OnEnable()
    //{
    //    moveAction.Enable();
    //}
    //private void OnDisable()
    //{
    //    moveAction.Disable();
    //}
}
