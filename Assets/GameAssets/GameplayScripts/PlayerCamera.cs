using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    PlayerControl playerControls;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // == ROTATION ==
        //Determine new values to rotate to.
        float xChange = transform.rotation.eulerAngles.x - playerControls.cameraAngle.x;
        float yChange = transform.rotation.eulerAngles.y - playerControls.cameraAngle.y;

        //Debug.Log($"Angle 1: {transform.rotation.eulerAngles.x} Angle 2: {playerControls.cameraAngle.y}");

        //Setting the rotation from euler atm.
        //transform.rotation = Quaternion.Euler(playerControls.cameraAngle.x, playerControls.cameraAngle.y, 0);

        transform.rotation = Quaternion.Euler(playerControls.cameraAngle.x, playerControls.cameraAngle.y, 0);
    }
}
