using UnityEngine;
using System.Collections;

public class BasicRoom : Room {
	
	public override bool CanAddBottomDoor() {
		return bottomDoorIndex == -1;
	}
	public override Door AddBottomDoor() {
		GameObject go = Instantiate(LevelBuilder.instance.doorPrefab, new Vector3(offset, 0, 0), 
			Quaternion.identity) as GameObject;
		int doorIndex = Random.Range(0, width);
		go.transform.position += new Vector3(doorIndex * 2 + .2f, .05f, 0);
		go.name = "Bottom Door";
		bottomDoorIndex = doorIndex;
		Door d = go.GetComponent<Door>();
		d.openLeftTeleporter.gameObject.SetActive(true);
		return d;
	}

	public override bool CanAddTopDoor() {
		return topDoorIndex == -1;
	}
	public override Door AddTopDoor() {
		GameObject go = Instantiate(LevelBuilder.instance.doorPrefab, new Vector3(offset, 0, height * 2), 
			Quaternion.identity) as GameObject;
		int doorIndex = Random.Range(0, width);
		go.transform.position += new Vector3(doorIndex * 2 + .2f, .05f, .3f);
		go.name = "Top Door";
		topDoorIndex = doorIndex;
		Door d = go.GetComponent<Door>();
		d.openRightTeleporter.gameObject.SetActive(true);
		return d;
	}

	public override bool CanAddRightDoor() {
		return rightDoorIndex == -1;
	}
	public override Door AddRightDoor() {
		GameObject go = Instantiate(LevelBuilder.instance.doorPrefab, new Vector3(offset + 2 * width + .3f, 0, 0), 
			Quaternion.identity) as GameObject;
		go.transform.eulerAngles += Vector3.up * 90f;
		int doorIndex = Random.Range(0, height);
		go.transform.position += new Vector3(0, .05f, doorIndex * 2 + 1.8f);
		go.name = "Right Door";
		rightDoorIndex = doorIndex;
		Door d = go.GetComponent<Door>();
		d.openRightTeleporter.gameObject.SetActive(true);
		return d;
	}

	public override bool CanAddLeftDoor() {
		return leftDoorIndex == -1;
	}
	public override Door AddLeftDoor() {
		GameObject go = Instantiate(LevelBuilder.instance.doorPrefab, new Vector3(offset, 0, 0), 
			Quaternion.identity) as GameObject;
		go.transform.eulerAngles += Vector3.up * 90f;
		int doorIndex = Random.Range(0, height - 1);
		go.transform.position += new Vector3(0, .05f, doorIndex * 2 + 1.8f);
		go.name = "Left Door";
		leftDoorIndex = doorIndex;
		Door d = go.GetComponent<Door>();
		d.openLeftTeleporter.gameObject.SetActive(true);
		return d;
	}
}

