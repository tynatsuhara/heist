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
		GameObject roomsContainer = new GameObject();
		roomsContainer.name = "Rooms";

		List<Room> rooms = new List<Room>();
		List<Room> spawnedRooms = new List<Room>();
		for (int i = 0; i < roomSpawnAmount.Length; i++) {
			for (int j = 0; j < roomSpawnAmount[i]; j++) {
				if (roomPrefabs[i] != null) {
					// spawn the room
					GameObject go = Instantiate(roomPrefabs[i]) as GameObject;
					go.transform.parent = roomsContainer.transform;
					rooms.Add(go.GetComponent<Room>());
				}
			}
		}
			
		rooms = rooms.OrderBy(x => x.spawnPriority).ToList();
		for (int i = 0; i < rooms.Count; i++) {
			rooms[i].SetId(i);
		}

		// Instantiate the starting room
		spawnedRooms.Add(rooms[0]);


		// Rooms are now spawned in order of increasing priority
		List<Room> roomsToSpawn = rooms.ToList();
		roomsToSpawn.RemoveAt(0);
		while (roomsToSpawn.Count > 0) {
			int priority = roomsToSpawn[0].spawnPriority;

			// this chunk of priority rooms (ie all the rooms with priority == 2)
			List<Room> priorityRooms = roomsToSpawn.Where(x => x.spawnPriority == priority).ToList();

			// randomize order of spawning
			for (int i = 0; i < priorityRooms.Count; i++) {
				int newIndex = Random.Range(0, priorityRooms.Count);
				Room temp = priorityRooms[i];
				priorityRooms[i] = priorityRooms[newIndex];
				priorityRooms[newIndex] = temp;
			}

			// each room to spawn
			foreach (Room r in priorityRooms) {
				bool attachedRoom = false;
				List<Room> lookedAtRooms = new List<Room>();
				while (!attachedRoom) {

					// Grab a random room and try to attach to it
					Room attachTo = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
					if (lookedAtRooms.Contains(attachTo))
						continue;
					lookedAtRooms.Add(attachTo);

					// try each direction in a random order
					List<int> dirs = new int[]{ 0, 1, 2, 3 }.ToList();
					while (dirs.Count > 0 && !attachedRoom) {
						int removeIndex = Random.Range(0, dirs.Count);
						int dir = dirs[removeIndex];
						dirs.RemoveAt(removeIndex);

						if (dir == 0 && attachTo.CanAddTopDoor() && r.CanAddBottomDoor()) {
							attachedRoom = true;
							Door d1 = attachTo.AddTopDoor();
							Door d2 = r.AddBottomDoor();
							d1.Mirror(d2);
						}

						if (dir == 1 && attachTo.CanAddBottomDoor() && r.CanAddTopDoor()) {
							attachedRoom = true;
							Door d1 = attachTo.AddBottomDoor();
							Door d2 = r.AddTopDoor();
							d1.Mirror(d2);
						}

						if (dir == 2 && attachTo.CanAddLeftDoor() && r.CanAddRightDoor()) {
							attachedRoom = true;
							Door d1 = attachTo.AddLeftDoor();
							Door d2 = r.AddRightDoor();
							d1.Mirror(d2);
						}

						if (dir == 3 && attachTo.CanAddRightDoor() && r.CanAddLeftDoor()) {
							attachedRoom = true;
							Door d1 = attachTo.AddRightDoor();
							Door d2 = r.AddLeftDoor();
							d1.Mirror(d2);
						}
					}
				}
				spawnedRooms.Add(r);
			}

			// Remove these rooms
			roomsToSpawn = roomsToSpawn.Where(x => x.spawnPriority > priority).ToList();
		}
			
		rooms.ForEach(x => x.Build());
	}
}
