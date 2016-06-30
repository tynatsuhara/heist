using UnityEngine;
using System.Collections;

public class TrackPlayer : MonoBehaviour {

	public Transform player;
	private Vector3 diff;

	// Use this for initialization
	void Start () {
		diff = player.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.position - diff;
	}
}
