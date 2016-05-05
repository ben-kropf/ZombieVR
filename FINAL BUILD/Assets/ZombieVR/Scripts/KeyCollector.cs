using UnityEngine;
using System.Collections;

public class KeyCollector : MonoBehaviour {

	public ScoreManager scoreManager;
	public GUISkin mySkin;

	private GameObject player;
	private bool showGui = false;

	// Use this for initialization
	void Start () {
		GameObject[] temp;
		temp = GameObject.FindGameObjectsWithTag("Player");

		if (temp[0] != null)
		{
			player = temp[0];
		}
	}
	
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance(transform.position, player.transform.position);

		if (distance <= 4) {
			showGui = true;

			if (Input.GetKeyDown(KeyCode.E)) {
				scoreManager.addScore (1);
				gameObject.SetActive (false);
			}


		} else {
			showGui = false;
		}
	}

	void OnGUI()
	{
		if (showGui)
		{
			GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Press key <color=#88FF6AFF> << E >> </color> to pick up ambiguous core", mySkin.customStyles[1]);
		}
	}
		
}
