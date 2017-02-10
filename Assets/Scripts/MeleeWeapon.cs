using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MeleeWeapon : Gun {

	public float swingSpeed;
	private bool canSwing = true;
	private Dictionary<byte, Color32> originalColors;	
	private Dictionary<byte, Vector3[]> locations;	
	public int swingDirection;
	public DamageType damageType;
	public byte bloodyVoxelMin;
	public byte bloodyVoxelMax;


	public override void Start() {
		base.Start();
		SaveColors();
	}

	public override bool Shoot() {
		bool couldSwing = canSwing;
		Melee(DamageType.SLICE, swingDirection);
		return couldSwing;
	}
	private void ResetShoot() {
		canSwing = true;
		volume.SetFrame(GUN_BASE_FRAME);
	}

	public override void Melee(DamageType type = DamageType.MELEE, int dir = 0) {
		if (!canSwing)
			return;
		canSwing = false;
		volume.SetFrame(ANIM_START_FRAME);
		bool couldSwing = canSwing;		
		base.Melee(type, swingDirection);
		Invoke("ResetShoot", swingSpeed);		
	}

	public override void Release() {}
	public override void Reload() {}
	public override void CancelReload() {}
	public override bool NeedsToReload() { return false; }
	
	public override void UpdateUI() {
		player.playerUI.UpdateAmmo(name);
	}

	protected override void MeleeHitPlayerCallback() {
		PaintBlood();
	}

	private List<byte> bytesToRecolor = new List<byte>();
	private void PaintBlood() {
		if (bloodyVoxelMax <= bloodyVoxelMin)
			return;
		int bloodTimes = Random.Range(3, bloodyVoxelMax - bloodyVoxelMin);
		byte index = (byte) Random.Range(bloodyVoxelMin, bloodyVoxelMax - bloodTimes);		
		for (byte i = index; i < index + bloodTimes; i++) {
			if (Random.Range(0, 3) == 0 || !originalColors.ContainsKey(i) || !locations.ContainsKey(i))
				continue;
			PicaVoxel.Volume v = GetComponent<PicaVoxel.Volume>();
			bytesToRecolor.Add(i);
			Invoke("RemoveBlood", Random.Range(10, 45));
			for (int frame = 0; frame < v.NumFrames; frame++) {
				Vector3 pos = locations[i][frame];
				PicaVoxel.Voxel? voxel = v.Frames[frame].GetVoxelAtArrayPosition((int) pos.x, (int) pos.y, (int) pos.z);
				PicaVoxel.Voxel vv = voxel.Value;
				vv.Color = WorldBlood.instance.BloodColor();
				v.Frames[frame].SetVoxelAtArrayPosition((int)pos.x, (int)pos.y, (int)pos.z, vv);
				v.UpdateChunks(true);				
			}
		}
	}

	private void RemoveBlood() {
		byte b = bytesToRecolor[0];
		bytesToRecolor.RemoveAt(0);
		if (bytesToRecolor.Contains(b))
			return;
		PicaVoxel.Volume v = GetComponent<PicaVoxel.Volume>();			
		for (int frame = 0; frame < v.NumFrames; frame++) {
			Vector3 pos = locations[b][frame];
			PicaVoxel.Voxel? voxel = v.Frames[frame].GetVoxelAtArrayPosition((int) pos.x, (int) pos.y, (int) pos.z);
			PicaVoxel.Voxel vv = voxel.Value;
			vv.Color = originalColors[b];
			v.Frames[frame].SetVoxelAtArrayPosition((int)pos.x, (int)pos.y, (int)pos.z, vv);
			v.UpdateChunks(true);				
		}
	}

	private void SaveColors() {
		PicaVoxel.Volume v = GetComponent<PicaVoxel.Volume>();
		originalColors = new Dictionary<byte, Color32>();
		locations = new Dictionary<byte, Vector3[]>();
		for (int frame = 0; frame < v.NumFrames; frame++) {
			for (int x = 0; x < v.XSize; x++) {
				for (int y = 0; y < v.YSize; y++) {
					for (int z = 0; z < v.ZSize; z++) {
						PicaVoxel.Voxel? vox = v.Frames[frame].GetVoxelAtArrayPosition(x, y, z);
						if (!vox.HasValue || 
							!vox.Value.Active || 
							vox.Value.Value >= bloodyVoxelMax || 
							vox.Value.Value < bloodyVoxelMin)
							continue;
						if (!originalColors.ContainsKey(vox.Value.Value)) {
							originalColors.Add(vox.Value.Value, vox.Value.Color);
							locations.Add(vox.Value.Value, new Vector3[v.NumFrames]);
						}
						locations[vox.Value.Value][frame] = new Vector3(x, y, z);
					}
				}
			}
		}
	}
}
