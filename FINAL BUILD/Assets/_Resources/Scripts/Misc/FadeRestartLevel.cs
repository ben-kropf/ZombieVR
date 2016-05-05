using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeRestartLevel : MonoBehaviour
{
    // FPS KIT [www.armedunity.com]

    float secBeforeFade = 3.0f;
    float fadeTime = 5.0f;
    public Texture fadeTexture;
    private bool fadeIn = false;
    private float tempTime;
    private float time = 0.0f;
    public AudioClip die;

    IEnumerator Start()
    {
        AudioSource.PlayClipAtPoint(die, transform.position);
        yield return new WaitForSeconds(secBeforeFade);
        fadeIn = true;
    }

    void Update()
    {

        if (fadeIn)
        {
            if (time < fadeTime) time += Time.deltaTime;
            tempTime = Mathf.InverseLerp(0.0f, fadeTime, time);
        }

        if (tempTime >= 1.0f)
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
        }
    }

    void OnGUI()
    {
        if (fadeIn)
        {
            GUI.color = new Color(1, 1, 1, tempTime);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }

}