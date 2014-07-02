using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyesightME : MonoBehaviour {

	string creTag = "Creature";
	string corTag = "Corpse";
	string meea = "MeatEater";
	GameObject cre;

	void Start () {
		cre = transform.parent.gameObject;
	}
	
	void Update () {
	
	}

	void OnTriggerEnter (Collider col) {

		var cTag = col.gameObject.tag;

		Debug.Log(cTag);

		if (cTag != "Plant"){

			if (col.gameObject.tag == creTag){
				cre.SendMessage("SeeLiveFood", col.transform);
				Debug.DrawLine(transform.position, col.transform.position, Color.red);
			}

			if (col.gameObject.tag == corTag){
				cre.SendMessage("SeeFood", col.transform);
				Debug.DrawLine(transform.position, col.transform.position, Color.yellow);
			}
		}
		
//		if (col.gameObject.tag == meea){
//			cre.SendMessage("SeeEnemy", col.transform);
//		}
	}
	
	void OnTriggerExit (Collider col){

		if (col.gameObject.tag == creTag)
			cre.SendMessage("EscFood", col.transform);
//		if (col.gameObject.tag == meea)
//			cre.SendMessage("EscEnemy", col.transform);
	}

}
