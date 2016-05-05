using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;
	private Maze mazeInstance;
    private GameObject playerInstance;
    public GameObject[] weapons = new GameObject[5];
    public GameObject[] zombies = new GameObject[6];
    private List<GameObject> activeWeapons = new List<GameObject>();
    private List<GameObject> activeZombies = new List<GameObject>();
    private GameObject[] lights = new GameObject[5];
    public GameObject light;
    private float nextRespawn = 150;
    private float respawnDelay = 60;

    AudioSource fxSound; // Emitir sons
	public AudioClip[] backMusic = new AudioClip[12]; // Som de fundo

	private void Start () {
		fxSound = GetComponent<AudioSource> ();
		fxSound.clip = backMusic[Random.Range(0,11)];
     	fxSound.Play ();
		BeginGame();
	}
	
	private void Update () {
		if (Time.time > nextRespawn) {
			SpawnZombies(5);
	        nextRespawn = Time.time + respawnDelay;
	    }
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
		SpawnZombies(20);

        Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		foreach (GameObject weapon in activeWeapons) {
			Destroy(weapon.gameObject);
		}
		foreach (GameObject zombie in activeZombies) {
			Destroy(zombie.gameObject);
		}
		BeginGame();
	}

	// Spawns one of each weapon randomly in the game.
	private void SpawnWeapons () {
		print("Spawning weapons.");
		int i = 0;
        foreach (GameObject weapon in weapons) {
        	GameObject newWeapon = (GameObject) Instantiate(weapon);
        	activeWeapons.Add(newWeapon);
        	newWeapon.transform.position = new Vector3(Random.Range(-35.0F, 35.0F), 0, Random.Range(-35.0F, 35.0F));
        	
        	GameObject newLight = (GameObject) Instantiate(light);
        	newLight.name = newWeapon.name + " Light";
        	newLight.transform.position = newWeapon.transform.position;
			newLight.transform.parent = newWeapon.transform;
        	
        	i++;
        }
	}

	private void SpawnZombies (int numZombies) {
		print("Spawning zombies.");
		for (int i = 0; i < numZombies; i++) {
			GameObject newZombie = (GameObject) Instantiate(zombies[Random.Range(0,5)]);
			activeZombies.Add(newZombie);
        	newZombie.transform.position = new Vector3(Random.Range(-35.0F, 35.0F), 2, Random.Range(-35.0F, 35.0F));
		}
	}
}