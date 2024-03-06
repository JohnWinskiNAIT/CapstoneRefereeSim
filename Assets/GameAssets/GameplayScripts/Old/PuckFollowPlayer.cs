using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PuckFollowPlayer : MonoBehaviour
{
    float timer; float positveNegativeMovement = 0.5f, speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > Time.time + 0.5)
        {
            if (positveNegativeMovement > 0)
            {
                positveNegativeMovement = -1;

            }
            else
            {
                positveNegativeMovement = 1;
            }
            timer = Time.time;
        }

        transform.Translate(new Vector3(positveNegativeMovement * speed * Time.deltaTime,0,0));
    }
}
