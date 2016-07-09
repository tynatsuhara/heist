using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour {

	private Character character;
	private GameObject player;
	private Character playerScript;
	private NavMeshAgent agent;

	public bool alerted;
	private bool invoked;

	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		player = GameObject.FindWithTag("Player");
		playerScript = player.GetComponent<Character>();
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!character.isAlive)
			return;

		LegAnimation();

		if (alerted)
			AggroBehavior();
		else
			PassiveBehavior();
	}

	private void PassiveBehavior() {
		// follow path if has one

		if (!invoked && character.CanSeeCharacter(player) && playerScript.IsEquipped()) {
			float reactionTime = (Random.Range(.2f, 1f));
			Invoke("BecomeAggro", reactionTime);
			invoked = true;
		}
	}

	private void BecomeAggro() {
		if (!character.isAlive)
			return;
		
		alerted = true;
		character.DrawWeapon();
		character.LookAt(player.transform);
	}

	private void AggroBehavior() {
		if (character.CanSeeCharacter(player)) {
			agent.destination = transform.position;
			if (playerScript.isAlive) {
				character.Shoot();
			}
		} else {
			agent.destination = player.transform.position;
		}
	}

	private void LegAnimation() {
		if (agent.velocity == Vector3.zero) {
			if (character.walk.isWalking) {
				character.walk.StopWalk();
			}
		} else {
			if (!character.walk.isWalking) {
				character.walk.StartWalk();
			}
		}
	}
}

