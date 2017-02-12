using UnityEngine;
using System.Collections;

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
		agent = GetComponent<NavMeshAgent>();
		// InvokeRepeating("CheckForEvidence", 0f, .5f);
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

	public override void Alert(Reaction importance, Vector3 position) {}
}
