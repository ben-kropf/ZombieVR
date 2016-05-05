using UnityEngine;
using System.Collections;

public class ObjectDamage : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public Target mainDamageReceiver;
    public float multiplier;
    public bool head = false;

    public void ApplyDamage(float hPoints)
    {
        mainDamageReceiver.FinalDamage(hPoints * multiplier, head);
    }
}