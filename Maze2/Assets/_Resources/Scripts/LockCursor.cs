using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour {

// FPS KIT [www.armedunity.com]

void Start (){
	Application.targetFrameRate = 120;

#if UNITY_EDITOR
	Cursor.visible = false;
	Cursor.lockState = CursorLockMode.Locked;
#else	
	Cursor.visible = false;
	Cursor.lockState = CursorLockMode.Locked;
#endif
}

void OnGUI (){

	 if (Event.current.type == EventType.KeyDown){
		/*
		if (Event.current.keyCode == KeyCode.P){
			Screen.SetResolution (1920, 1080, true);
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		*/	
        if (Event.current.keyCode == KeyCode.Escape){
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}	
		
	if(Cursor.lockState == CursorLockMode.None){
		if(GUI.Button( new Rect(Screen.width -120, 20, 100, 30), "Lock Cursor" )){
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}	
	}
}
	 
		 
}