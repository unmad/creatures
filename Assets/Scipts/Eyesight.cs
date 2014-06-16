using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Eyesight : MonoBehaviour {

	string plTag = "Plant";
	string meea = "MeatEater";
	GameObject cre;

	void Start () {
		cre = transform.parent.gameObject;
	}
	
	void Update () {
	
	}

	void OnTriggerEnter (Collider col) {

		if (col.gameObject.tag == plTag){
			cre.SendMessage("SeeFood", col);
		}
		
		if (col.gameObject.tag == meea){
			cre.SendMessage("SeeEnemy", col);
		}
	}
	
	void OnTriggerExit (Collider col){

		if (col.gameObject.tag == plTag)
			cre.SendMessage("EscFood", col);
		if (col.gameObject.tag == meea)
			cre.SendMessage("EscEnemy", col);
	}

}
