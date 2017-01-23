using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : Character {

	private static Dictionary<PlayerControls, Vector3> lastKnownLocations = new Dictionary<PlayerControls, Vector3>();

	private NavMeshAgent agent;

	private enum EnemyState {
		PASSIVE,
		CURIOUS,          // they know something is up, but don't know of the player
		SEARCHING,   	  // they are aware of the player, but don't know location
		ATTACKING
	}
	private EnemyState currentState;

	public float walkingAnimationThreshold;
	private bool invoked;

	private string[] copUniform = {
		"0 0-73; 1 57 60 44 45 31; 2 46; 6 58 59; 4 14-27; 3 17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"0 1; 5 0",
		"done in Start()"
	};

	void Awake () {
		rb = GetComponent<Rigidbody>();
		agent = GetComponent<NavMeshAgent>();
		speech = GetComponentInChildren<TextObject>();

		SpawnGun();		
	}

	void Start() {
		copUniform[3] = "0 " + Random.Range(1, 3) + "-3";  // sleeve length
		GetComponent<CharacterCustomization>().ColorCharacter(copUniform, true);

		if (GameManager.instance.alarmsRaised) {
			Alert(Reaction.AGGRO);
		} else {
			CheckForCameraComputer();
		}

		InvokeRepeating("CheckForEvidence", 0f, .5f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAlive || GameManager.paused)
			return;

		LegAnimation();
		walking = agent.enabled && agent.velocity != Vector3.zero;
		
		timeInCurrentState += Time.deltaTime;

		switch (currentState) {
			case EnemyState.PASSIVE:
				StatePassive();
				break;
			case EnemyState.CURIOUS:
				StateCurious();
				break;
			case EnemyState.SEARCHING:
				StateSearching();
				break;
			case EnemyState.ATTACKING:
				StateAttacking();
				break;
		}
	}

	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;

		// Drag();
		Rotate();
	}

	//=================== STATE FUNCTIONS ===================//

	// EnemyState.PASSIVE
	private void StatePassive() {}

	// EnemyState.CURIOUS
	private void StateCurious() {}

	// EnemyState.SEARCHING
	private void StateSearching() {}

	// EnemyState.ATTACKING
	private void StateAttacking() {}


	private bool transitioningState;
	private float timeInCurrentState;
	private EnemyState stateToTransitionTo;
	private void TransitionState(EnemyState newState, float time = 0f) {
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

	/*private void GlimpsedPlayer() {
		CheckCanSeeEvidence();
		if (!seesEvidence)
			return;
		
		speech.SayRandom(Speech.ENEMY_SPOTTED_PLAYER, showFlash: true, color: "red");
		Alert(Reaction.AGGRO, player.transform.position);
	}

	// TODO: being alerted without knowing location   <- could accomplish with global lastKnownLocation?

	private void AggroBehavior() {
		bool inRange = (player.transform.position - transform.position).magnitude < gunScript.range;

		DrawWeapon();		
		// If one enemy can see the player, they all know where he is
		if (CanSee(player)) {
			knowsPlayerLocation = true;
			enemyIsKnown = true;
			lastKnownPlayerLocation = player.transform.position;
			if (inRange) {
				agent.destination = transform.position;
			} else {
				agent.destination = player.transform.position;
			}
			if (CanSee(player, fov:30f)) {
				Shoot();
			}
			LookAt(lastKnownPlayerLocation);			
		// If they can't see him, go to where they know he was last
		} else if (knowsPlayerLocation) {
			LoseLookTarget();
			agent.destination = lastKnownPlayerLocation;
			if (agent.velocity.magnitude == 0f) {
				knowsPlayerLocation = false;
				// TODO: What happens when you can't find the enemy?
				//       Maybe regroup with other officers. Maybe explore.
			}
		} else {
			LoseLookTarget();			
			agent.destination = investigatePoint;
		}
	}

	private void SuspiciousBehavior() {
		float followDistance = 4f;
		if (CanSee(player)) {
			lastKnownPlayerLocation = player.transform.position;
			knowsPlayerLocation = true;
			if ((transform.position - player.transform.position).magnitude < followDistance) {
				agent.destination = transform.position;			
			} else {
				agent.destination = lastKnownPlayerLocation;
			}
			timeSpentWatchingPlayer += Time.deltaTime;
			float timeSpentWatchingBeforeGivingUp = 5f;
			if (timeSpentWatchingPlayer > timeSpentWatchingBeforeGivingUp) {
				suspicious = false;
				agent.destination = transform.position;
				LoseLookTarget();					
			}			
		} else if (knowsPlayerLocation) {
			agent.destination = lastKnownPlayerLocation;
		} else {
			agent.destination = investigatePoint;
		}
		CheckCanSeeEvidence();
	}*/

	public void Alert(Character.Reaction importance) {
		Alert(importance, transform.position + transform.forward);
	}

	public override void Alert(Reaction importance, Vector3 position) {

	}

	public override void Shoot() {
		base.Shoot();
		if (currentGun.NeedsToReload())
			Reload();
	}

	private void CheckCanSeeEvidence() {
		if (!invoked && seesEvidence) {
			float reactionTime = (Random.Range(.2f, 1f));
			Invoke("GlimpsedPlayer", reactionTime);
			invoked = true;
		}
	}

	private void CheckForCameraComputer() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, 2f)) {
			cameraScreen = hit.collider.GetComponentInParent<Computer>();
		}
	}

	private void LegAnimation() {
		Vector3 velocity = agent.velocity;
		velocity.y = 0f;
		if (velocity == Vector3.zero) {
			if (walk.isWalking) {
				walk.StopWalk();
			}
		} else if (velocity.magnitude > 0f) {
			if (!walk.isWalking) {
				walk.StartWalk();
			}
		}
	}

	void OnCollisionEnter(Collision collision) {
		PlayerControls pc =collision.collider.GetComponentInParent<PlayerControls>();
		if (pc != null) {
			Alert(Reaction.SUSPICIOUS);
			LookAt(pc.transform);			
		}
    }
}

