using UnityEngine;
using System.Collections;

public class FlashlightToggle : MonoBehaviour {

	public GameObject flashlight;

	public AudioSource off;
	public AudioSource on;

	// Update is called once per frame
	void Update () {

		// If pressed f to toggle flashlight on or off
		if (Input.GetKeyDown ("f")) {
			if (flashlight.activeSelf == true) {
				flashlight.SetActive (false);
				off.Play ();
			} else {
				flashlight.SetActive (true);
				on.Play ();
			}
		}
	}
}
