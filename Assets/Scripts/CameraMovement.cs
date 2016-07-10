using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public static CameraMovement instance;
	public float minZoom;
	public float maxZoom;
	public float rotationAngle;
	private Camera cam;

	public Transform player;

	private Vector3 diff;

	private float power;
	private float duration;
	private float timeElapsed;

	private bool rotating;

	void Start () {
		instance = this;
		cam = GetComponent<Camera>();
		diff = player.position - transform.position;
	}
	
	void Update () {
		transform.position = player.position - diff;
		transform.LookAt(player.position);

		// rotation
		bool rotateButtonPress = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C);
		if (!rotating && rotateButtonPress) {
			int dir = Input.GetKeyDown(KeyCode.Z) ? -1 : 1;
			transform.RotateAround(player.position, Vector3.up, -rotationAngle * dir);
			diff = player.position - transform.position;
		} 

		// shaking
		if (timeElapsed < duration) {
			transform.position += Random.insideUnitSphere * power * (duration - timeElapsed);
			timeElapsed += Time.deltaTime;
		}

		// zoom in/out
		cam.orthographicSize = Mathf.Min(Mathf.Max(minZoom, cam.orthographicSize - Input.GetAxis("Mouse ScrollWheel")), maxZoom);
	}

	public void Shake(float power, float duration) {
		this.power = power;
		this.duration = duration;
		timeElapsed = 0;
	}
}
