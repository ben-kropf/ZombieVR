using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;

	public Player playerPrefab;

	private Maze mazeInstance;

	private Player playerInstance;

    public GameObject player;

	private void Start () {
		BeginGame();
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			RestartGame();
		}
	}

	private void BeginGame () {
		Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
		mazeInstance = Instantiate(mazePrefab) as Maze;
		mazeInstance.Generate();
		mazeInstance.transform.localScale = new Vector3(4.0f,4.0f,4.0f);
        Instantiate(player, new Vector3(0.5f, 0.5f, 0.0f), transform.rotation);
		//playerInstance.SetLocation(mazeInstance.GetCell(mazeInstance.RandomCoordinates));
		//Camera.main.clearFlags = CameraClearFlags.Depth;
		//Camera.main.rect = new Rect(0f, 0f, 0.5f, 0.5f);

	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		if (playerInstance != null) {
			Destroy(playerInstance.gameObject);
		}
		BeginGame();
	}
}