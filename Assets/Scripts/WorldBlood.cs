using UnityEngine;
using System.Collections;

public class WorldBlood : MonoBehaviour {

	public static WorldBlood instance;

	void Awake () {
		instance = this;
	}
	
	public void BleedAt(Vector3 worldLocation) {

	}
}
