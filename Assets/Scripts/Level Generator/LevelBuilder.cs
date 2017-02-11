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
	private PicaVoxel.Volume[,] floorTiles;

	public const int CHUNK_SIZE = 120;
	public const int CHUNK_PADDING = 50;
	public const int TILE_GRID_LENGTH = 10;
	public const int TILE_SIZE = 2;  // in-game units

	public void Awake() {
		instance = this;
		floorTiles = new PicaVoxel.Volume[TILE_GRID_LENGTH, TILE_GRID_LENGTH];
	}

	public void BuildLevel() {

		// TEMP
		GameObject floorHolder = new GameObject();
		floorHolder.name = "Floor";
		for (int i = 0; i < TILE_GRID_LENGTH; i++) {
			for (var j = 0; j < TILE_GRID_LENGTH; j++) {
				GameObject tile = Instantiate(floorPrefab, new Vector3(i * TILE_SIZE, -.2f, j * TILE_SIZE), 
											  Quaternion.identity) as GameObject;
				tile.transform.parent = floorHolder.transform;
				floorTiles[i, j] = tile.GetComponent<PicaVoxel.Volume>();
			}
		}


		// List<Room> rooms = spawnRoomsFromArrayCounts();

		/*
		 * Steps:
		 * 1. Spawn rooms in grid, 1x1 size
		 * 2. Put rooms in area grid
		 * 3. Stretch/shrink and verify all rooms are within valid size limits
		 * 4. Maybe make rooms encroach on each other
		 */

//		rooms.ForEach(x => x.Build());
	}

	public PicaVoxel.Volume FloorTileAt(Vector3 pos) {
		int x = (int)(pos.x / TILE_SIZE);
		int z = (int)(pos.z /  TILE_SIZE);
		if (x >= TILE_GRID_LENGTH || z >= TILE_GRID_LENGTH || x < 0 || z < 0)
			return null;
		return floorTiles[x, z];
	}

	public Floor FloorAt(Vector3 pos) {
		PicaVoxel.Volume f = FloorTileAt(pos);
		return f == null ? null : f.GetComponent<Floor>();
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
