using UnityEngine;
using System.Collections;
// FPS KIT [www.armedunity.com]

public class TargetManager : MonoBehaviour {

    public Target[] allTargets;
    public float hitpoints = 100;

    private float timer = 0.0f;
    public float duration = 30.0f;
    private int trainingScore = 0;
    private int kills = 0;
    private int headshots = 0;
    [HideInInspector]
    public int state = 0;

    public AudioSource aSource;
    public AudioClip countdownSound;
    public GUISkin mySkin;

    void Start()
    {
        state = 0;

        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].baseHitPoints = hitpoints;
            allTargets[i].trainingMode = false;
            StartCoroutine(allTargets[i].TargetUp());
        }
    }

    public void NextTarget()
    {
        StartCoroutine(allTargets[Random.Range(0, allTargets.Length)].TargetUp());
    }

    void StartTraining()
    {

        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].baseHitPoints = hitpoints;
            allTargets[i].trainingMode = true;
            StartCoroutine(allTargets[i].TargetDown());
        }

        trainingScore = 0;
        headshots = 0;
        kills = 0;
        timer = duration;
        state = 1;
        StartCoroutine(Ready());
    }

    IEnumerator Ready()
    {
        aSource.PlayOneShot(countdownSound, 0.5f);
        yield return new WaitForSeconds(6.0f);
        state = 2;
        NextTarget();
    }

    void Update()
    {
        if (state == 2)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
               StartCoroutine(TrainingEnds());
            }
        }
    }

    public void SetScore(int s, bool hs)
    {
        trainingScore += s;
        kills++;
        if (hs) headshots++;
    }

    IEnumerator TrainingEnds()
    {
        state = 3;
        yield return new WaitForSeconds(10.0f);
        state = 0;
        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].baseHitPoints = hitpoints;
            allTargets[i].trainingMode = false;
            StartCoroutine(allTargets[i].TargetUp());
        }
    }

    void OnGUI()
    {
        if (state == 1 || state == 2)
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height - 50, 100, 60), "<color=#88FF6AFF>TIME LEFT  </color>" + FormatSeconds(timer), mySkin.customStyles[0]);
        if (state == 3)
        {
            GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 150, 100, 60), "<color=#88FF6AFF>SCORE :  </color>" + trainingScore, mySkin.customStyles[0]);
            GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 120, 100, 60), "<color=#88FF6AFF>KILLS :  </color>" + kills, mySkin.customStyles[0]);
            GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 60), "<color=#88FF6AFF>HEADSHOTS :  </color>" + headshots, mySkin.customStyles[0]);
        }
    }

    string FormatSeconds(float elapsed)
    {
        int d = (int)(elapsed * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    void Action()
    {
        if (state == 0) StartTraining();
    }
}