using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {

	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;
	public GameObject doorPrefab;

	public const int CHUNK_SIZE = 120;
	public const int CHUNK_PADDING = 50;

	public void BuildLevel() {
		int roomAmount = 2;
		RoomLocation[] rooms = new RoomLocation[roomAmount];
		for (int i = 0; i < roomAmount; i++) {
			rooms[i] = InstantiateRoom(5, 5, i);
		}
		for (int i = 0; i < roomAmount - 1; i++) {
			LinkRooms(rooms[i], rooms[i + 1]);
		}
		for (int i = 0; i < roomAmount; i++) {
			InstantiateWalls(rooms[i]);
		}
	}

	private void LinkRooms(RoomLocation r1, RoomLocation r2) {
		Door d1 = SpawnRightDoor(r1);
		Door d2 = SpawnLeftDoor(r2);
		d1.mirror = d2;
		d2.mirror = d1;
	}

	// Use this as the map is loaded lazily. We can create
	// objects in code that represent the rooms with diff. values,
	// and then instantiate them when a neighboring room is entered?
	private RoomLocation InstantiateRoom(int width, int height, int id) {
		width = Mathf.Min(20, width);
		height = Mathf.Min(20, height);

		float offset = id * CHUNK_SIZE + CHUNK_PADDING;

		// Floor tiles
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				// multiply by 2 because the size of a floor panel is 20x20 .1 voxels
				Instantiate(floorPrefab, new Vector3(offset + i * 2, -.1f, j * 2), Quaternion.identity);
			}
		}

		return new RoomLocation(id, width, height);
	}

	private void InstantiateWalls(RoomLocation r) {
		int width = r.width;
		int height = r.height;

		float offset = r.id * CHUNK_SIZE + CHUNK_PADDING;

		Instantiate(wallCornerPrefab, new Vector3(offset + -.15f, 1.1f, .7f), Quaternion.identity);
		Instantiate(wallCornerPrefab, new Vector3(offset + .15f + width * 2, 1.1f, .7f), Quaternion.identity);
		Instantiate(wallCornerPrefab, new Vector3(offset + .15f + width * 2, 1.1f, 1f + height * 2), Quaternion.identity);
		Instantiate(wallCornerPrefab, new Vector3(offset + -.15f, 1.1f, 1f + height * 2), Quaternion.identity);

		// bottom walls
		Quaternion wallSpawnRotation = Quaternion.identity;
		wallSpawnRotation.eulerAngles = wallSpawnRotation.eulerAngles + new Vector3(0, 90, 0);
		for (int i = 0; i < width; i++) {
			Instantiate(wallPrefab, new Vector3(offset + i * 2 + 1, 1.1f, -.15f), wallSpawnRotation);
		}

		// left walls
		for (int j = 0; j < height; j++) {
			if (j != r.leftDoorIndex)
				Instantiate(wallPrefab, new Vector3(offset + -.15f, 1.1f, 2 * j + 1f), Quaternion.identity);
		}

		// right walls
		for (int j = 0; j < height; j++) {
			if (j != r.rightDoorIndex)
				Instantiate(wallPrefab, new Vector3(offset + .15f + width * 2, 1.1f, 2 * j + 1f), Quaternion.identity);
		}

		// top walls
		for (int i = 0; i < width; i++) {
			Instantiate(wallPrefab, new Vector3(offset + i * 2 + 1, 1.1f, height * 2 + .15f), wallSpawnRotation);
		}
	}

	private Door SpawnRightDoor(RoomLocation r1) {
		GameObject go = Instantiate(doorPrefab, new Vector3(r1.id * CHUNK_SIZE + CHUNK_PADDING + 2 * r1.width + .3f, 0, 0), 
			                Quaternion.identity) as GameObject;
		go.transform.eulerAngles += Vector3.up * 90f;
		int doorIndex = Random.Range(0, r1.height - 1);
		go.transform.position += new Vector3(0, .05f, doorIndex * 2 + 1.8f);
		r1.rightDoorIndex = doorIndex;
		Door d = go.GetComponent<Door>();
		d.openRightTeleporter.gameObject.SetActive(true);
		return d;
	}

	private Door SpawnLeftDoor(RoomLocation r1) {
		GameObject go = Instantiate(doorPrefab, new Vector3(r1.id * CHUNK_SIZE + CHUNK_PADDING, 0, 0), 
			                Quaternion.identity) as GameObject;
		go.transform.eulerAngles += Vector3.up * 90f;
		int doorIndex = Random.Range(0, r1.height - 1);
		go.transform.position += new Vector3(0, .05f, doorIndex * 2 + 1.8f);
		r1.leftDoorIndex = doorIndex;
		Door d = go.GetComponent<Door>();
		d.openLeftTeleporter.gameObject.SetActive(true);
		return d;
	}

	private class RoomLocation {

		public int rightDoorIndex = -1;
		public int leftDoorIndex = -1; 

		public int id;
		public int width;
		public int height;

		public RoomLocation(int id, int width, int height) {
			this.id = id;
			this.width = width;
			this.height = height;
		}
	}
}
