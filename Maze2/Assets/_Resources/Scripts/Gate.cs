using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{

    //  FPS Kit [www.armedunity.com]

    public Animation gateAnims;
    public AudioSource aSource;
    public AudioClip[] sounds;
    public string[] anims = new string[] { "OpenGate", "CloseGate" };
    private int state = 0;
    private bool inTransition = false;
    public GameObject map;

    IEnumerator Action()
    {
        if (inTransition) yield break;
        inTransition = true;

        if (!map.activeSelf)
            map.SetActive(true);

        aSource.clip = sounds[state];
        aSource.Play();
        gateAnims.Play(anims[state]);
        state = System.Convert.ToBoolean(state) ? 0 : 1;
        yield return new WaitForSeconds(1.5f);

        inTransition = false;
    }
}