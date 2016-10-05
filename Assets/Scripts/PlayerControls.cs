using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerControls : Character {

	private string[] outfit = {
		"3 0-13 70-73; 0 14-69; 1 58-59 44-45 30-31 16-17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"3 1; 5 0",
		"0 1-3"
	};

	void Start() {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();
		speech = GetComponentInChildren<TextObject>();
		GetComponent<CharacterCustomization>().ColorCharacter(outfit);

		gunScript.UpdateUI();
		GameUI.instance.UpdateHealth(health, healthMax, armor, armorMax);
	}

	void Update() {
		GetInput();
	}

	void GetInput() {
		if (!isAlive || GameManager.paused)
			return;

		if (Input.GetKeyDown(KeyCode.F)) {
			if (weaponDrawn) {
				Shout();
			}
			DrawWeapon();
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			Interact();
		} else if (Input.GetKeyUp(KeyCode.E)) {
			InteractCancel();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			DragBody();
		} else if (Input.GetKeyUp(KeyCode.Space)) {
			ReleaseBody();
		}

		if (Input.GetMouseButton(0)) {
			Shoot();
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			Reload();
		}
	}
 
	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
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
		if (Cheats.instance.IsCheatEnabled("konami"))
			speed *= 3f;

		Vector3 pos = transform.position;
		pos.x += speed * (z * Mathf.Sin(cameraRotation * Mathf.Deg2Rad) + 
			x * Mathf.Sin((cameraRotation + 90) * Mathf.Deg2Rad));
		pos.z += speed * (z * Mathf.Cos(cameraRotation * Mathf.Deg2Rad) + 
			x * Mathf.Cos((cameraRotation + 90) * Mathf.Deg2Rad));
//		transform.position = pos;
		rb.MovePosition(pos);

		if ((x != 0 || z != 0) && !walk.isWalking) {
			walk.StartWalk();
		} else if (x == 0 && z == 0 && walk.isWalking) {
			walk.StopWalk(true);
		}
		if (x != 0 || z != 0) {
			lastMoveDirection = new Vector3(x, 0, z).normalized;
		}
	}

	public override void Die(Vector3 angle) {
		base.Die(angle);
		GameManager.instance.GameOver(false);
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

	public void Shout() {
		if (!speech.currentlyDisplaying) {
			speech.SayRandom(Speech.PLAYER_SHOUT, showFlash: true, color:"yellow");
		}
		GameManager.instance.AlertInRange(Reaction.AGGRO, transform.position, 4f);
	}

	public override void Alert(Character.Reaction importance, Vector3 position) {}
	public void RemoveBody() {}
}
