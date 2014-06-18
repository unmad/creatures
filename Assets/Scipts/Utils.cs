using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour {

	public static Transform FindNearest(Transform tra, List<Transform> tars){
		Transform t = null;
		tars.RemoveAll(o => o == null);
		
		if (tars.Count > 0){
			float dis = float.MaxValue;
			float minDis = float.MaxValue;
			
			foreach (var tar in tars) {
				dis = (tra.position - tar.position).sqrMagnitude;
				if (dis < minDis) {
					t = tar;
					minDis = dis;
				}
			}
			return t;
		} else 
			return null;
	}
}