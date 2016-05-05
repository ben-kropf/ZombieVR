using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public float hitPoints;
    public int maxHitPoints;
    public bool regeneration = false;
    public float regenerationSpeed;
    public AudioSource aSource;
    public AudioClip painSound;
    public AudioClip fallDamageSound;
    public Transform deadReplacement;
    public GUISkin mySkin;
    private GameObject radar;
    public Texture damageTexture;
    private float t = 0.0f;
    private float alpha;
    private bool isDead = false;
    private ScoreManager scoreManager;
    public Transform camShake;
	private Vector3 originalPos;

    void Start()
    {
	
		originalPos = camShake.localPosition;
		
        if (regeneration)
            hitPoints = maxHitPoints;
        alpha = 0.0f;
    }

    void Update()
    {
        if (t > 0.0f)
        {
            t -= Time.deltaTime;
            alpha = t;
        }

        if (regeneration)
        {
            if (hitPoints < maxHitPoints)
                hitPoints += Time.deltaTime * regenerationSpeed;
        }
    }

    public void PlayerDamage(int damage)
    {
        if (hitPoints < 0.0f) return;

        hitPoints -= damage;
        aSource.PlayOneShot(painSound, 1.0f);
        t = 2.0f;

        if (hitPoints <= 0.0f) Die();
    }

    //Picking up MedicKit
    public void Medic(int medic)
    {

        hitPoints += medic;

        if (hitPoints > maxHitPoints)
        {
            float convertToScore = hitPoints - maxHitPoints;
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addScore(System.Convert.ToInt32(convertToScore));
            hitPoints = maxHitPoints;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (scoreManager == null)
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        scoreManager.PlayerDead();

        Instantiate(deadReplacement, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    void OnGUI()
    {
        GUI.skin = mySkin;

        GUI.Label(new Rect(40, Screen.height - 50, 60, 60), " Health: ");
        GUI.Label(new Rect(100, Screen.height - 50, 60, 60), "" + hitPoints.ToString("F0"), mySkin.customStyles[0]);


        GUI.color = new Color(1, 1, 1, alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), damageTexture);
    }

    public void PlayerFallDamage(float dam)
    {
        PlayerDamage(System.Convert.ToInt32(dam));
        if (fallDamageSound) aSource.PlayOneShot(fallDamageSound, 1.0f);
    }

	IEnumerator Shake(float p)
    {
		
	
        float t = 1.0f;
        float shakePower;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            shakePower = t / 50;
			
			camShake.localPosition = originalPos + Random.insideUnitSphere * shakePower * 35;
		  
			yield return 0;
        }
    }

}