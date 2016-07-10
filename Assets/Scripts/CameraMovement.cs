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
	private Vector3 goalPosDiff;
	private Quaternion goalRot;

	void Start () {
		instance = this;
		cam = GetComponent<Camera>();
		diff = player.position - transform.position;
	}
	
	void Update () {
		transform.position = player.position - diff;

		// rotation
		if (!rotating && Input.GetKeyDown(KeyCode.Z)) {
			transform.RotateAround(player.position, Vector3.up, rotationAngle);
			goalPosDiff = player.position - transform.position;
			goalRot = transform.rotation;
			rotating = true;
			transform.RotateAround(player.position, Vector3.up, -rotationAngle);
		} else if (!rotating && Input.GetKeyDown(KeyCode.C)) {
			transform.RotateAround(player.position, Vector3.up, -rotationAngle);
			goalPosDiff = player.position - transform.position;
			goalRot = transform.rotation;
			rotating = true;
			transform.RotateAround(player.position, Vector3.up, rotationAngle);
		}
		if (rotating) {
			transform.position = Vector3.Slerp(transform.position, player.position - goalPosDiff, .1f);
			transform.rotation = Quaternion.Slerp(transform.rotation, goalRot, .1f);
			diff = player.position - transform.position;
			if ((transform.position - player.position + goalPosDiff).magnitude < .05f) {
				rotating = false;
			}
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
