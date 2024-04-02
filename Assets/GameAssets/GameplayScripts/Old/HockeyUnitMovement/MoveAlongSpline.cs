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
    Vector3 distanceVector;
    Vector3 otherObjectPosition;
    Vector3 distanceVectorNormalized;
    // Start is called before the first frame update
    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    // Update is called once per frame
    void Update()
    {
        #region Moving along the spline
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }
        #endregion
        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);

        #region offset shenanigans
        //if (otherObjectReference != null)
        //{
        //    otherObjectPosition = otherObjectReference.transform.position;
        //    distanceVector = childObject.transform.position - otherObjectPosition;
        //    distanceVectorNormalized = distanceVector.normalized;
        //    OffsettObject(otherObjectPosition);
        //}
        #endregion
    }
    void OffsettObject(Vector3 other)
    {
        //childObject.transform.localPosition = distanceVector * (3.75f -distanceVector.magnitude);
        //childObject.transform.localPosition = distanceVectorNormalized * (5 / ( distanceVector.magnitude));
        childObject.transform.localPosition = distanceVectorNormalized * (distanceVector.magnitude / (distanceVector.magnitude * distanceVector.magnitude));

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
