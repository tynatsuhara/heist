using UnityEngine;
using System.Collections;

public class Civilian : Character {

	private string[] copUniform = {
		"0 0-73; 1 57 60 44 45 31; 2 46; 6 58 59; 4 14-27; 3 17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"0 1; 5 0",
		"0 1-3"
	};

	public enum CivilianState {
		PASSIVE,                    // default behavior
		ALERTING,                   // running to notify guards
		FLEEING,                    // running off the map
		ATTACKING,                  // surprising the player
		HELD_HOSTAGE_UNTIED,
		HELD_HOSTAGE_TIED,
		HELD_HOSTAGE_CALLING
	}
	public CivilianState currentState;

	private Character playerScript;	
	private bool braveCitizen;  // can enter attacking state	

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

		switch (currentState) {
			case CivilianState.PASSIVE:
				StatePassive();
				break;
			case CivilianState.ALERTING:
				break;
			case CivilianState.FLEEING:
				break;
			case CivilianState.ATTACKING:
				StateAttacking();
				break;
			case CivilianState.HELD_HOSTAGE_UNTIED:
				break;
			case CivilianState.HELD_HOSTAGE_TIED:
				break;
			case CivilianState.HELD_HOSTAGE_CALLING:
				break;
		}
	}

	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;

		Rotate();
	}

	private bool transitioningState;
	private CivilianState stateToTransitionTo;
	private void TransitionState(CivilianState newState, float time) {
		stateToTransitionTo = newState;
		transitioningState = true;
		if (time <= 0f) {
			CompleteTransition();
		} else {
			Invoke("CompleteTransition", time);
		} 
	}
	private void CompleteTransition() {
		currentState = stateToTransitionTo;
		transitioningState = false;
	}

	//=================== STATE FUNCTIONS ===================//

	// CivilianState.PASSIVE
	private bool checkDrawWeaponInvoked = false;
	private void StatePassive() {
		LoseLookTarget();
		if (!checkDrawWeaponInvoked && braveCitizen && playerScript.IsEquipped()) {
			if (CanSee(playerScript.gameObject) && !playerScript.CanSee(gameObject)) {
				Invoke("CheckDrawWeapon", Random.Range(.3f, 3f));
				checkDrawWeaponInvoked = true;				
			}
		}
	}
	private void CheckDrawWeapon() {
		if (!playerScript.CanSee(gameObject)) {
			TransitionState(CivilianState.ATTACKING, 0f);
		}
	}

	// CivilianState.ATTACKING
	private void StateAttacking() {
		DrawWeapon();
		if (CanSee(playerScript.gameObject, fov:40f)) {
			LookAt(playerScript.transform);
			Shoot();
		}
	}

	public override void Alert(Character.Reaction importance, Vector3 position) {

	}
}
