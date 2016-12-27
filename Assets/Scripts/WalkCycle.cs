using UnityEngine;
using System.Collections;

public class WalkCycle : MonoBehaviour {

	private PicaVoxel.Volume volume;
	private float timeElapsed = .1f;
	private int startFrame;
	private int endFrame;

	public float interval;
	public int standingFirstFrame;
	public int standingLastFrame;
	public int walkingFirstFrame;
	public int walkingLastFrame;

	public bool isWalking {
		get { return startFrame == walkingFirstFrame; }
	}

	void Awake () {
		volume = GetComponent<PicaVoxel.Volume>();
		StopWalk();
	}

	public void StartWalk() {
		startFrame = walkingFirstFrame;
		endFrame = walkingLastFrame;
		timeElapsed = 0f;
		volume.SetFrame(walkingFirstFrame);
	}

	public void StopWalk(bool immediate = false) {
		startFrame = standingFirstFrame;
		endFrame = standingLastFrame;
		timeElapsed = 0f;
		if (immediate)
			volume.SetFrame(standingFirstFrame);
	}
	
	void Update () {
		if (GameManager.paused) {
			return;
		}

		timeElapsed += Time.deltaTime;
		if (timeElapsed >= interval) {
			timeElapsed %= interval;
			int newFrame = volume.CurrentFrame + 1;
			if (newFrame > endFrame) {
				newFrame %= (endFrame + 1);
				newFrame += startFrame;
			}
			volume.SetFrame(newFrame);
		}
	}
}
