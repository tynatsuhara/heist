using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Data structure representing a 2d grid in the world,
 * infinitely extending in x and y dimensions
 */
public class RoomGrid {
	private Dictionary<int, Dictionary<int, Room>> map;

	public RoomGrid() {
		map = new Dictionary<int, Dictionary<int, Room>>();
	}
	
	/**
	 * Returns the Room at [x, y] if it exists, otherwise null
	 */
	private Room Get(int x, int y) {
		if (!map.ContainsKey(x)) {
			return null;
		}
		Dictionary<int, Room> xMap = map[x];
		if (!xMap.ContainsKey(y)) {
			return null;
		}
		return xMap[y];
	}

	/**
	 * Returns the Room at [x, y] if it exists, otherwise null
	 */
	private Room Add(Room r, int x, int y) {
		if (!map.ContainsKey(x)) {
			map[x] = new Dictionary<int, Room>();
		}
		Room returnVal = null;
		if (map[x].ContainsKey(y)) {
			returnVal = map[x][y];
		}
		map[x][y] = r;
		return returnVal;
	}

	private Vector2 FindRoom(Room r) {
		return Vector2.zero;
	}
}
