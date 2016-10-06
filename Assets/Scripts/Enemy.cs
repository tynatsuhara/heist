using UnityEngine;
using System.Collections;
using System.Linq;

public class Enemy : Character {

	private GameObject player;
	private Character playerScript;
	private NavMeshAgent agent;



	// refactor AI to use these
	private enum EnemyState {
		PASSIVE,
		SEARCHING,          // they know something is up, but don't know of the player
		AGGRO_SEARCHING,    // they are aware of the player, but don't know location
		AGGRO_ATTACKING
	}


	// TODO: static lastPlayerLocation and lastSightingTime


	// state booleans, in somewhat order of precedence
	public bool alerted;
	public bool suspicious;
	public bool knowsPlayerLocation;
	public static bool enemyIsKnown;  // the enemies know that the player is their target
	public static Vector3 lastKnownPlayerLocation;
	public Vector3 investigatePoint;
	public float timeSpentWatchingPlayer;  // suspiciously watch the player for a while, then un-aggro

	public float walkingAnimationThreshold;
	private bool invoked;

	private string[] copUniform = {
		"0 0-73; 1 57 60 44 45 31; 2 46; 6 58 59; 4 14-27; 3 17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"0 1; 5 0",
		"done in Awake()"
	};

	void Awake () {
		copUniform[3] = "0 " + Random.Range(1, 3) + "-3";  // sleeve length

		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();

		player = GameObject.FindWithTag("Player");
		playerScript = player.GetComponent<Character>();
		agent = GetComponent<NavMeshAgent>();
		speech = GetComponentInChildren<TextObject>();
	}

	void Start() {
		GetComponent<CharacterCustomization>().ColorCharacter(copUniform, true);

		if (GameManager.instance.alarmsRaised) {
			Alert(Reaction.AGGRO);
			knowsPlayerLocation = enemyIsKnown;
		}

		InvokeRepeating("CheckForEvidence", 0f, .5f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAlive || GameManager.paused)
			return;

		LegAnimation();

		if (alerted) {
			AggroBehavior();
		} else if (suspicious) {
			SuspiciousBehavior();
		} else {
			PassiveBehavior();
		}
	}

	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;

		Drag();
		Rotate();
	}

	private void GlimpsedPlayer() {
		if (!CanSee(player) || !playerScript.IsEquipped() || !isAlive) {
			invoked = false;
			return;
		}
		speech.SayRandom(Speech.ENEMY_SPOTTED_PLAYER, showFlash: true, color: "red");
		Alert(Reaction.AGGRO, player.transform.position);
	}

	public void Alert(Character.Reaction importance) {
		Alert(importance, transform.position + transform.forward);
	}

	// TODO: being alerted without knowing location   <- could accomplish with global lastKnownLocation?
	public override void Alert(Reaction importance, Vector3 position) {
		if (!isAlive)
			return;

		if (!alerted || (alerted && importance == Reaction.AGGRO)) {
			investigatePoint = position;			
		}

		if (importance == Reaction.AGGRO) {
			alerted = true;
			GameManager.instance.WereGoingLoudBoys();
		} else if (importance == Reaction.SUSPICIOUS) {
			timeSpentWatchingPlayer = 0f;
			suspicious = true;
		} else if (importance == Reaction.MILDLY_SUSPICIOUS) {

		}
	}



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
			if (playerScript.isAlive && CanSee(player, fov:30f)) {
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
	}

	private void PassiveBehavior() {
		// TODO: follow path if has one
		
		CheckCanSeeEvidence();
	}

	private void CheckCanSeeEvidence() {
		if (!invoked && seesEvidence) {
			float reactionTime = (Random.Range(.2f, 1f));
			Invoke("GlimpsedPlayer", reactionTime);
			invoked = true;
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
        if (collision.collider.transform.root.gameObject == player) {
			Alert(Reaction.SUSPICIOUS);
			LookAt(player.transform);			
		}
    }
}

