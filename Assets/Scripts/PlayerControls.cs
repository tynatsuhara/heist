using UnityEngine;
using System.Collections;

public class PlayerControls : Character {

	void Start() {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();
		speech = GetComponentInChildren<TextObject>();
	}

	void Update() {
		GetInput();
	}

	void GetInput() {
		if (!isAlive)
			return;

		if (Input.GetKeyDown(KeyCode.Space)) {
			DrawWeapon();
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

	public void Move(float x, float z) {
		float cameraRotation = CameraMovement.instance.transform.eulerAngles.y;

		float speed = moveSpeed;
		if (draggedBody != null)
			speed *= .5f;

		Vector3 pos = transform.position;
		pos.x += speed * (z * Mathf.Sin(cameraRotation * Mathf.Deg2Rad) + 
			x * Mathf.Sin((cameraRotation + 90) * Mathf.Deg2Rad));
		pos.z += speed * (z * Mathf.Cos(cameraRotation * Mathf.Deg2Rad) + 
			x * Mathf.Cos((cameraRotation + 90) * Mathf.Deg2Rad));
		transform.position = pos;

		if ((x != 0 || z != 0) && !walk.isWalking) {
			walk.StartWalk();
		} else if (x == 0 && z == 0 && walk.isWalking) {
			walk.StopWalk();
		}
		if (x != 0 || z != 0) {
			lastMoveDirection = new Vector3(x, 0, z).normalized;
		}
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
