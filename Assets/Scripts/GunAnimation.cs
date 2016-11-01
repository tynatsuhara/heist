using UnityEngine;
using System.Collections;

public class GunAnimation : MonoBehaviour {

	public float interval;

	private PicaVoxel.Volume volume;
	private bool playing;
	private float timer;

	void Awake () {
		volume = GetComponent<PicaVoxel.Volume>();
	}

	public void Shoot() {
		volume.SetFrame(2);
		playing = true;
	}

	void Update () {
		if (GameManager.paused || !playing || volume.CurrentFrame == 1)
			return;

		timer += Time.deltaTime;
		if (timer >= interval) {
			timer = 0;
			volume.SetFrame((volume.CurrentFrame + 1) % volume.NumFrames);
		}
		if (volume.CurrentFrame == 0) {
			playing = false;			
		}
	}
}
