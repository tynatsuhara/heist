using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCustomization : MonoBehaviour {

	private byte[] EYES = {37, 40};
	private byte[] GORE = {255};

	public Color32 skinColor;
	
	// Each character component
	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body;
	public PicaVoxel.Volume legs;
	public PicaVoxel.Volume arms;
	public PicaVoxel.Volume gunz;

	// Use this for initialization
	void Start () {
		GenerateNakedness();
	}

	public void GenerateNakedness() {
		PicaVoxel.Volume[] volumez = {head, body, legs, arms, gunz};
		foreach (PicaVoxel.Volume volume in volumez) {
			foreach (PicaVoxel.Frame frame in volume.Frames) {
				for (int x = 0; x < frame.XSize; x++) {
					for (int y = 0; y < frame.YSize; y++) {
						for (int z = 0; z < frame.ZSize; z++) {
							PicaVoxel.Voxel? voxq = frame.GetVoxelAtArrayPosition(x, y, z);
							PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
							if (voxq == null || vox.State != PicaVoxel.VoxelState.Active)
								continue;

							if (vox.Value == 255) {
								byte gb = (byte)Random.Range(0, 30);
								vox.Color = new Color32((byte)(120 + Random.Range(0, 60)), gb, gb, 0);
							} else if (volume == head && (vox.Value == 37 || vox.Value == 40)) {
								vox.Color = new Color32(50, 50, 50, 0);
							} else if (volume == head || volume == body || volume == legs ||
								(volume == arms && vox.Value <= 4) || (volume == gunz && vox.Value <= 4)) {
								vox.Color = skinColor;
							}
							frame.SetVoxelAtArrayPosition(new PicaVoxel.PicaVoxelPoint(new Vector3(x, y, z)), vox);
						}
					}
				}
				frame.UpdateChunks(true);
			}
		}
	}

	// Takes a dict from colors to byte[] and returns a dict
	// mapping bytes to colors
	private Dictionary<byte, Color32> BytesToColors(Dictionary<Color32, byte[]> dict) {
		Dictionary<byte, Color32> res = new Dictionary<byte, Color32>();
		foreach (Color32 color in dict.Keys) {
			foreach (byte b in dict[color]) {
				res.Add(b, color);
			}
		}
		return res;
	}

	// Returns a byte array from start to end (both inclusive)
	private byte[] Range(int start, int end) {
		byte startByte = (byte)start;
		byte endByte = (byte)end;
		byte[] res = new byte[end - start + 1];
		for (byte i = startByte; i <= endByte; i++) {
			res[i - start] = i;
		}
		return res;
	}

	private byte[] Merge(params byte[][] lists) {
		int len = 0;
		foreach (byte[] b in lists) {
			len += b.Length;
		}
		byte[] res = new byte[len];
		int index = 0;
		foreach (byte[] bytes in lists) {
			foreach (byte b in bytes) {
				res[index++] = b;
			}
		}
		return res;
	}

	// Private helper method for testing
	private void DebugBytes(byte[] bytes) {
		if (bytes.Length == 0) {
			Debug.Log("[]");
		} else if (bytes.Length == 1) {
			Debug.Log("[" + (int)bytes[0] + "]");
		} else {
			string s = "[" + (int)bytes[0];
			for (int i = 1; i < bytes.Length; i++) {
				s += "," + (int)bytes[i];
			}
			Debug.Log(s + "]");
		}
	}
}
