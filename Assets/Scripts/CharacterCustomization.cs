using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterCustomization : MonoBehaviour {

	private byte[] EYES = { 37, 40 };
	private byte[] GORE = { 255 };

	public Color32 skinColor;
	public Color32 shirtColor1;
	public Color32 shirtColor2;
	public Color32 pantsColor;
	
	// Each character component
	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body;
	public PicaVoxel.Volume legs;
	public PicaVoxel.Volume arms;
	public PicaVoxel.Volume gunz;


	Dictionary<Color32, byte[]> bodyColors = new Dictionary<Color32, byte[]>();
	Dictionary<Color32, byte[]> headColors = new Dictionary<Color32, byte[]>();
	Dictionary<Color32, byte[]> legColors = new Dictionary<Color32, byte[]>();
	Dictionary<Color32, byte[]> armColors = new Dictionary<Color32, byte[]>();


	// Use this for initialization
	void Start () {
		string bodyString = "0 0 13 70 73; 1 14 69; 2 58 59 44 45 30 31 16 17";
		bodyColors = Parse(bodyString, new Color32[]{ pantsColor, shirtColor1, shirtColor2 });
//		bodyColors.Add(pantsColor, Merge(Range(0, 13), Range(70, 73)));
//		bodyColors.Add(shirtColor1, Range(14, 69));
//		bodyColors.Add(shirtColor2, Merge(Range(58, 59), Range(44, 45), Range(30, 31), Range(16, 17)));

		headColors.Add(new Color32(50, 50, 50, 0), EYES);

		legColors.Add(new Color32(50, 50, 50, 0), Range(0, 0));
		legColors.Add(pantsColor, Range(1, 1));

		armColors.Add(shirtColor1, Range(1, 3));

		


		ColorCharacter();
	}

	private Dictionary<Color32, byte[]> Parse(string palette, Color32[] colors) {
		Dictionary<Color32, byte[]> dict = new Dictionary<Color32, byte[]>();
		List<string> strings = palette.Split(';').ToList();
		strings.ForEach(str => str.Trim());
		foreach (string s in strings) {
			string[] ranges = s.Trim().Split(' ');
			Color32 color = colors[int.Parse(ranges[0])];
			if (!dict.ContainsKey(color))
				dict.Add(color, new byte[0]);
			for (int i = 1; i < ranges.Length; i += 2) {
				int a = int.Parse(ranges[i]);
				int b = int.Parse(ranges[i + 1]);
				dict[color] = Merge(dict[color], Range(a, b));
			}
		}

		return dict;
	}

	public void ColorCharacter() {
		PicaVoxel.Volume[] volumez = { head, body, legs, arms, gunz };
		Dictionary<Color32, byte[]>[] palettes = { headColors, bodyColors, legColors, armColors, armColors };
		for (int i = 0; i < volumez.Length; i++) {
			PicaVoxel.Volume volume = volumez[i];
			Dictionary<byte, Color32> palette = (palettes[i] == null) ? null : BytesToColors(palettes[i]);

			foreach (PicaVoxel.Frame frame in volume.Frames) {
				for (int x = 0; x < frame.XSize; x++) {
					for (int y = 0; y < frame.YSize; y++) {
						for (int z = 0; z < frame.ZSize; z++) {
							PicaVoxel.Voxel? voxq = frame.GetVoxelAtArrayPosition(x, y, z);
							PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
							if (voxq == null || vox.State != PicaVoxel.VoxelState.Active)
								continue;

							if (palette != null && palette.ContainsKey(vox.Value)) {
								Color32 c = palette[vox.Value];
								// DISCOLORATION FACTOR (maybe disable this randomness for later optimization)
								int r = 5;
								Color32 d = new Color32((byte)(c.r + Random.Range(-r, r)),
									(byte)(c.g + (byte)Random.Range(-r, r)),
									(byte)(c.b + (byte)Random.Range(-r, r)), (byte)0); 
								vox.Color = d;
							} else if (vox.Value == 255) {
								// guts
								byte gb = (byte)Random.Range(0, 30);
								vox.Color = new Color32((byte)(120 + Random.Range(0, 60)), gb, gb, 0);
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
				if (!res.ContainsKey(b))
					res.Add(b, color);
				res[b] = color;
			}
		}
		return res;
	}

	// Returns a byte array from start to end (both inclusive)
	private static byte[] Range(int start, int end) {
		byte startByte = (byte)start;
		byte endByte = (byte)end;
		byte[] res = new byte[end - start + 1];
		for (byte i = startByte; i <= endByte; i++) {
			res[i - start] = i;
		}
		return res;
	}

	private static byte[] Merge(params byte[][] lists) {
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
	private static void DebugBytes(byte[] bytes) {
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
