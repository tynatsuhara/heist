using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : NPC {

	private static Dictionary<PlayerControls, Vector3?> lastKnownLocations = new Dictionary<PlayerControls, Vector3?>();

	public float walkingAnimationThreshold;
	private bool invoked;

	public override void Start() {
		base.Start();
		GetComponent<CharacterCustomization>().ColorCharacter(Outfits.fits["cop1"], true);

		if (GameManager.instance.alarmsRaised) {
			currentState = NPCState.SEARCHING;
		} else {
			currentState = NPCState.PASSIVE;			
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAlive || GameManager.paused)
			return;

		switch (currentState) {
			case NPCState.PASSIVE:
				StatePassive();
				break;
			case NPCState.CURIOUS:
				StateCurious();
				break;
			case NPCState.SEARCHING:
				StateSearching();
				break;
			case NPCState.ATTACKING:
				StateAttacking();
				break;
		}

		timeInCurrentState += Time.deltaTime;		
	}

	//=================== STATE FUNCTIONS ===================//

	// EnemyState.PASSIVE
	private void StatePassive() {
		/*
		PSEUDO:
		patrol points (stored in queue)
		

		*/
	}

	// EnemyState.CURIOUS
	private void StateCurious() {
		/*
		PSEUDO:
		investigate point of curiosity
		*/
	}

	// EnemyState.SEARCHING
	private void StateSearching() {
		/*
		PSEUDO:
		investigate point of curiosity
		*/
	}

	// EnemyState.ATTACKING
	private PlayerControls closestPlayer;
	private void StateAttacking() {
		/*
		PSEUDO:
		investigate point of curiosity
		*/

		if (closestPlayer == null)
			closestPlayer = ClosestPlayerInSight();

		if (closestPlayer == null) {
			TransitionState(NPCState.SEARCHING);
		} else {
			bool inRange = (closestPlayer.transform.position - transform.position).magnitude < currentGun.range;			
			if (inRange) {
				agent.Stop();
			} else {
				agent.SetDestination(closestPlayer.transform.position);
			}
			if (CanSee(closestPlayer.gameObject, fov:30f)) {
				Shoot();
			}
			LookAt(closestPlayer.transform.position);	
		}
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

	void OnCollisionEnter(Collision collision) {
		PlayerControls pc =collision.collider.GetComponentInParent<PlayerControls>();
		if (pc != null) {
			Alert(Reaction.SUSPICIOUS);
			LookAt(pc.transform);			
		}
    }
}

