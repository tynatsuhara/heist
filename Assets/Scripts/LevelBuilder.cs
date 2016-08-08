using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {

	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;

	public void Build(int width, int height) {
		for (int i = 0; i < width; i++) {
			Quaternion spawnRotation = Quaternion.identity;
			spawnRotation.eulerAngles = spawnRotation.eulerAngles + new Vector3(0, 90, 0);
			Instantiate(wallPrefab, new Vector3(i * 2, -.1f, 0), spawnRotation);
		}
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				// multiply by 2 because the size of a floor panel is 20x20 .1 voxels
				Instantiate(floorPrefab, new Vector3(i * 2, -.1f, j * 2), Quaternion.identity);
			}
		}
	}
}
