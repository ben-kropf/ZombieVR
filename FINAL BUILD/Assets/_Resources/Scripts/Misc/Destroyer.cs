using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public ParticleEmitter e;

    public void DestroyNow()
    {
        e.emit = false;
        Destroy(gameObject, 5.0f);
    }
}