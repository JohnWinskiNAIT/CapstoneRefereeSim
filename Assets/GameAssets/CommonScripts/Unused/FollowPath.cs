using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    //This is a script defining the path, and generically moves an object forward on the path.
    public PathWaypoint[] path = new PathWaypoint[0];
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFollowingPath(path));
    }

    // Update is called once per frame
    private IEnumerator StartFollowingPath(PathWaypoint[] pathToFollow)
    {
        int i = 0;
        
        foreach (var point in pathToFollow)
        {
            var distance = Vector3.Distance(transform.position, point.waypointPosition);
            while (Vector3.Distance(a: transform.position, b: point.waypointPosition) > 0.1f)
            {
                //transform.position = Vector3.MoveTowards(current: transform.position, target: point.waypointPosition, maxDistanceDelta: Time.deltaTime
                //    * (1 * path[i].timeAtWaypoint));

                transform.position = Vector3.MoveTowards(current: transform.position, target: point.waypointPosition, maxDistanceDelta: Time.deltaTime
                     * distance / path[i].timeAtWaypoint * (i+1) );
                
                yield return new WaitForEndOfFrame();
            }

            i++;
        }
    }
}

[Serializable]
public class PathWaypoint
{
    public Vector3 waypointPosition;
    public float timeAtWaypoint;


}
