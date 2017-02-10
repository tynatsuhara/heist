using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerControls : Character {

	public int id;
	public PlayerCamera playerCamera;
	public PlayerUI playerUI;

	public override void Start() {
		CharacterCustomizationMenu.instance.LoadWeaponsFromPrefs(this);
		base.Start();
		CharacterCustomizationMenu.instance.ColorizeFromPrefs(this);	
		name = "Player " + id;
		explosive = GetComponent<Explosive>();	
	}

	void Update() {
		GetInput();
		playerUI.UpdateHealth(health, healthMax, armor, armorMax);
		if (currentGun != null)
			currentGun.UpdateUI();
	}

	void GetInput() {
		if (!isAlive || GameManager.paused)
			return;

		bool p1 = id == 1;

		if ((p1 && Input.GetKeyDown(KeyCode.F)) || Input.GetKeyDown("joystick " + id + " button 3")) {
			if (weaponDrawn) {
				Shout();
			}
			DrawWeapon();
		}

		if ((p1 && Input.GetKeyDown(KeyCode.E)) || Input.GetKeyDown("joystick " + id + " button 1")) {
			Interact();
		} else if ((p1 && Input.GetKeyUp(KeyCode.E)) || Input.GetKeyUp("joystick " + id + " button 1")) {
			InteractCancel();
		}

		if ((p1 && Input.GetKeyDown(KeyCode.G)) || Input.GetKeyDown("joystick " + id + " button 2")) {
			DropBag();
		}

		if ((p1 && Input.GetKeyDown(KeyCode.Space)) || Input.GetKeyDown("joystick " + id + " button 5")) {
			DragBody();
		} else if ((p1 && Input.GetKeyUp(KeyCode.Space)) || Input.GetKeyUp("joystick " + id + " button 5")) {
			ReleaseBody();
		}

		if ((p1 && Input.GetMouseButton(0)) || Input.GetKey("joystick " + id + " button 7")) {
			Shoot();
		} else if ((p1 && Input.GetMouseButtonDown(1)) || Input.GetKeyDown("joystick " + id + " button 6")) {
			Melee();
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			Explosive();
		}

		if ((p1 && Input.GetKeyDown(KeyCode.R)) || Input.GetKeyDown("joystick " + id + " button 0")) {
			Reload();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SelectGun(0);
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			SelectGun(1);
		}

		if (Input.GetKeyDown(KeyCode.K)) {
			Vector3 damageDir = Random.insideUnitSphere;
			Damage(transform.position - damageDir * .5f, damageDir, 1000f, type: DamageType.EXPLOSIVE);
		}

		playerUI.JoystickCursorMove(Input.GetAxis("RSX" + id), Input.GetAxis("RSY" + id));		
	}
 
	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;
		
    	LookAtMouse();
		Walk();
		Drag();
		Rotate();
    }

	private void Walk() {
		float h = 0;
		float v = 0;
		if (id == 1) {  // player 1 can use keyboard
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
			walking = h != 0 || v != 0;
		}
		if ((id == 1 && !walking) || id != 1) {
			h = Input.GetAxis("Horizontal" + id);
			v = Input.GetAxis("Vertical" + id);
			walking = h != 0 || v != 0;
		}
		Move(h, v);
	}

	private void Move(float x, float z) {
		float cameraRotation = playerCamera.transform.eulerAngles.y;

		float speed = moveSpeed;
		if (draggedBody != null)
			speed *= .5f;
		if (Cheats.instance.IsCheatEnabled("konami"))
			speed *= 3f;
		if (hasBag)
			speed *= bag.speedMultiplier;

		Vector3 pos = transform.position;
		pos.x += speed * (z * Mathf.Sin(cameraRotation * Mathf.Deg2Rad) + 
			x * Mathf.Sin((cameraRotation + 90) * Mathf.Deg2Rad));
		pos.z += speed * (z * Mathf.Cos(cameraRotation * Mathf.Deg2Rad) + 
			x * Mathf.Cos((cameraRotation + 90) * Mathf.Deg2Rad));
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

	void LookAtMouse() {
		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.up, transform.position);
		Ray ray = playerCamera.cam.ScreenPointToRay(playerUI.mousePos);
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
}
