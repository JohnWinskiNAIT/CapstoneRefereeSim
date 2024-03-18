using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPlayer : MonoBehaviour
{
    GameObject otherObjectReference;
    Vector3 otherObjectPosition;
    Vector3 distanceVector;
    Vector3 distanceVectorNormalized;
    Vector3 targetPosition;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (otherObjectReference != null)
        {
            otherObjectPosition = otherObjectReference.transform.position;
            distanceVector = transform.position - otherObjectPosition;
            distanceVectorNormalized = distanceVector.normalized;
            OffsettObject(otherObjectPosition);
        }
    }
    void OffsettObject(Vector3 other)
    {
        transform.localPosition = distanceVectorNormalized * (distanceVector.magnitude / (distanceVector.magnitude * distanceVector.magnitude));

        Debug.Log(distanceVector.magnitude);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        if (other.CompareTag("Player"))
        {
            otherObjectReference = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == otherObjectReference)
        {
            otherObjectReference = null;
        }
    }
}
