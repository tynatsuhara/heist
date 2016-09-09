using UnityEngine;
using System.Collections;

public class WorldBlood : MonoBehaviour {

	public static WorldBlood instance;

	void Awake() {
		instance = this;
	}
	
	public void BleedFrom(GameObject bleeder, Vector3 worldLocation, bool randomizePosition = true) {
		// 1. save the blood location (to the nearest voxel (.1) size)


		// 2. raycast to see if we hit a volume
		RaycastHit hit;
		PicaVoxel.Volume vol = BledOnVolume(worldLocation, out hit);
		if (vol == null || vol.transform.root == bleeder.transform)
			return;
		
		Vector3 circlePos = Random.insideUnitCircle * (randomizePosition ? .5f : 0f);
		Vector3 pos = hit.point - new Vector3(circlePos.x, .03f, circlePos.y);
		PicaVoxel.Voxel? voxq = vol.GetVoxelAtWorldPosition(pos);
		if (voxq == null)
			return;
		PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
		if (vox.State == PicaVoxel.VoxelState.Active) {
			byte gb = (byte)Random.Range(0, 30);
			vox.Color = BloodColor();
			vol.SetVoxelAtWorldPosition(pos, vox);
		}
	}

	public static Color32 BloodColor() {
		byte gb = (byte)Random.Range(0, 30);
		return new Color32((byte)(120 + Random.Range(0, 60)), gb, gb, 0);
	}

	private PicaVoxel.Volume BledOnVolume(Vector3 worldLocation, out RaycastHit hit) {
		if (!Physics.Raycast(worldLocation, Vector3.down, out hit))
			return null;
		Debug.Log(hit.collider.gameObject.name);
		return hit.collider.GetComponentInParent<PicaVoxel.Volume>();
	}
}
