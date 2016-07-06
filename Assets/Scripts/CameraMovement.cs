using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public static CameraMovement mainCamera;

	public Transform player;

	private Vector3 diff;

	private float power;
	private float duration;
	private float timeElapsed;

	void Start () {
		mainCamera = this;
		diff = player.position - transform.position;
	}
	
	void Update () {
		transform.position = player.position - diff;

		if (timeElapsed < duration) {
			transform.position += Random.insideUnitSphere * power * (duration - timeElapsed);
			timeElapsed += Time.deltaTime;
		}
	}

	public void Shake(float power, float duration) {
		this.power = power;
		this.duration = duration;
		timeElapsed = 0;
	}
}
