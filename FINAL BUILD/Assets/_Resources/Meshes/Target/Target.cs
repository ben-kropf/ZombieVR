using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    private float hitPoints = 100;
    [HideInInspector]
    public float baseHitPoints = 100;
    private Quaternion targetrotation;
    private float targetAngle;
    private float smooth = 10.0f;
    private bool showTarget;
    public Transform pivot;
    public ScoreManager scoreManager;
    public TargetManager targetManager;
    public float resetTime = 5.0f;
    [HideInInspector]
    public bool trainingMode;

    public void FinalDamage(float damage, bool head)
    {
        if (hitPoints < 0.0f) return;

        if (showTarget)
        {
            int score = 100;

            if (damage >= hitPoints && head)
                score = 250;

            StartCoroutine(scoreManager.DrawCrosshair());
            hitPoints -= damage;

            if (hitPoints <= 0)
            {
                StartCoroutine(TargetDown());
                scoreManager.addScore(score);
                if (targetManager.state == 2)
                    targetManager.SetScore(score, head);
            }
        }
    }

    public IEnumerator TargetDown()
    {
        showTarget = false;

        targetAngle = 0.0f;
        targetrotation = Quaternion.Euler(0, targetAngle, 0);
        while (pivot.localRotation != targetrotation)
        {
            pivot.localRotation = Quaternion.Slerp(pivot.localRotation, targetrotation, Time.deltaTime * smooth);
            yield return null;
        }

        if (!trainingMode)
            StartCoroutine(ResetTarget());
        else
            if (targetManager.state == 2)
            targetManager.NextTarget();
    }

    public IEnumerator TargetUp()
    {
        showTarget = true;
        targetAngle = -90.0f;
        targetrotation = Quaternion.Euler(0, targetAngle, 0);

        while (pivot.localRotation != targetrotation)
        {
            pivot.localRotation = Quaternion.Slerp(pivot.localRotation, targetrotation, Time.deltaTime * smooth);
            yield return null;
        }
        hitPoints = baseHitPoints;
    }

    IEnumerator ResetTarget()
    {
        yield return new WaitForSeconds(resetTime);
        if (!trainingMode)
			StartCoroutine(TargetUp());	
    }


}