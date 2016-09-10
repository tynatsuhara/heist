﻿using UnityEngine;
using System.Collections;

public class WorldBlood : MonoBehaviour {

	public static WorldBlood instance;

	public static Color32[] rainbow = {
		Color.red
	};

	void Awake() {
		instance = this;
	}
	
	public void BleedFrom(GameObject bleeder, Vector3 worldLocation, bool randomizePosition = true) {
		PicaVoxel.Volume vol = LevelBuilder.instance.FloorTileAt(worldLocation);
		if (vol == null || vol.transform.root == bleeder.transform)
			return;
		worldLocation.y = -.05f;
		Vector3 circlePos = Random.insideUnitCircle * (randomizePosition ? .5f : 0f);
		Vector3 pos = worldLocation - new Vector3(circlePos.x, .03f, circlePos.y);
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

	public Color32 BloodColor() {
		if (Cheats.instance.IsCheatEnabled("pride")) {
			return (Color32) Color.green;
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
