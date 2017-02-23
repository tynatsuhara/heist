using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour {

	public GameObject[] enemyPrefabs;
	public GameObject[] enemyVehicles;
	public float spawnRate;

	private List<SpawnPoint> enemySpawnPoints;

	void Start() {
		enemySpawnPoints = Object.FindObjectsOfType<SpawnPoint>().Where(x => x.enemies).ToList();
	}

	public void StartSpawning() {
		StartCoroutine("SpawnEnemy");
	}

	private IEnumerator SpawnEnemy() {
		while (true) {
			GameObject enemy = (GameObject) Instantiate(enemyPrefabs[0], 
					RandomPoint(enemySpawnPoints), Quaternion.identity);
			GameManager.characters.Add(enemy.GetComponent<NPC>());
			yield return new WaitForSeconds(spawnRate);
		}
	}
	
	void Stop() {
		StopAllCoroutines();
	}

	private Vector3 RandomPoint(List<SpawnPoint> lst) {
		return lst[Random.Range(0, lst.Count)].transform.position;
	}
}
