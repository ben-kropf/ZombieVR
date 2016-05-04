using UnityEngine;
using System.Collections;

public class PoolTrigger : MonoBehaviour {

// FPS KIT [www.armedunity.com]

void OnTriggerEnter ( Collider other  ){
	if(other.CompareTag("Player"))
		other.SendMessage("PlayerInWater", transform.position.y, SendMessageOptions.DontRequireReceiver);
}
}