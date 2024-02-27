using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LazerEmitter : MonoBehaviour
{
    bool hitSomething = false;
    Vector3 hitPos;

    public bool isHeldDown = false;
    LazerReciever reciever;



    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
    void FixedUpdate()
    {
        ///     Bit shift the index of the layer (8) to get a bit mask
        /// int layerMask = 1 << 8;
        ///     This would cast rays only against colliders in layer 8.
        ///     But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        /// layerMask = ~layerMask;
        ///     I dont understand any of this, so we wont use it.
        /// 

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 100
            /*, layerMask*/
            ))
        {
            hitSomething = true;
            hitPos = hit.point;
            reciever = hit.transform.gameObject.GetComponent<LazerReciever>();

            //Debug.Log($"Did Hit {hit.point} {hit.collider.gameObject.name}");
        }
        else
        {
            hitSomething = false;
            hitPos = Vector3.zero;
            reciever = null;

            //Debug.Log("Did not Hit");
        }
    }

    public void Activate()
    {
        isHeldDown = true;
        //Debug.Log("LazerActivation");
        if (isHeldDown && reciever!=null)
        {
            reciever.Activate();
        }
    }

    public void Deactivate()
    {
        //Debug.Log("LazerDeactivation");
        isHeldDown = false;
    }



    private void OnDrawGizmos()
    {
        if (isHeldDown)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(transform.position, transform.TransformDirection(Vector3.forward) * 100);

        if ( hitSomething )
        {
            Gizmos.DrawSphere(hitPos, 1);
        }
    }
}
