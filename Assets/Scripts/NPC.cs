﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NPC : Character, Interactable {

	public enum NPCState {
		PASSIVE,
		CURIOUS,    			 	// they know something is up, but don't know of the player
		SEARCHING,   				// they are aware of the player, but don't know location
		ALERTING,					// running to notify guards
		FLEEING,					// running off the map
		HELD_HOSTAGE_UNTIED,
		HELD_HOSTAGE_TIED,
		ATTACKING
	}

	public NPCState currentState;
	protected NavMeshAgent agent;

	public override void Start() {
		base.Start();
		CharacterCustomization cc = GetComponent<CharacterCustomization>();
		string outfitName = cc.outfitNames[Random.Range(0, cc.outfitNames.Length)];
		GetComponent<CharacterCustomization>().ColorCharacter(Outfits.fits[outfitName], true);
		transform.RotateAround(transform.position, transform.up, Random.Range(0, 360));
		agent = GetComponent<NavMeshAgent>();
		InvokeRepeating("UpdateEvidenceInSight", 0f, .5f);
	}

	void Update() {
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
			case NPCState.ALERTING:
				StateAlerting();
				break;
			case NPCState.FLEEING:
				StateFleeing();
				break;
			case NPCState.HELD_HOSTAGE_UNTIED:
				StateHeldHostageUntied();
				break;
			case NPCState.HELD_HOSTAGE_TIED:
				StateHeldHostageTied();
				break;
			case NPCState.ATTACKING:
				StateAttacking();
				break;
		}

		timeInCurrentState += Time.deltaTime;
	}

	void FixedUpdate () {
		if (!isAlive || GameManager.paused)
			return;

		LegAnimation();
		walking = agent.enabled && agent.velocity != Vector3.zero;
		Rotate();
	}

	protected virtual void StatePassive() {}
	protected virtual void StateCurious() {}
	protected virtual void StateSearching() {}
	protected virtual void StateAlerting() {}
	protected virtual void StateFleeing() {}
	protected virtual void StateHeldHostageUntied() {}
	protected virtual void StateHeldHostageTied() {}
	protected virtual void StateAttacking() {}

	public override void Die(Vector3 location, Vector3 angle, DamageType type = DamageType.MELEE) {
		if (arms.CurrentFrame != 0 && Random.Range(0, 2) == 0 && currentState != NPCState.HELD_HOSTAGE_TIED)
			arms.SetFrame(0);
		base.Die(location, angle, type);		
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

	private bool transitioningState;
	protected float timeInCurrentState;
	private NPCState stateToTransitionTo;
	public void TransitionState(NPCState newState, float time = 0f) {
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

	public void Interact(Character character) {
		if (character.zipties > 0 && (currentState == NPCState.HELD_HOSTAGE_UNTIED)) {
			character.zipties--;
			TransitionState(NPCState.HELD_HOSTAGE_TIED);
		}
	}
	public void Uninteract(Character character) {}

	protected void LookForEvidence() {
		if (!glimseInvoked && seesEvidence) {
			Invoke("GlimpsedEvidence", Random.Range(.2f, .5f));
			glimseInvoked = true;
		}
	}
	private bool glimseInvoked;
	private void GlimpsedEvidence() {
		UpdateEvidenceInSight();
		glimseInvoked = false;
		if (!seesEvidence)
			return;
		
		Alert(Reaction.SUSPICIOUS, evidencePoint.Value);
	}

	public bool seesEvidence;
	public Vector3? evidencePoint;
	public void UpdateEvidenceInSight() {
		evidencePoint = EvidenceInSight();
		seesEvidence = evidencePoint != null;
	}

	private Vector3? EvidenceInSight() {
		Computer cameraScreen = CheckForCameraComputer();
		bool canSeeOnCameras = cameraScreen != null && cameraScreen.PlayerInSight();
		List<PlayerControls> seenPlayers = GameManager.players
				.Where(x => CanSee(x.gameObject) && x.IsEquipped())
				.OrderBy(x => (x.transform.position - transform.position).magnitude)
				.ToList();
		if (seenPlayers.Count > 0 || canSeeOnCameras)
			return seenPlayers[0].transform.position;
		
		foreach (NPC c in GameManager.characters) {
			bool isEvidence = !c.isAlive;
			isEvidence |= c.currentState == Civilian.NPCState.ALERTING;
			isEvidence |= c.currentState == Civilian.NPCState.HELD_HOSTAGE_TIED;
			isEvidence |= c.currentState == Civilian.NPCState.HELD_HOSTAGE_UNTIED;
			if ((isEvidence && CanSee(c.gameObject)) || 
			    (isEvidence && cameraScreen != null && cameraScreen.InSight(c.gameObject))) {
				return c.transform.position;
			}
		}

		return null;
	}
	private Computer CheckForCameraComputer() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, 2f)) {
			return hit.collider.GetComponentInParent<Computer>();
		}
		return null;
	}

	public override void Alert(Reaction importance, Vector3 position) {}
}