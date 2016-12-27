using UnityEngine;
using System.Linq;

public class CameraMovement : MonoBehaviour {

	public static CameraMovement instance;
	public float minZoom;
	public float maxZoom;
	public float rotationAngle;
	public float rotationSpeed;
	public Camera cam;

	private Transform[] players;

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
		Transform[] newPlayers = GameManager.players.Where(x => x.isAlive).Select(x => x.transform).ToArray();
		if (newPlayers.Length > 0)		
			players = newPlayers;
		UpdatePosition();
	}

	private int lastDpadValue;
	private void UpdatePosition() {
		transform.localPosition = diff;
		transform.position = AveragePointBetweenPlayers();
		Vector3 cameraLookAtPosition = transform.position;
		cam.transform.LookAt(transform.position);

		int newDpadValue = Input.GetAxis("DPX1") == 0 ? 0 : (int) Mathf.Sign(Input.GetAxis("DPX1"));
		bool pressedDpad = newDpadValue != lastDpadValue;
		lastDpadValue = newDpadValue;

		// rotation
		bool rotateButtonPress = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C) || pressedDpad;
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
		float zoom = Input.GetAxis("Mouse ScrollWheel") != 0 ? Input.GetAxis("Mouse ScrollWheel") : Input.GetAxis("DPY1") * .5f;
		cam.orthographicSize = Mathf.Min(Mathf.Max(minZoom, cam.orthographicSize - zoom), maxZoom);
	}

	public void Shake(float power, float duration) {
		this.power = power;
		this.duration = duration;
		timeElapsed = 0;
	}

	// pre: at least one player in the players array
	private Vector3 AveragePointBetweenPlayers() {
		Vector3 minValues = new Vector3(players[0].position.x, players[0].position.y, players[0].position.z);
		Vector3 maxValues = minValues;
		for (int i = 1; i < players.Length; i++) {
			minValues.x = Mathf.Min(minValues.x, players[i].position.x);
			minValues.y = Mathf.Min(minValues.y, players[i].position.y);
			minValues.z = Mathf.Min(minValues.z, players[i].position.z);
			maxValues.x = Mathf.Max(maxValues.x, players[i].position.x);
			maxValues.y = Mathf.Max(maxValues.y, players[i].position.y);
			maxValues.z = Mathf.Max(maxValues.z, players[i].position.z);
		}
		return new Vector3(minValues.x + (maxValues.x - minValues.x) / 2,
						   minValues.y + (maxValues.y - minValues.y) / 2,
						   minValues.z + (maxValues.z - minValues.z) / 2);
	}
}
