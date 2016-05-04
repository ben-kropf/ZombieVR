using UnityEngine;
using System.Collections;

public class RotorHit : MonoBehaviour {


public GameObject hitPrefab;	

void OnTriggerEnter (){
	Instantiate( hitPrefab, transform.position, transform.rotation );
}

void OnCollisionEnter ( Collision col  ){
    ContactPoint contact = col.contacts[0];
    Instantiate( hitPrefab, contact.point, Quaternion.LookRotation(contact.normal)); 
}
}