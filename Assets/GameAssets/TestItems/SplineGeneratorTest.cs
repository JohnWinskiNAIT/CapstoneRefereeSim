using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SplineGeneratorTest : MonoBehaviour
{
    [SerializeField]
    GameObject splineArea;
    [SerializeField]
    int pointCount;
    [SerializeField]
    float minimumMagnitude, maximumMagnitude;
    
    float splineAreaMaxX, splineAreaMinX, splineAreaMaxZ, splineAreaMinZ;

    // Start is called before the first frame update
    void Start()
    {
        splineAreaMaxX = splineArea.transform.position.x + splineArea.transform.localScale.x / 2;
        splineAreaMinX = splineArea.transform.position.x - splineArea.transform.localScale.x / 2;
        splineAreaMaxZ = splineArea.transform.position.z + splineArea.transform.localScale.z / 2;
        splineAreaMinZ = splineArea.transform.position.z - splineArea.transform.localScale.z / 2;

        SplineContainer splineComponent = GetComponent<SplineContainer>();

        GenerateSplinePoints(splineComponent.Spline);

        /*splineComponent.Spline = new Spline();
        splineComponent.Spline.Clear();

        BezierKnot newKnot = new BezierKnot();
        newKnot.Position = Vector3.zero;

        splineComponent.Spline.Add(newKnot);

        newKnot = new BezierKnot();
        newKnot.Position = new Vector3(5, 0, 2);

        splineComponent.Spline.Add(newKnot);
        splineComponent.Spline.Closed = true;*/
    }

    //TODO: Magnitude should be *away from the last point*, not away from the center.
    //Find a way to change it from Bezier to Auto after generation.

    void GenerateSplinePoints(Spline spline)
    {
        BezierKnot[] splineKnots = new BezierKnot[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 nextPosition = Vector3.zero;
            while (nextPosition.magnitude < minimumMagnitude)
            {
                nextPosition = new Vector3(Random.Range(splineAreaMinX, splineAreaMaxX), 0, Random.Range(splineAreaMinZ, splineAreaMaxZ));
            }

            splineKnots[i] = new BezierKnot();
            splineKnots[i].Position = nextPosition;
            splineKnots[i].Rotation = Quaternion.identity;
            //
        }

        spline.Clear();

        for (int i = 0; i < pointCount; i++)
        {
            spline.Add(splineKnots[i]);
        }

        spline.Closed = true;
        spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
