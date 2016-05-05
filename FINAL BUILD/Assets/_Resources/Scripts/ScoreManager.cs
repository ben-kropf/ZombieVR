using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public int currentScore = 0;
    public Texture hitCrosshairTexture;
    private float alphaHit;
    public AudioClip hitSound;

    public GUISkin mySkin;

    public int pointsToNextRank = 50;
    public int hits = 0;
    public AudioClip levelUpSound;
    private bool playerDead = false;
    public AudioSource aSource;

    void Update()
    {
        if (alphaHit > 0)
            alphaHit -= Time.deltaTime;
    }

    public IEnumerator DrawCrosshair()
    {
        yield return new WaitForSeconds(0.05f);
        alphaHit = 1.0f;
        aSource.PlayOneShot(hitSound, 0.2f);
    }

    public void addScore(int val)
    {
        currentScore += val;

        if (currentScore >= 4)
        {
            aSource.PlayOneShot(levelUpSound, 0.2f);
            pointsToNextRank += currentScore;
        }
    }

    public void addHits(int val) {
        hits += val;
    }

    public void PlayerDead()
    {
        playerDead = true;
    }

    void OnGUI()
    {
        if (playerDead) return;

        GUI.skin = mySkin;
        GUI.depth = 2;

        GUI.Label(new Rect(40, Screen.height - 80, 100, 60), " CORES :");
        GUI.Label(new Rect(100, Screen.height - 80, 160, 60), "" + currentScore + "/4", mySkin.customStyles[0]);

        GUI.Label(new Rect(40, Screen.height - 140, 100, 60), " HITS :");
        GUI.Label(new Rect(100, Screen.height - 140, 160, 60), "" + hits, mySkin.customStyles[0]);

        GUI.color = new Color(1.0f, 1.0f, 1.0f, alphaHit);
        GUI.DrawTexture(new Rect((Screen.width / 2) - 16, (Screen.height / 2) - 16, 32, 32), hitCrosshairTexture);
    }
}