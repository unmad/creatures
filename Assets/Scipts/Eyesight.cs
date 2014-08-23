using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Eyesight : MonoBehaviour {
	string corTag = "Corpse";
	string plTag = "Plant";
	string creTag = "Creature";
	GameObject creObj;
	CreatureGenerator cg;

	void Start () {
		creObj = transform.parent.gameObject;
		cg = CreatureGenerator.Instance;
	}
	
	void Update () {
	
	}

	void OnTriggerEnter (Collider col) {

		if (col.gameObject.tag == plTag){
			creObj.SendMessage("SeeFood", col.transform);
		}

		if (col.gameObject.tag == corTag){
			creObj.SendMessage("SeeCorpse", col.transform);
		}
		
		if (col.gameObject.tag == creTag){
			Creature cre = col.gameObject.GetComponent<Creature>();
			if (cre.TypeID != creObj.GetComponent<Creature>().TypeID && cg.CTypes[cre.TypeID].eatMeat){ //если не такой же как я и плотоядный значит враг

				creObj.SendMessage("SeeEnemy", col.transform);
			}
		}
	}
	
	void OnTriggerExit (Collider col){

		if (col.gameObject.tag == plTag)
			creObj.SendMessage("EscFood", col.transform);
		if (col.gameObject.tag == creTag)
			creObj.SendMessage("EscEnemy", col.transform);
	}

}
