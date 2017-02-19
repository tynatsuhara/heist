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
		if (firstStateIteration || targetPlayer == null) {
			DrawWeapon();
			targetPlayer = null;
			foreach (PlayerControls pc in GameManager.players) {
				if (targetPlayer == null || (transform.position - targetPlayer.transform.position).magnitude > 
						(transform.position - pc.transform.position).magnitude)
					targetPlayer = pc;
			}
			if (targetPlayer != null) {
				agent.SetDestination(targetPlayer.transform.position);
			}
		} else {
			foreach (PlayerControls pc in GameManager.players.OrderBy(x => (transform.position - x.transform.position).magnitude)) {
				if (!knowledge[pc].unknownLocation) {
					targetPlayer = pc;
					TransitionState(NPCState.ATTACKING);
					return;
				}
			}
			foreach (PlayerControls pc in GameManager.players.Where(x => x.IsEquipped() && x.isAlive)) {
				if (CanSee(pc.gameObject)) {
					TransitionState(NPCState.ATTACKING);
					return;
				}
			}
		}
	}

	// EnemyState.ATTACKING
	private PlayerControls targetPlayer;
	protected override void StateAttacking() {
		if (targetPlayer == null || !targetPlayer.isAlive || knowledge[targetPlayer].unknownLocation) {
			targetPlayer = ClosestEnemyPlayerInSight();
			if (targetPlayer != null) {
				knowledge[targetPlayer].lastKnownLocation = targetPlayer.transform.position;
				knowledge[targetPlayer].lastSeenTime = Time.time;
			}
		}

		if (targetPlayer == null) {
			TransitionState(NPCState.SEARCHING);
		} else {
			DrawWeapon();
			if (firstStateIteration && Random.Range(0, 3) == 0)
				speech.SayRandom(Speech.ENEMY_SPOTTED_PLAYER, showFlash: true, color: "red");				
			bool inRange = (targetPlayer.transform.position - transform.position).magnitude < currentGun.range;			
			if (inRange || !targetPlayer.isAlive) {
				agent.SetDestination(transform.position);
			} else {
				agent.SetDestination(targetPlayer.transform.position);
			}
			if (CanSee(targetPlayer.gameObject, fov:20f)) {
				Shoot();
			}
			LookAt(targetPlayer.transform.position);	
		}
	}

	public void Alert(Reaction importance) {
		Alert(importance, transform.position + transform.forward);
	}

	public override void Alert(Reaction importance, Vector3 position) {
		if (importance == Reaction.SUSPICIOUS && currentState != NPCState.ATTACKING) {
			TransitionState(NPCState.CURIOUS);
			LookAt(position);
		} else if (currentState != NPCState.ATTACKING) {
			TransitionState(NPCState.ATTACKING);
			LookAt(position);
			GameManager.instance.WereGoingLoudBoys();
		}
	}

	public override void Shoot() {
		base.Shoot();
		if (currentGun.NeedsToReload())
			Reload();
	}

	void OnCollisionEnter(Collision collision) {
		PlayerControls pc = collision.collider.GetComponentInParent<PlayerControls>();
		if (pc != null)
			Alert(GameManager.instance.alarmsRaised ? Reaction.AGGRO : Reaction.SUSPICIOUS, pc.transform.position);
    }

	private class PlayerKnowledge {
		public Vector3? lastKnownLocation;
		public float lastSeenTime;
		public bool unknownLocation {
			get { return Time.time - lastSeenTime > .3; }
		}
	}
}

