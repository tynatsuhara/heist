using UnityEngine;
using System.Collections;

public class GunAnimation : MonoBehaviour {

	public float interval;

	private PicaVoxel.Volume volume;
	private bool playing;
	private float timer;

	void Start () {
		volume = GetComponent<PicaVoxel.Volume>();
	}

	public void Shoot() {
		volume.SetFrame(2);
		playing = true;
	}

	public void Hold() {
		playing = false;
		volume.SetFrame(0);
	}
	
	void Update () {
		if (GameManager.paused || !playing)
			return;

		timer += Time.deltaTime;
		if (timer > interval) {
			timer -= interval;
			volume.SetFrame((volume.CurrentFrame + 1) % volume.NumFrames);
		}
		if (volume.CurrentFrame == 0) {
			Hold();
		}
	}
}
