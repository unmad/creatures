using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class tester : MonoBehaviour {

	myUI ui;
	public GameObject te;
	public Component[] comps;

	// Use this for initialization
	void Start () {

		comps = te.GetComponents(typeof(Component));

		foreach (var comp in comps){

			Debug.Log (comp.GetType());
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
