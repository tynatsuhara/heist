using UnityEngine;
using System.Collections;

public class Civilian : Character {

	private Character playerScript;

	private bool braveCitizen;  // they have a gun and will take matters into their own hands

	private string[] copUniform = {
		"0 0-73; 1 57 60 44 45 31; 2 46; 6 58 59; 4 14-27; 3 17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"0 1; 5 0",
		"0 1-3"
	};

	void Awake() {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();
		playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
		speech = GetComponentInChildren<TextObject>();
	}

	void Start () {
		GetComponent<CharacterCustomization>().ColorCharacter(copUniform, true);

		braveCitizen = Random.Range(0, 100) < 10;
	}
	
	void Update () {
		if (!isAlive || GameManager.paused)
			return;

		PassiveBehavior();
	}

	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;

		Rotate();
	}

	private bool drawWeaponInvoked = false;
	private void PassiveBehavior() {
		if (braveCitizen && playerScript.IsEquipped()) {
			if (!drawWeaponInvoked && !weaponDrawn && CanSee(playerScript.gameObject) && !playerScript.CanSee(gameObject)) {
				Invoke("DrawWeaponIfUnseen", Random.Range(.3f, 4f));
				drawWeaponInvoked = true;
			}
			if (weaponDrawn) {
				LookAt(playerScript.transform);
				Shoot();
			}
		}
	}

	private void DrawWeaponIfUnseen() {
		if (!playerScript.CanSee(gameObject)) {
			DrawWeapon();
		} else {
			drawWeaponInvoked = false;
		}
	}

	public override void Alert(Character.Reaction importance, Vector3 position) {

	}
}
