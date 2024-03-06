using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline : MonoBehaviour
{
    public SplineContainer spline;
    public GameObject childObject;
    public float speed = 1f;
    float distancePercentage = 0f;

    float splineLength;

    GameObject otherObjectReference;
    Vector3 otherObjectPosition;
    Vector3 distanceVector;
    Vector3 distanceVectorNormalized;
    Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    // Update is called once per frame
    void Update()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);


        if (otherObjectReference != null)
        {
            otherObjectPosition = otherObjectReference.transform.position;
            OffsettObject(otherObjectPosition);
        }
    }
    void OffsettObject(Vector3 other)
    {
        Vector3 distanceVector = childObject.transform.position - other;
        Vector3 distanceVectorNormalized = distanceVector.normalized;
        //Vector3 targetPosition += (distanceVectorNormalized);
        childObject.transform.localPosition = distanceVectorNormalized * (3.75f - distanceVector.magnitude);

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
        if(other.gameObject == otherObjectReference)
        {
            otherObjectReference = null;    
        }
    }
}
