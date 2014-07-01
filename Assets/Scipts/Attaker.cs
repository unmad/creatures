using UnityEngine;
using System.Collections;

public class Attaker : MonoBehaviour {

	void Attack(GameObject cre){
		cre.SendMessage("SetHp", -50);
	}
}
