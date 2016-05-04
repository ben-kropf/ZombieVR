using UnityEngine;
using System.Collections;

public class HitSounds : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public float vel = 1.0f;
    public AudioSource aSource;
    public Rigidbody rb;

    void OnCollisionEnter(Collision col)
    {
        vel = rb.velocity.magnitude;
        if (vel > 1)
        {
            aSource.volume = vel / 10;
            aSource.Play();
        }
    }
}