using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationSounds : MonoBehaviour
{
    [SerializeField]
    private AudioClip skateSound, brakeSound;

    [SerializeField]
    private float brakeThreshold;

    private AudioSource audioPlayer;

    PlayerControl playerControl;
    Rigidbody rb;
    bool canBreak;

    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        playerControl = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
        canBreak = true;
    }

    private void Update()
    {
        if (rb.velocity.magnitude > brakeThreshold)
        {
            if (Vector2.Angle(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(playerControl.inputAngle.x, playerControl.inputAngle.z)) > 120 && canBreak)
            {
                audioPlayer.clip = brakeSound;
                audioPlayer.Play();
                canBreak = false;
            }
            if (Vector2.Angle(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(playerControl.inputAngle.x, playerControl.inputAngle.z)) < 80 && !canBreak)
            {
                canBreak = true;
            }
        }
    }
}
