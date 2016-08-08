using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {

	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;

	public void Build(int width, int height) {
		Instantiate(wallCornerPrefab, new Vector3(-.15f, 1.1f, .7f), Quaternion.identity);
		Instantiate(wallCornerPrefab, new Vector3(.15f + width * 2, 1.1f, .7f), Quaternion.identity);
		Instantiate(wallCornerPrefab, new Vector3(.15f + width * 2, 1.1f, 1f + height * 2), Quaternion.identity);
		Instantiate(wallCornerPrefab, new Vector3(-.15f, 1.1f, 1f + height * 2), Quaternion.identity);

		// bottom walls
		for (int i = 0; i < width; i++) {
			Quaternion spawnRotation = Quaternion.identity;
			spawnRotation.eulerAngles = spawnRotation.eulerAngles + new Vector3(0, 90, 0);
			Instantiate(wallPrefab, new Vector3(i * 2 + 1, 1.1f, -.15f), spawnRotation);
		}

		// left walls
		for (int j = 0; j < height; j++) {
			Instantiate(wallPrefab, new Vector3(-.15f, 1.1f, 2 * j + 1f), Quaternion.identity);
		}

		// Floor tiles
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				// multiply by 2 because the size of a floor panel is 20x20 .1 voxels
				Instantiate(floorPrefab, new Vector3(i * 2, -.1f, j * 2), Quaternion.identity);
			}
		}

		// right walls
		for (int j = 0; j < height; j++) {
			Instantiate(wallPrefab, new Vector3(.15f + width * 2, 1.1f, 2 * j + 1f), Quaternion.identity);
		}

		// top walls
		for (int i = 0; i < width; i++) {
			Quaternion spawnRotation = Quaternion.identity;
			spawnRotation.eulerAngles = spawnRotation.eulerAngles + new Vector3(0, 90, 0);
			Instantiate(wallPrefab, new Vector3(i * 2 + 1, 1.1f, height * 2 + .15f), spawnRotation);
		}
	}
}
