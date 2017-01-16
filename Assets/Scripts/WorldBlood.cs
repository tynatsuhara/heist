using UnityEngine;
using System.Collections;

public class WorldBlood : MonoBehaviour {

	public static WorldBlood instance;

	private Color32[] options = {
		new Color32(255, 94, 94, 255),
		new Color32(255, 160, 94, 255),
		new Color32(246, 255, 0, 255),
		new Color32(94, 255, 108, 255),
		new Color32(115, 173, 255, 255),
		new Color32(255, 115, 255, 255)
	};

	void Awake() {
		instance = this;
	}
	
	public void BleedFrom(GameObject bleeder, Vector3 worldLocation, bool randomizePosition = true) {
		Vector3 circlePos = Random.insideUnitCircle * (randomizePosition ? Random.Range(.3f, .5f) : 0);
		worldLocation.y = -.05f;
		Vector3 pos = worldLocation - new Vector3(circlePos.x, 0f, circlePos.y);
		PicaVoxel.Volume vol = LevelBuilder.instance.FloorTileAt(pos);
		if (vol == null)
			return;
		PicaVoxel.Voxel? voxq = vol.GetVoxelAtWorldPosition(pos);
		if (voxq == null)
			return;
		PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
		if (vox.State == PicaVoxel.VoxelState.Active) {
			vox.Color = BloodColor();
			vol.SetVoxelAtWorldPosition(pos, vox);
		}
	}

	public Color32 BloodColor() {
		if (Cheats.instance != null && Cheats.instance.IsCheatEnabled("pride")) {
			return options[Random.Range(0, options.Length)];
		}

		byte gb = (byte)Random.Range(0, 30);
		return new Color32((byte)(120 + Random.Range(0, 60)), gb, gb, 0);
	}

	private PicaVoxel.Volume BledOnVolume(Vector3 worldLocation, out RaycastHit hit) {
		if (!Physics.Raycast(worldLocation, Vector3.down, out hit))
			return null;
		return hit.collider.GetComponentInParent<PicaVoxel.Volume>();
	}
}
