using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PicaVoxel;

public class Recolor : MonoBehaviour {

	private PicaVoxel.Volume volume;

	void Start () {
		volume = GetComponent<PicaVoxel.Volume>();
	}

	public Dictionary<Vector3, Color> Colorize(Dictionary<byte, Color> colorMap) {
		var resultMap = GetState();
		for (int x = 0; x < volume.XSize; x++) {
			for (int y = 0; y < volume.YSize; y++) {
				for (int z = 0; z < volume.ZSize; z++) {
					Voxel? voxq = volume.GetVoxelAtArrayPosition(x, y, z);
					Voxel vox = (Voxel)voxq;
					if (voxq != null && vox.State == VoxelState.Active && colorMap.ContainsKey(vox.Value)) {
						Color color = colorMap[vox.Value];
						vox.Color = color;
						volume.SetVoxelAtArrayPosition(new PicaVoxelPoint(new Vector3(x, y, z)), vox);
					} 
				}
			}
		}
		return resultMap;
	}

	public Dictionary<Vector3, Color> Colorize(Dictionary<Vector3, Color> colorMap) {
		var resultMap = GetState();
		foreach (Vector3 v in colorMap.Keys) {
			Voxel? voxq = volume.GetVoxelAtArrayPosition((int)v.x, (int)v.y, (int)v.z);
			Voxel vox = (Voxel)voxq;
			if (voxq != null && vox.State == VoxelState.Active) {
				Color color = colorMap[v];
				vox.Color = color;
				volume.SetVoxelAtArrayPosition(new PicaVoxelPoint(v), vox);
			}
		}
		return resultMap;
	}

	public Dictionary<Vector3, Color> ColorAll(Color color) {
		var resultMap = GetState();
		for (int x = 0; x < volume.XSize; x++) {
			for (int y = 0; y < volume.YSize; y++) {
				for (int z = 0; z < volume.ZSize; z++) {
					Voxel? voxq = volume.GetVoxelAtArrayPosition(x, y, z);
					Voxel vox = (Voxel)voxq;
					if (voxq != null && vox.State == VoxelState.Active) {
						vox.Color = color;
						volume.SetVoxelAtArrayPosition(new PicaVoxelPoint(new Vector3(x, y, z)), vox);
					} 
				}
			}
		}
		return resultMap;
	}

	private Dictionary<Vector3, Color> GetState() {
		Dictionary<Vector3, Color> resultMap = new Dictionary<Vector3, Color>();
		for (int x = 0; x < volume.XSize; x++) {
			for (int y = 0; y < volume.YSize; y++) {
				for (int z = 0; z < volume.ZSize; z++) {
					Voxel? voxq = volume.GetVoxelAtArrayPosition(x, y, z);
					Voxel vox = (Voxel)voxq;
					if (voxq != null && vox.State == VoxelState.Active) {
						resultMap.Add(new Vector3(x, y, z), vox.Color);
					} 
				}
			}
		}
		return resultMap;
	}

	private Dictionary<Vector3, Color> oldState;
	public void Flash(Color color, float duration) {
		if (oldState != null)
			return;

		oldState = ColorAll(color);
		Invoke("Unflash", duration);
	}

	private void Unflash() {
		Colorize(oldState);
		oldState = null;
	}
}
