using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationSounds : MonoBehaviour
{
    [SerializeField]
    private AudioClip skateSound, brakeSound;

    [SerializeField]
    private float moveThreshold;

    private AudioSource continousPlayer, singlePlayer;

    PlayerControl playerControl;
    Rigidbody rb;
    bool canBreak;

    private void Start()
    {
        singlePlayer = GetComponent<AudioSource>();
        playerControl = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
        canBreak = true;
    }

    private void Update()
    {
        //Check to make sure the player is moving while this check occurs and they won't just repeatedly spam the sound in place.
        if (rb.velocity.magnitude > moveThreshold)
        {
            if (Vector2.Angle(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(playerControl.InputAngle.x, playerControl.InputAngle.z)) > 120 && canBreak)
            {
                singlePlayer.clip = brakeSound;
                singlePlayer.Play();
                canBreak = false;
            }
            if (Vector2.Angle(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(playerControl.InputAngle.x, playerControl.InputAngle.z)) < 80 && !canBreak)
            {
                canBreak = true;
            }
        }

        //Controls the volume and makes sure that the skating sound is only playing while in motion.

    }
}
