using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public static CameraMovement mainCamera;
	public float minZoom;
	public float maxZoom;
	private Camera cam;

	public Transform player;

	private Vector3 diff;

	private float power;
	private float duration;
	private float timeElapsed;

	void Start () {
		mainCamera = this;
		cam = GetComponent<Camera>();
		diff = player.position - transform.position;
	}
	
	void Update () {
		transform.position = player.position - diff;

		if (timeElapsed < duration) {
			transform.position += Random.insideUnitSphere * power * (duration - timeElapsed);
			timeElapsed += Time.deltaTime;
		}

		cam.orthographicSize = Mathf.Min(Mathf.Max(minZoom, cam.orthographicSize - Input.GetAxis("Mouse ScrollWheel")), maxZoom);
	}

	public void Shake(float power, float duration) {
		this.power = power;
		this.duration = duration;
		timeElapsed = 0;
	}
}
