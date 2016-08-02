﻿using UnityEngine;
using System.Collections;

public class Enemy : Character {

	private GameObject player;
	private Character playerScript;
	private NavMeshAgent agent;

	public bool alerted;
	public float walkingAnimationThreshold;
	private bool invoked;

	private string[] outfit = {
		"3 0-13 70-73; 0 14-69; 1 58-59 44-45 30-31 16-17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"3 1; 5 0",
		"0 1-3"
	};

	void Awake () {
		rb = GetComponent<Rigidbody>();
		gunScript = gun.GetComponent<Gun>();

		player = GameObject.FindWithTag("Player");
		playerScript = player.GetComponent<Character>();
		agent = GetComponent<NavMeshAgent>();
		speech = GetComponentInChildren<TextObject>();
		GetComponent<CharacterCustomization>().ColorCharacter(outfit);
	}

	void Start() {
		if (GameManager.instance.alarmsRaised) {
			Alert(Reaction.AGGRO, transform.position + transform.forward);
		}
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
		// TODO: follow path if has one
		
		if (!invoked && CanSeeEvidence()) {
			float reactionTime = (Random.Range(.2f, 1f));
			Invoke("GlimpsedPlayer", reactionTime);
			invoked = true;
		}
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
		if (!CanSee(player) || !playerScript.IsEquipped()) {
			invoked = false;
			return;
		}
		speech.Say("HEY WHAT THE FUCK", showFlash:true, color:"red");
		speech.SayRandom(new string[] {
				"HEY WHAT THE FUCK",
				"HANDS UP",
				"YOU'RE GOING DOWN",
				"TIME TO MEET HARAMBE",
				"BIG MISTAKE",
				"OH IT'S LIT"
			}, showFlash: true, color: "red");
		Alert(Reaction.AGGRO, player.transform.position);
	}

	public void Alert() {
		Alert(Reaction.AGGRO, player.transform.position);
	}

	// States to be in:
	//    passive
	//    suspicious
	//    alerted without knowing location   <- could accomplish with global lastKnownLocation?
	//    alerted with knowing location
	public override void Alert(Character.Reaction importance, Vector3 position) {
		if (alerted || !isAlive)
			return;
		DrawWeapon();
		LookAt(player.transform);
		if (importance == Reaction.AGGRO) {
			alerted = true;
			GameManager.instance.WereGoingLoudBoys();
		}
	}

	private void AggroBehavior() {
		bool inRange = (player.transform.position - transform.position).magnitude < gunScript.range;

		if (CanSee(player)) {
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

