using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Vector2[] path = new Vector2[0];
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFollowingPath(path));
    }

    // Update is called once per frame
    private IEnumerator StartFollowingPath(Vector2[] pathToFollow)
    {
        foreach (var point in  pathToFollow)
        {
            while (Vector2.Distance(a: transform.position, b: point) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(current: transform.position, target: point, maxDistanceDelta: Time.deltaTime * 1);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
