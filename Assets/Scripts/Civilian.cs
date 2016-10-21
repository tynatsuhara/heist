using UnityEngine;
using System.Collections;

public class Civilian : Character, Interactable {

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
	public bool braveCitizen;  // can enter attacking state	

	void Awake() {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();
		playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
		speech = GetComponentInChildren<TextObject>();
		InvokeRepeating("CheckForEvidence", 0f, .5f);
	}

	void Start () {
		GetComponent<CharacterCustomization>().ColorCharacter(copUniform, true);
		currentState = CivilianState.PASSIVE;
		braveCitizen = Random.Range(0, 100) < 10;
	}
	
	void Update () {
		if (!isAlive || GameManager.paused)
			return;
		
		timeInCurrentState += Time.deltaTime;

		switch (currentState) {
			case CivilianState.PASSIVE:
				StatePassive();
				break;
			case CivilianState.ALERTING:
				StateAlerting();
				break;
			case CivilianState.FLEEING:
				StateFleeing();
				break;
			case CivilianState.ATTACKING:
				StateAttacking();
				break;
			case CivilianState.HELD_HOSTAGE_UNTIED:
				StateHeldHostageUntied();
				break;
			case CivilianState.HELD_HOSTAGE_TIED:
				StateHeldHostageTied();
				break;
			case CivilianState.HELD_HOSTAGE_CALLING:
				StateHeldHostageCalling();
				break;
		}
	}

	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;

		Rotate();
	}

	private bool transitioningState;
	private float timeInCurrentState;
	private CivilianState stateToTransitionTo;
	private void TransitionState(CivilianState newState, float time = 0f) {
		stateToTransitionTo = newState;
		transitioningState = true;
		if (time <= 0f) {
			CompleteTransition();
		} else {
			Invoke("CompleteTransition", time);
		} 
	}
	private void CompleteTransition() {
		if (currentState != stateToTransitionTo)
			timeInCurrentState = 0f;
		
		currentState = stateToTransitionTo;
		transitioningState = false;
	}

	//=================== STATE FUNCTIONS ===================//

	// CivilianState.PASSIVE
	private bool checkDrawWeaponInvoked = false;
	private void StatePassive() {
		LoseLookTarget();

		if (seesEvidence) {
			// TODO: switch to ALERTING

			
		}
		BraveCitizenCheck();		
	}
	private void CheckDrawWeapon() {
		if (!playerScript.CanSee(gameObject)) {
			TransitionState(CivilianState.ATTACKING, 0f);
		} else {
			checkDrawWeaponInvoked = false;
		}
	}
	private void BraveCitizenCheck() {
		if (braveCitizen && !checkDrawWeaponInvoked && playerScript.IsEquipped() && playerScript.isAlive) {
			// switch to attacking
			bool canSeePlayer = currentState == CivilianState.PASSIVE ? CanSee(playerScript.gameObject) : ClearShot(playerScript.gameObject);
			if (canSeePlayer && !playerScript.CanSee(gameObject)) {
				Invoke("CheckDrawWeapon", Random.Range(.3f, 3f));
				checkDrawWeaponInvoked = true;				
			}
		}
	}

	// any states that can come after CivilianState.HELD_HOSTAGE_UNTIED should call this	
	private void ResetRB() {
		if (rb.constraints == RigidbodyConstraints.None) {
			rb.rotation = Quaternion.Euler(new Vector3(0, rb.rotation.y, 0));
			rb.position = new Vector3(rb.position.x, 1.1f, rb.position.z);
			GetComponent<NavMeshAgent>().enabled = true;			
			arms.SetFrame(0);
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}

	// CivilianState.ALERTING
	private void StateAlerting() {}

	// CivilianState.FLEEING
	private void StateFleeing() {
		ResetRB();
	}

	// CivilianState.ATTACKING
	private void StateAttacking() {
		ResetRB();
		DrawWeapon();
		LookAt(playerScript.transform);		
		if (CanSee(playerScript.gameObject, fov:40f)) {
			Shoot();
		}
	}

	// CivilianState.HELD_HOSTAGE_UNTIED
	private void StateHeldHostageUntied() {
		BraveCitizenCheck();
		LoseLookTarget();						

		if (rb.constraints == RigidbodyConstraints.None)
			return;

		rb.constraints = RigidbodyConstraints.None;
		GetComponent<NavMeshAgent>().enabled = false;
		Vector3 rot = rb.rotation.eulerAngles;
		rot.x += 32f;
		rb.MoveRotation(Quaternion.Euler(rot));
	}

	// CivilianState.HELD_HOSTAGE_TIED
	private void StateHeldHostageTied() {
		if (arms.CurrentFrame != 2) {
			arms.SetFrame(2);
		}
	}

	// CivilianState.HELD_HOSTAGE_CALLING
	private void StateHeldHostageCalling() {}


	public override void Alert(Character.Reaction importance, Vector3 position) {
		if (currentState == CivilianState.ATTACKING ||
			currentState == CivilianState.HELD_HOSTAGE_UNTIED ||
			currentState == CivilianState.HELD_HOSTAGE_TIED)
			return;

		if (currentState == CivilianState.HELD_HOSTAGE_CALLING) {
			TransitionState(CivilianState.HELD_HOSTAGE_UNTIED);
			return;
		}

		arms.SetFrame(1);  // hands up
		TransitionState(CivilianState.HELD_HOSTAGE_UNTIED, Random.Range(.3f, 1f));
	}

	public void Interact(Character character) {
		if (currentState == CivilianState.HELD_HOSTAGE_UNTIED || currentState == CivilianState.HELD_HOSTAGE_CALLING) {
			TransitionState(CivilianState.HELD_HOSTAGE_TIED);
		}
	}
	public void Uninteract(Character character) {}
}
