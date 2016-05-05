using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour {
	private Transform target;
	public float moveSpeed;
	public float rotationSpeed;
	private Transform myTransform; //cache transform data for easy access/preformanc;
	private bool isNotDead;
	private float hitPoints;
    [HideInInspector]
    public ScoreManager scoreManager;
    private string[] walk = {"walk1", "walk2"};
    private string[] attack = {"attack1", "attack2"};
    private bool seen = false;
    private float distanceToPlayer = 22f;
    public HealthScript hs;
    private bool attacking = false;
    private float nextHitAllowed = 0;
    private float hitDelay = 5f;

	void Start() {
		myTransform = transform;
	    target = GameObject.FindWithTag("Player").transform; //target the player
	    moveSpeed = Random.Range(1f, 5f);
	    rotationSpeed = Random.Range(1f,3f);
	    hitPoints = Random.Range(75f,150f);
	    isNotDead = true;
	    hs = GameObject.Find("Player").GetComponent<HealthScript>();
	    scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
	}

	void Update () {
		Vector3 position = transform.position;
		position.y = 0;
		transform.position = position;

		if (hitPoints < 1){
			isNotDead = false;
			GetComponent<Animation>().Play("die");
			Destroy(gameObject, 1);
		}

		if (Vector3.Distance(myTransform.position, target.transform.position) < distanceToPlayer) seen = true;
		
		if (isNotDead) {
			if (seen) {
			    
			    float distance = Vector3.Distance(target.position, myTransform.position);
			    if (distance < 2f) {
			    	if (Time.time > nextHitAllowed) {
				        GetComponent<Animation>().Play(attack[Random.Range(0, 1)]);
				        hs.PlayerDamage(8);
				        nextHitAllowed = Time.time + hitDelay;
				    }
			    }  else {   
			    	// move towards the player
			    	myTransform.LookAt(target);
			   		myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
			   		GetComponent<Animation>().Play("walk1");
			    }
			}

		}
	}

	void ApplyDamage(float dmg){
        if (hitPoints < 0.0f) return;

        int score = 1;

        if (dmg >= hitPoints)
            score = 10;

        StartCoroutine(scoreManager.DrawCrosshair());
        hitPoints -= dmg;

        if (hitPoints <= 0) {
            scoreManager.addHits(score);
        }
	}
}