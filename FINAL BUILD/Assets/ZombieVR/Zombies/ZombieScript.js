var target : Transform; //the enemy's target
var moveSpeed = 3; //move speed
var rotationSpeed = 3; //speed of turning

var myTransform : Transform; //current transform data of this enemy
var isNotDead : boolean = true;
var health : float = 100;
function Awake()
{
    myTransform = transform; //cache transform data for easy access/preformance
}

function Start()
{
     target = GameObject.FindWithTag("Player").transform; //target the player

}

function Update () {
	
	if(health < 1){
	
		isNotDead = false;
		GetComponent.<Animation>().Play("die");
		Destroy(gameObject, 1);
	}
	
	if(isNotDead){
	
	    //rotate to look at the player
	    myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
	    Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed*Time.deltaTime);
	
	
	    
	    var distance = Vector3.Distance(target.position, myTransform.position);
	    if (distance < 3.0f) {
	        GetComponent.<Animation>().Play("attack1");
	    }
	    else{   
	    	//move towards the player
	   		myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
	   		GetComponent.<Animation>().Play("walk1");
	    }

	}
}

function ApplyDamage(dmg : float){

	health -= dmg;

}