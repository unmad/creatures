using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyesightME : MonoBehaviour {
	
	string corTag = "Corpse";
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
		
		if (col.gameObject.tag == corTag){
			creObj.SendMessage("SeeFood", col.transform);
		}
		
		if (col.gameObject.tag == creTag){
			Creature cre = col.gameObject.GetComponent<Creature>();

			if (cre.TypeID != creObj.GetComponent<Creature>().TypeID){ //если не такой же как я
				if (cg.CTypes[cre.TypeID].eatMeat){ // и плотоядный значит враг
					creObj.SendMessage("SeeEnemy", col.transform);
				}else if (cg.CTypes[cre.TypeID].eatPlant) //травоядный знаяит еда !!переписать!!
					creObj.SendMessage("SeeLifeFood", col.transform);
			}
		}
	}
	
	void OnTriggerExit (Collider col){
		
		if (col.gameObject.tag == corTag)
			creObj.SendMessage("EscFood", col.transform);
		if (col.gameObject.tag == creTag)
			creObj.SendMessage("EscEnemy", col.transform);
	}
	
}
