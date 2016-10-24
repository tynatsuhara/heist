using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour {

	public GameObject cameraTop;
	public Transform hinge;
	public float rotationRange;  // assuming it starts centered
	public float rotationSpeed;  // degrees/sec
	public float stopTime;       // how long the camera stays still before reversing direction

	private bool changeDirectionInvoked;
	private bool rotatingRight;
	private float rotatedDist;
	
	void Start() {
		// randomize rotation
		cameraTop.transform.RotateAround(hinge.position, Vector3.up, -rotationRange/2);
		float delta = Random.Range(0f, rotationRange);
		rotatingRight = Random.Range(0, 2) == 1;
		rotatedDist = rotatingRight ? delta : rotationRange - delta;
		cameraTop.transform.RotateAround(hinge.position, Vector3.up, delta);				
	}

	void Update () {
		if (CanSee(GameManager.instance.player.gameObject, 80f)) {
			Debug.Log("can see player");
		}
		KeepRotating();
	}

	private void KeepRotating() {
		if (changeDirectionInvoked) {
			return;
		}
		float y = cameraTop.transform.rotation.eulerAngles.y % 360;
		if (rotatedDist > rotationRange) {
			SwapDir();
		} else {
			float delta = (rotatingRight ? 1 : -1) * rotationSpeed * Time.deltaTime;
			cameraTop.transform.RotateAround(hinge.position, Vector3.up, delta);
			rotatedDist += Mathf.Abs(delta);
		}
	}

	private void SwapDir() {
		changeDirectionInvoked = true;
		rotatingRight = !rotatingRight;
		rotatedDist = 0;
		Invoke("ResumeMoving", stopTime);
	}
	private void ResumeMoving() {
		changeDirectionInvoked = false;
		Debug.Log("resume moving");
	}

	public bool CanSee(GameObject target, float fov = 130f, float maxViewDist = 20f, float minViewDist = 1f) {
		Vector3 diff = transform.position - target.transform.position;
		if (diff.magnitude > maxViewDist || diff.magnitude < minViewDist)
			return false;
		
		Vector3 camPos = cameraTop.transform.position;
		camPos.y = GameManager.instance.player.transform.position.y;
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - target.transform.position), cameraTop.transform.right);
		float angleDegrees = 90 + Mathf.Asin(angle) * Mathf.Rad2Deg;
		if (angleDegrees > fov / 2f) {
			return false;
		}

		RaycastHit hit;
		if (Physics.Raycast(camPos, target.transform.position - camPos, out hit, maxViewDist))
			return hit.collider.transform.root.gameObject == target;

		return false;
	}
}
