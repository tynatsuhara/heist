using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour {

	private Character character;
	private GameObject player;
	private Character playerScript;

	public bool alerted;

	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		player = GameObject.FindWithTag("Player");
		playerScript = player.GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!character.isAlive)
			return;
		
		if (alerted)
			AggroBehavior();
		else
			PassiveBehavior();
	}

	private void PassiveBehavior() {
		// follow path if has one

		if (character.CanSeeCharacter(player) && playerScript.PoliceShouldAttack()) {
			float reactionTime = (Random.Range(0, 3) + 1) * 1f;
			Invoke("BecomeAggro", reactionTime);
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
		character.Shoot();
	}


}

