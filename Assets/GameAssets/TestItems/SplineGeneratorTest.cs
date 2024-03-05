using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    [SerializeField]
    GameObject testingTheThing;

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

        Vector3 nextPosition = Vector3.zero;

        //this breaks the ability to compile? I don't get it
        nextPosition = new Vector3(Random.Range(splineAreaMinX, splineAreaMaxX), 0, Random.Range(splineAreaMinZ, splineAreaMaxZ));
        splineKnots[0] = new BezierKnot();
        splineKnots[0].Position = nextPosition;
        splineKnots[0].Rotation = Quaternion.identity;

        for (int i = 1; i < pointCount; i++)
        {
            Vector3 lastPosition = nextPosition;

            while ((nextPosition - lastPosition).magnitude < minimumMagnitude)
            {
                nextPosition = new Vector3(Random.Range(splineAreaMinX, splineAreaMaxX), 0, Random.Range(splineAreaMinZ, splineAreaMaxZ));
            }

            splineKnots[i] = new BezierKnot();
            splineKnots[i].Position = nextPosition;
            splineKnots[i].Rotation = Quaternion.identity;
        }

        /*for (int i = 0; i < pointCount; i++)
        {
            while (nextPosition.magnitude < minimumMagnitude)
            {
                nextPosition = new Vector3(Random.Range(splineAreaMinX, splineAreaMaxX), 0, Random.Range(splineAreaMinZ, splineAreaMaxZ));
            }

            splineKnots[i] = new BezierKnot();
            splineKnots[i].Position = nextPosition;
            splineKnots[i].Rotation = Quaternion.identity;
        }*/

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
        //AI will rotate on the Z axis and make unpredictable and weird motions during very sharp or irregular knots.
        //This does a good job of fixing the issue for now until we can identify a potentially more satisfying solution.
        testingTheThing.transform.localRotation = new Quaternion(0, testingTheThing.transform.rotation.y, 0, testingTheThing.transform.rotation.w);
    }
}
