using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PicaVoxel;

public class Recolor : MonoBehaviour {

	private PicaVoxel.Volume volume;

	// Use this for initialization
	void Start () {
		volume = GetComponent<PicaVoxel.Volume>();
		//Colorize(null);
	}

	public void Colorize(Dictionary<byte, Color32> colorMap) {
		for (int x = 0; x < volume.XSize; x++) {
			for (int y = 0; y < volume.YSize; y++) {
				for (int z = 0; z < volume.ZSize; z++) {
					Voxel? voxq = volume.GetVoxelAtArrayPosition(x, y, z);
					Voxel vox = (Voxel)voxq;
					if (voxq != null && vox.State == VoxelState.Active) {
						vox.Color = Color.red;
						volume.SetVoxelAtArrayPosition(new PicaVoxelPoint(new Vector3(x, y, z)), vox);	
					} 
				}
			}
		}
	}
}
