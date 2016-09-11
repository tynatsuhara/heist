using UnityEngine;
using System.Collections;

public class Enemy : Character {

	private GameObject player;
	private Character playerScript;
	private NavMeshAgent agent;

	// state booleans, in somewhat order of precedence
	public bool alerted;
	public bool suspicious;
	public bool knowsPlayerLocation;

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

	private bool seesEvidence;
	private void CheckForEvidence() {
		seesEvidence = CanSeeEvidence();
	}
	// TODO: can they see the player, bodies, etc?
	private bool CanSeeEvidence() {
		bool visiblePlayer = (CanSee(player) && playerScript.IsEquipped());
		if (visiblePlayer)
			return true;
		
		Character[] dead = GameManager.instance.DeadCharacters();
		foreach (Character c in dead) {
			if (CanSee(c.gameObject)) {
				return true;
			}
		}

		return false;
	}

	private void GlimpsedPlayer() {
		if (!CanSee(player) || !playerScript.IsEquipped() || !isAlive) {
			invoked = false;
			return;
		}
		speech.SayRandom(Speech.ENEMY_SPOTTED_PLAYER, showFlash: true, color: "red");
		Alert(Reaction.AGGRO, player.transform.position);
	}

	public void Alert() {
		Alert(Reaction.AGGRO, player.transform.position);
	}

	public void Alert(Character.Reaction importance) {
		Alert(importance, transform.position + transform.forward);
	}

	// TODO: being alerted without knowing location   <- could accomplish with global lastKnownLocation?
	public override void Alert(Character.Reaction importance, Vector3 position) {
		if (alerted || !isAlive)
			return;
		LookAt(player.transform);
		if (importance == Reaction.AGGRO) {
			alerted = true;
			GameManager.instance.WereGoingLoudBoys();
		} else if (importance == Reaction.SUSPICIOUS) {
			suspicious = true;
		} else if (importance == Reaction.MILDLY_SUSPICIOUS) {

		}
	}



	private void AggroBehavior() {
		bool inRange = (player.transform.position - transform.position).magnitude < gunScript.range;

		if (CanSee(player)) {
			DrawWeapon();			
			if (inRange) {
				agent.destination = transform.position;
			} else {
				agent.destination = player.transform.position;
			}
			if (playerScript.isAlive) {
				Shoot();
			}
		} else {
			agent.destination = player.transform.position;
		}
	}

	private void SuspiciousBehavior() {

	}

	private void PassiveBehavior() {
		// TODO: follow path if has one
		
		if (!invoked && seesEvidence) {
			float reactionTime = (Random.Range(.2f, 1f));
			Invoke("GlimpsedPlayer", reactionTime);
			invoked = true;
		}
	}




	private void LegAnimation() {
		if (agent.velocity == Vector3.zero) {
			if (walk.isWalking) {
				walk.StopWalk();
			}
		} else if (agent.velocity.magnitude > walkingAnimationThreshold) {
			if (!walk.isWalking) {
				walk.StartWalk();
			}
		}
	}

	void OnCollisionEnter(Collision collision) {
        if (collision.collider.transform.root.gameObject == player) {
			Alert(Reaction.SUSPICIOUS);
		}
    }
}

