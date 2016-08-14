using UnityEngine;
using System.Collections;

public abstract class Room : MonoBehaviour {

	public int spawnPriority;  // 0 is the best priority, 1 is worse, etc

	public int id;
	public int widthMin;
	public int widthMax;
	public int width;
	public int heightMin;
	public int heightMax;
	public int height;
	public int offset;
	
	public int rightDoorIndex = -1;
	public int leftDoorIndex = -1;
	public int topDoorIndex = -1;
	public int bottomDoorIndex = -1;

	public abstract bool CanAddBottomDoor();
	public abstract Door AddBottomDoor();

	public abstract bool CanAddTopDoor();
	public abstract Door AddTopDoor();

	public abstract bool CanAddRightDoor();
	public abstract Door AddRightDoor();

	public abstract bool CanAddLeftDoor();
	public abstract Door AddLeftDoor();

	void Awake() {
		width = Random.Range(widthMin, widthMax + 1);
		height = Random.Range(heightMin, heightMax + 1);
	}

	public void Build() {
		BuildFloor();
		BuildWalls();
	}

	public void SetId(int id) {
		this.id = id;
		offset = id * LevelBuilder.CHUNK_SIZE + LevelBuilder.CHUNK_PADDING;
	}

	private void BuildFloor() {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				// multiply by 2 because the size of a floor panel is 20x20 .1 voxels
				Instantiate(LevelBuilder.instance.floorPrefab, new Vector3(offset + i * 2, -.1f, j * 2), Quaternion.identity);
			}
		}
	}


	private void BuildWalls() {
		
		// Corners
		Instantiate(LevelBuilder.instance.wallCornerPrefab, new Vector3(offset + -.15f, 1.1f, .7f), Quaternion.identity);
		Instantiate(LevelBuilder.instance.wallCornerPrefab, new Vector3(offset + .15f + width * 2, 1.1f, .7f), Quaternion.identity);
		Instantiate(LevelBuilder.instance.wallCornerPrefab, new Vector3(offset + .15f + width * 2, 1.1f, 1f + height * 2), Quaternion.identity);
		Instantiate(LevelBuilder.instance.wallCornerPrefab, new Vector3(offset + -.15f, 1.1f, 1f + height * 2), Quaternion.identity);

		// bottom walls
		Quaternion wallSpawnRotation = Quaternion.identity;
		wallSpawnRotation.eulerAngles = wallSpawnRotation.eulerAngles + new Vector3(0, 90, 0);
		for (int i = 0; i < width; i++) {
			if (i != bottomDoorIndex)
				Instantiate(LevelBuilder.instance.wallPrefab, new Vector3(offset + i * 2 + 1, 1.1f, -.15f), wallSpawnRotation);
		}

		// left walls
		for (int j = 0; j < height; j++) {
			if (j != leftDoorIndex)
				Instantiate(LevelBuilder.instance.wallPrefab, new Vector3(offset + -.15f, 1.1f, 2 * j + 1f), Quaternion.identity);
		}

		// right walls
		for (int j = 0; j < height; j++) {
			if (j != rightDoorIndex)
				Instantiate(LevelBuilder.instance.wallPrefab, new Vector3(offset + .15f + width * 2, 1.1f, 2 * j + 1f), Quaternion.identity);
		}

		// top walls
		for (int i = 0; i < width; i++) {
			if (i != topDoorIndex)
				Instantiate(LevelBuilder.instance.wallPrefab, new Vector3(offset + i * 2 + 1, 1.1f, height * 2 + .15f), wallSpawnRotation);
		}
	}
}
