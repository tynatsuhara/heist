using UnityEngine;
using System.Linq;

public class PlayerCamera : MonoBehaviour {

	public PlayerControls player;
	public float minZoom;
	public float maxZoom;
	public float rotationAngle;
	public float rotationSpeed;
	public Camera cam;

	private Transform[] targets;

	private float power;
	private float duration;
	private float timeElapsed;
	private bool rotating;
	private Quaternion rotationGoal;
	private Vector3 diff;

	private float startTime;

	void Start () {
		rotationGoal = transform.rotation;
		diff = transform.localPosition;
		cam.GetComponent<AudioListener>().enabled = player.id == 1;  // can only have one at a time
		cam.cullingMask |= (1 << LayerMask.NameToLayer("textCam" + (player.id - 1)));
	}
	
	void Update () {
		UpdatePosition();
	}

	private int lastDpadValue;
	private void UpdatePosition() {
		transform.localPosition = diff;
		transform.position = AveragePointBetweenTargets();
		Vector3 cameraLookAtPosition = transform.position;
		cam.transform.LookAt(transform.position);

		int newDpadValue = Input.GetAxis("DPX" + player.id) == 0 ? 0 : (int) Mathf.Sign(Input.GetAxis("DPX" + player.id));
		bool pressedDpad = newDpadValue != lastDpadValue;
		lastDpadValue = newDpadValue;

		// rotation
		bool rotateButtonPress = (player.id == 1 && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C))) || pressedDpad;
		if (rotateButtonPress) {
			startTime = Time.realtimeSinceStartup;
			int dir = Input.GetKeyDown(KeyCode.Z) ? -1 : 1;
			if (pressedDpad)
				dir = newDpadValue;
			Quaternion tempRot = transform.rotation;
			transform.rotation = rotationGoal;
			transform.RotateAround(cameraLookAtPosition, Vector3.up, -rotationAngle * dir);
			rotationGoal = transform.rotation;
			transform.RotateAround(cameraLookAtPosition, Vector3.up, rotationAngle * dir);
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
		float zoom = player.id == 1 && Input.GetAxis("Mouse ScrollWheel") != 0 
				? Input.GetAxis("Mouse ScrollWheel") 
				: Input.GetAxis("DPY" + player.id) * .5f;
		cam.orthographicSize = Mathf.Min(Mathf.Max(minZoom, cam.orthographicSize - zoom), maxZoom);
	}

	public void Shake(float power, float duration) {
		this.power = power;
		this.duration = duration;
		timeElapsed = 0;
	}

	// pre: at least one player in the players array
	private Vector3 AveragePointBetweenTargets() {
		if (player == null)
			return transform.position;
		if (targets == null || targets.Count() == 0)
			return player.transform.position;

		Vector3 minValues = player.transform.position;
		Vector3 maxValues = minValues;
		for (int i = 0; i < targets.Length; i++) {
			minValues.x = Mathf.Min(minValues.x, targets[i].position.x);
			minValues.y = Mathf.Min(minValues.y, targets[i].position.y);
			minValues.z = Mathf.Min(minValues.z, targets[i].position.z);
			maxValues.x = Mathf.Max(maxValues.x, targets[i].position.x);
			maxValues.y = Mathf.Max(maxValues.y, targets[i].position.y);
			maxValues.z = Mathf.Max(maxValues.z, targets[i].position.z);
		}
		return new Vector3(minValues.x + (maxValues.x - minValues.x) / 2,
						   minValues.y + (maxValues.y - minValues.y) / 2,
						   minValues.z + (maxValues.z - minValues.z) / 2);
	}
}
