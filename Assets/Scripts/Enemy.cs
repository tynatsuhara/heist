using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : NPC {

	private static Dictionary<PlayerControls, Vector3?> lastKnownLocations = new Dictionary<PlayerControls, Vector3?>();

	public float walkingAnimationThreshold;

	public override void Start() {
		base.Start();

		if (GameManager.instance.alarmsRaised) {
			currentState = NPCState.SEARCHING;
		} else {
			currentState = NPCState.PASSIVE;			
		}
	}

	//=================== STATE FUNCTIONS ===================//

	// EnemyState.PASSIVE
	protected override void StatePassive() {
		/*
		PSEUDO:
		patrol points (stored in queue)
		

		*/
		LookForEvidence();
	}

	// EnemyState.CURIOUS
	protected override void StateCurious() {
		/*
		PSEUDO:
		investigate point of curiosity
		*/
		LookForEvidence();
	}

	// EnemyState.SEARCHING
	protected override void StateSearching() {
		/*
		PSEUDO:
		look for a player if you can, otherwise look for points of interest
		*/
		LookForEvidence();
	}

	// EnemyState.ATTACKING
	private PlayerControls closestPlayer;
	protected override void StateAttacking() {
		if (closestPlayer == null)
			closestPlayer = ClosestPlayerInSight();

		if (closestPlayer == null) {
			TransitionState(NPCState.SEARCHING);
		} else {
			DrawWeapon();
			if (!lastKnownLocations.ContainsKey(closestPlayer))
				lastKnownLocations.Add(closestPlayer, null);
			lastKnownLocations[closestPlayer] = closestPlayer.transform.position;
			if (timeInCurrentState == 0)
				speech.SayRandom(Speech.ENEMY_SPOTTED_PLAYER, showFlash: true, color: "red");				
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



	// TODO: being alerted without knowing location   <- could accomplish with global lastKnownLocation?

	/*private void AggroBehavior() {
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

	public void Alert(Reaction importance) {
		Alert(importance, transform.position + transform.forward);
	}

	public override void Alert(Reaction importance, Vector3 position) {
		TransitionState(NPCState.ATTACKING);
	}

	public override void Shoot() {
		base.Shoot();
		if (currentGun.NeedsToReload())
			Reload();
	}

	void OnCollisionEnter(Collision collision) {
		PlayerControls pc = collision.collider.GetComponentInParent<PlayerControls>();
		if (pc != null) {
			Alert(Reaction.SUSPICIOUS);
			LookAt(pc.transform);			
		}
    }
}

