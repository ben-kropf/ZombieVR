using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;
	private Maze mazeInstance;
	public GameObject playerPrefab;
    private GameObject playerInstance;
    public GameObject[] weapons;
    public GameObject weaponPrefab;

	private void Start () {
		BeginGame();
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			RestartGame();
		}
	}

	private void BeginGame () {
		print("New game.\n");
		mazeInstance = Instantiate(mazePrefab) as Maze;
		mazeInstance.Generate();
		mazeInstance.transform.localScale = new Vector3(4.0f,4.0f,4.0f);
		SpawnWeapons();
		

        //playerInstance = Instantiate(playerPrefab, new Vector3(0.5f, 0.5f, 0.0f), transform.rotation) as GameObject;
        Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		//Destroy(playerInstance.gameObject);
		
		BeginGame();
	}

	// Spawns one of each weapon randomly in the game.
	private void SpawnWeapons () {
		weapons = GameObject.FindGameObjectsWithTag("Weapon");
		print(weapons.Length + " weapons spawned!\n");

        foreach (GameObject weapon in weapons) {
        	weapon.transform.position = new Vector3(Random.Range(-35.0F, 35.0F), 2, Random.Range(-35.0F, 35.0F));
        }
	}
}