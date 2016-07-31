using UnityEngine;
using System.Collections;

public class Enemy : Character {

	private GameObject player;
	private Character playerScript;
	private NavMeshAgent agent;

	public bool alerted;
	public float walkingAnimationThreshold;
	private bool invoked;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();

		player = GameObject.FindWithTag("Player");
		playerScript = player.GetComponent<Character>();
		agent = GetComponent<NavMeshAgent>();
		speech = GetComponentInChildren<TextObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAlive)
			return;

		LegAnimation();

		if (alerted)
			AggroBehavior();
		else
			PassiveBehavior();
	}

	void FixedUpdate () {
		if (!isAlive)
			return;

		Drag();
		Rotate();
	}

	private void PassiveBehavior() {
		// follow path if has one

		if (!invoked && CanSeeCharacter(player) && playerScript.IsEquipped()) {
			float reactionTime = (Random.Range(.2f, 1f));
			Invoke("GlimpsedPlayer", reactionTime);
			invoked = true;
		}
	}

	private void GlimpsedPlayer() {
		if (!CanSeeCharacter(player) || !playerScript.IsEquipped()) {
			invoked = false;
			return;
		}
		Alert();
	}


	// States to be in:
	//    passive
	//    suspicious
	//    alerted without knowing location   <- could accomplish with global lastKnownLocation?
	//    alerted with knowing location
	public override void Alert() {
		if (alerted || !isAlive)
			return;
		Debug.Log("alerted");
		alerted = true;
		DrawWeapon();
		LookAt(player.transform);
		speech.Say("HEY WHAT THE FUCK", showFlash:true);
	}

	private void AggroBehavior() {
		bool inRange = (player.transform.position - transform.position).magnitude < gunScript.range;

		if (CanSeeCharacter(player)) {
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
}

