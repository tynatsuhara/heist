using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public static CameraMovement instance;
	public float minZoom;
	public float maxZoom;
	public float rotationAngle;
	public float rotationSpeed;
	public Camera cam;

	public Transform player;

	private float power;
	private float duration;
	private float timeElapsed;
	private bool rotating;
	private Quaternion rotationGoal;
	private Vector3 diff;

	private float startTime;

	void Start () {
		instance = this;
		rotationGoal = transform.rotation;
		diff = transform.localPosition;
	}
	
	void Update () {
		UpdatePosition();
	}

	private void UpdatePosition() {
		transform.localPosition = diff;
		transform.position = player.transform.position;
		cam.transform.LookAt(player.position);

		// rotation
		bool rotateButtonPress = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C);
		if (rotateButtonPress) {
			startTime = Time.realtimeSinceStartup;
			int dir = Input.GetKeyDown(KeyCode.Z) ? -1 : 1;
			Quaternion tempRot = transform.rotation;
			transform.rotation = rotationGoal;
			transform.RotateAround(player.position, Vector3.up, -rotationAngle * dir);
			rotationGoal = transform.rotation;
			transform.RotateAround(player.position, Vector3.up, rotationAngle * dir);
			transform.rotation = tempRot;
			rotating = true;
		} 
		if (rotating) {
			// not linked to deltaTime, since time is frozen when paused
			float realTimeElapsed = (Time.realtimeSinceStartup - startTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotationGoal, rotationSpeed * realTimeElapsed);
		}

		// shaking
		if (timeElapsed < duration && !GameManager.paused) {
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
