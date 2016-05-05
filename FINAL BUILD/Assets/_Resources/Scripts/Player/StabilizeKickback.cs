using UnityEngine;
using System.Collections;

public class StabilizeKickback : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public float returnSpeed = 2.0f;
    public Transform myTransform;

    void LateUpdate()
    {
        myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
    }
}