using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : NPC {

	private static Dictionary<PlayerControls, PlayerKnowledge> knowledge;

	public float walkingAnimationThreshold;

	public override void Start() {
		base.Start();
		if (knowledge == null) {
			knowledge = new Dictionary<PlayerControls, PlayerKnowledge>();
			foreach (PlayerControls pc in GameManager.players)
				knowledge.Add(pc, new PlayerKnowledge());
		}

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
		if (timeInCurrentState == 0) {
			Debug.Log("first search round");
			DrawWeapon();
			PlayerControls closestLastKnownPlayer = null;
			foreach (PlayerControls pc in GameManager.players) {
				if (closestLastKnownPlayer == null || (transform.position - closestLastKnownPlayer.transform.position).magnitude > 
						(transform.position - pc.transform.position).magnitude)
					closestLastKnownPlayer = pc;
				Debug.Log(closestLastKnownPlayer.transform.position);
			}
			if (closestLastKnownPlayer != null) {
				agent.SetDestination(closestLastKnownPlayer.transform.position);
			}
		}
		LookForEvidence();		
	}

	// EnemyState.ATTACKING
	private PlayerControls closestPlayer;
	protected override void StateAttacking() {
		if (closestPlayer == null || Time.time - knowledge[closestPlayer].lastSeenTime > .3) {
			closestPlayer = ClosestEnemyPlayerInSight();
			if (closestPlayer != null) {
				knowledge[closestPlayer].lastKnownLocation = closestPlayer.transform.position;
				knowledge[closestPlayer].lastSeenTime = Time.time;
			}
		}

		if (closestPlayer == null) {
			TransitionState(NPCState.SEARCHING);
		} else {
			DrawWeapon();
			if (timeInCurrentState == 0)
				speech.SayRandom(Speech.ENEMY_SPOTTED_PLAYER, showFlash: true, color: "red");				
			bool inRange = (closestPlayer.transform.position - transform.position).magnitude < currentGun.range;			
			if (inRange) {
				agent.Stop();
			} else {
				agent.SetDestination(closestPlayer.transform.position);
			}
			if (CanSee(closestPlayer.gameObject, fov:20f)) {
				Shoot();
			}
			LookAt(closestPlayer.transform.position);	
		}
	}
/*

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
		// GameManager.instance.WereGoingLoudBoys();
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

	private class PlayerKnowledge {
		public Vector3? lastKnownLocation;
		public float lastSeenTime;
	}
}

