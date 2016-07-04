using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour {

	private Character character;
	private GameObject player;

	public bool alerted;

	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		character.DrawWeapon();

		player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null)
			return;

		Debug.Log(CanSeePlayer());
	}

	public bool CanSeePlayer() {
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - player.transform.position), transform.forward);
		if (angle >= -.2f)
			return false;

		RaycastHit hit;
		if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit)) {
			return hit.collider.transform.root.gameObject == player;
		}

		return false;
	}
}

