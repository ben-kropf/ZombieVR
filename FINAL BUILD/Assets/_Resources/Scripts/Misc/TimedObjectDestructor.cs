using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour {

// FPS KIT [www.armedunity.com]

public float timeOut = 1.0f;
public bool detachChildren = false;

void Awake (){
	Invoke ("DestroyNow", timeOut);
}

void DestroyNow (){
	if (detachChildren) 
		transform.DetachChildren ();
	
	Destroy(gameObject);
}
}