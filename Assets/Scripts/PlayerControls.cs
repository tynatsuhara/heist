using UnityEngine;
using System.Collections;

public class PlayerControls : Character {

	void Start() {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();
	}

	void Update() {
		GetInput();
	}

	void GetInput() {
		if (!isAlive)
			return;

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (!weaponDrawn) {
				DrawWeapon();
			} else {
				HideWeapon();
			}
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			Interact();
		} else if (Input.GetKeyUp(KeyCode.E)) {
			InteractCancel();
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			DragBody();
		} else if (Input.GetKeyUp(KeyCode.E)) {
			ReleaseBody();
		}

		if (Input.GetMouseButton(0)) {
			Shoot();
		}
	}
 
	void FixedUpdate () {
		if (!isAlive)
			return;
		
    	LookAtMouse();
		Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		Drag();
		Rotate();
    }

	void LookAtMouse() {
		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.up, transform.position);
     	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    	float hitdist = 0f;
    	// If the ray is parallel to the plane, Raycast will return false.
    	if (playerPlane.Raycast(ray, out hitdist)) {
			LookAt(ray.GetPoint(hitdist));
		}
	}

	public override void Alert() {}
}
