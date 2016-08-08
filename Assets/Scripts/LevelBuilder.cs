using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {

	public GameObject floorPrefab;

	public void Build() {
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				Instantiate(floorPrefab, new Vector3(i * 2, -.1f, j * 2), Quaternion.identity);
			}
		}
	}
}
