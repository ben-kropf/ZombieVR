using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {


Camera lookAtCamera = null; 

void Update (){
	if(lookAtCamera){
		Vector3 v  = lookAtCamera.transform.position - transform.position;
		v.x = v.z = 0.0f;
		transform.LookAt (lookAtCamera.transform.position - v);
	}
}
}