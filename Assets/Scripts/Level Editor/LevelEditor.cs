using UnityEngine;
using System.Collections;

public class LevelEditor : MonoBehaviour {

	public Transform highlightBox;

	void Start () {
	
	}
	
	void Update () {
		Plane playerPlane = new Plane(Vector3.up, Vector3.zero);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float hitdist = 0f;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast(ray, out hitdist)) {
			Vector3 pos = ray.GetPoint(hitdist);
			pos.x -= pos.x % LevelBuilder.TILE_SIZE;
			pos.z -= pos.z % LevelBuilder.TILE_SIZE;
			highlightBox.position = pos;
			highlightBox.transform.localScale = new Vector3(LevelBuilder.TILE_SIZE, LevelBuilder.TILE_SIZE, LevelBuilder.TILE_SIZE);
		}
	}
}
