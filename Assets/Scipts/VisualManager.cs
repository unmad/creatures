using UnityEngine;
using System.Collections;

public class VisualManager : MonoBehaviour {

	public Transform visual;
	float scale;
	float minScale;
	float maxScale;

	CreatureGenerator cg;


	void Start () {
		cg = CreatureGenerator.Instance;
	}

	void Update () {
	
	}

	public void SetScale (float i){
		scale = i;
		visual.localScale = Vector3.one * (Mathf.Lerp(minScale, maxScale, scale));
	}

	public void SetMinScale (float i){minScale = i; }
	
	public void SetMaxScale (float i){maxScale = i;	}
	
}
