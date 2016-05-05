using UnityEngine;
using System.Collections;

public class BreathAnim : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public Animation anim;
    public string breathAnim = "Breath";
    public string idleAnim = "BreathIdle";

    void Update()
    {
        if (!Input.GetButton("Fire2"))
            anim.Play(breathAnim);
        else
            anim.CrossFade(idleAnim);
    }


}