using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PuckFollowPlayer : MonoBehaviour
{
    float timer; float positveNegativeMovement = 1, speed;
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    void Start()
    {
       player = GetComponentInParent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > Time.time + 2)
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

        transform.Translate(new Vector3(positveNegativeMovement * speed * Time.deltaTime,transform.position.y,transform.position.z));
    }
}
