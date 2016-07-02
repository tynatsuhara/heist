using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	Character character;

	void Start() {
		character = gameObject.GetComponent<Character>();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (!character.weaponDrawn) {
				character.DrawWeapon();
			} else {
				character.HideWeapon();
			}
		}
		if (Input.GetMouseButtonDown(0)) {
			character.Shoot();
		}
	}
 
	void FixedUpdate () {
    	LookAtMouse();
		Movement();
    }

	void Movement() {
		character.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}

	void LookAtMouse() {
		// Generate a plane that intersects the transform's position with an upwards normal.
    	Plane playerPlane = new Plane(Vector3.up, transform.position);
     	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    	float hitdist = 0f;
    	// If the ray is parallel to the plane, Raycast will return false.
    	if (playerPlane.Raycast(ray, out hitdist)) {
			character.LookAt(ray.GetPoint(hitdist));
		}
	}
}
