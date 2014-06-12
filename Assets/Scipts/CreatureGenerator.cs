using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureGenerator : MonoBehaviour {
	public GameObject creaturePlantPrefab;
	public List<GameObject> creaturePlant;
	GameObject logic;
	public bool gen;
	
	void Start () {
		gen = false;
		logic = GameObject.FindWithTag ("Logic");
		creaturePlant = new List<GameObject>();
	}

	void Update () {
		if (!gen & logic.GetComponent<UI> ().playing){
			Generate ();
		}
		if (gen & !logic.GetComponent<UI> ().playing) {
			Clear();
		}
	}

	void Generate(){
		int width, height;
		width = logic.GetComponent<UI> ().width;
		height = logic.GetComponent<UI> ().height;
		int l = logic.GetComponent<UI> ().countOfTypes * logic.GetComponent<UI> ().countOfCreature;
		int mins = logic.GetComponent<UI> ().minCreatureSize;
		int maxs = logic.GetComponent<UI> ().maxCreatureSize;

		for (int cot = 0; cot < logic.GetComponent<UI>().countOfTypes; cot++){
			int typeSize = Random.Range(mins, maxs);
			for (int coc = 0; coc < logic.GetComponent<UI>().countOfCreature; coc++){
				GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(Random.Range(-width/2, width/2), Random.Range(-height/2, height/2), 0),Quaternion.Euler(new Vector3(90f, 270f, 270f)));
				p.GetComponent<CreaturePlant>().Generate(cot,typeSize);
				creaturePlant.Add(p);
			}
		}
		gen = true;
	}
	void Clear (){
		var waypoints = GameObject.FindGameObjectsWithTag ("waypoint");
		foreach (var w in waypoints) {
			Destroy(w);
		}
		creaturePlant.ForEach (Destroy);
		creaturePlant.Clear ();
	}
}
