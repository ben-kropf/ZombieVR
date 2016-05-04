using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;
	private Maze mazeInstance;
	public GameObject playerPrefab;
    private GameObject playerInstance;

	private void Start () {
		BeginGame();
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			RestartGame();
		}
	}

	private void BeginGame () {
		mazeInstance = Instantiate(mazePrefab) as Maze;
		mazeInstance.Generate();
		mazeInstance.transform.localScale = new Vector3(4.0f,4.0f,4.0f);

        playerInstance = Instantiate(playerPrefab, new Vector3(0.5f, 0.5f, 0.0f), transform.rotation) as GameObject;
        Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		Destroy(playerInstance.gameObject);
		
		BeginGame();
	}
}