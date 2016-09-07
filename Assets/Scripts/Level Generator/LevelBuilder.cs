using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	public static LevelBuilder instance;

	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject wallCornerPrefab;
	public GameObject doorPrefab;

	public GameObject[] roomPrefabs;
	public int[] roomSpawnAmount;

	public const int CHUNK_SIZE = 120;
	public const int CHUNK_PADDING = 50;

	public void Awake() {
		instance = this;
	}

	public void BuildLevel() {
		List<Room> rooms = spawnRoomsFromArrayCounts();

		/*
		 * Steps:
		 * 1. Spawn rooms in grid, 1x1 size
		 * 2. Put rooms in area grid
		 * 3. Stretch/shrink and verify all rooms are within valid size limits
		 * 4. Maybe make rooms encroach on each other
		 */

//		rooms.ForEach(x => x.Build());
	}

	private List<Room> spawnRoomsFromArrayCounts() {
		List<Room> rooms = new List<Room>();
		for (int i = 0; i < roomSpawnAmount.Length; i++) {
			for (int j = 0; j < roomSpawnAmount[i]; j++) {
				if (roomPrefabs[i] != null) {
					// spawn the room?
//					rooms.Add(Instantiate(roomPrefabs[i]).GetComponent<Room>();
				}
			}
		}
		return rooms.OrderBy(x => x.spawnPriority).ToList();
	}
}
