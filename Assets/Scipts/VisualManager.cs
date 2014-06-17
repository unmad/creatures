using UnityEngine;
using System.Collections;

public class VisualManager : MonoBehaviour {

	public Transform visual;
	float scale;
	float minScale;
	float maxScale;


	void Start () {
		visual.localScale = Vector3.one * (Mathf.Lerp(minScale, maxScale, (float)SendMessage("Size") / SendMessage("MaxSize")));
	}

	void Update () {
	
	}

	public void SetMinScale (float i){minScale = i; }
	
	public void SetMaxScale (float i){maxScale = i;	}

	public void SetScale (float i){scale = i;}

	public float Scale {get {return scale;}}
}
