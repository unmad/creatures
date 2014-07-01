using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyesightME : MonoBehaviour {

	string plTag = "Creature";
	string corTag = "Corpse";
	string meea = "MeatEater";
	GameObject cre;

	void Start () {
		cre = transform.parent.gameObject;
	}
	
	void Update () {
	
	}

	void OnTriggerEnter (Collider col) {

		if (col.gameObject.tag == plTag){
			cre.SendMessage("SeeLiveFood", col.transform);
		}

		if (col.gameObject.tag == corTag){
			cre.SendMessage("SeeFood", col.transform);
		}

		
//		if (col.gameObject.tag == meea){
//			cre.SendMessage("SeeEnemy", col.transform);
//		}
	}
	
	void OnTriggerExit (Collider col){

		if (col.gameObject.tag == plTag)
			cre.SendMessage("EscFood", col.transform);
//		if (col.gameObject.tag == meea)
//			cre.SendMessage("EscEnemy", col.transform);
	}

}
