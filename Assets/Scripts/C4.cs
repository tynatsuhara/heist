using UnityEngine;
using System.Collections;

public class C4 : Explosive {

	public int amount;
	public GameObject prefab;

	private GameObject placed;

	public override void Trigger() {
		if (amount == 0)
			return;
		if (placed != null) {
			Explode(placed);
			placed = null;
		} else {
			amount--;
			RaycastHit hit;
			GameObject wall = GetComponent<PlayerControls>().FacingObstruction(out hit, 1f);
			placed = Instantiate(prefab) as GameObject;			
			if (wall == null || wall.GetComponentInParent<Character>() != null) {
				Vector3 pos = transform.position + transform.forward * .5f;
				pos.y = 0;
				placed.transform.position = pos;
				placed.transform.eulerAngles = transform.eulerAngles + Vector3.up * Random.Range(-20, 20);
			} else {
				placed.transform.eulerAngles = wall.transform.eulerAngles + Vector3.forward * 90f;
				PicaVoxel.Volume vol = wall.GetComponentInParent<PicaVoxel.Volume>();
				if (vol != null) {
					Vector3 dir = (hit.point - transform.position).normalized * .01f;
					placed.transform.position = transform.position;					
					PicaVoxel.Voxel? v = vol.GetVoxelAtWorldPosition(placed.transform.position);
					while (!v.HasValue || !v.Value.Active) {
						placed.transform.position += dir;
						v = vol.GetVoxelAtWorldPosition(placed.transform.position);						
					}
 				}
			}
		}
	}
}
