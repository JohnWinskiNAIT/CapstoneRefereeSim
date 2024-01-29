using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private NavMeshAgent agent;

    Vector3 destination, movementVector;


    [SerializeField] List<Vector3> destinationList;
    [SerializeField] List<float> timeToWait;
    //Each destionation must have its assoiated time to wait for its destination. Ensure that time to wait and destination list have the same count of lists. 

    [SerializeField] int maxDestinations, currentDestinationIndicator = 0;
    [SerializeField] float timeStamp;
    void Start()
    {
        maxDestinations = destinationList.Count;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = destinationList[currentDestinationIndicator];
    }

    void FixedUpdate()
    {

    }
    private void Update()
    {
        // Once the timer is up, The current destination is changed to the next destination in the list. If it is at the last destination in the list it goes back to the first.
        if (Time.time > timeStamp + timeToWait[currentDestinationIndicator])
        {
            currentDestinationIndicator++;
            if (currentDestinationIndicator >= maxDestinations)
            {
                //Loops back to start
                currentDestinationIndicator = 0;
            }
            timeStamp = Time.time;
            agent.destination = destinationList[currentDestinationIndicator];
        }
    }
}
