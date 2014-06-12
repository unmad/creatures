using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureGenerator : MonoBehaviour {
	public GameObject creaturePlantPrefab;
	public List<GameObject> creaturePlant;
	UI ui;
	public bool gen;

	//Magic
	float borderSize = 1f;

	string loTag = "Logic";
	string wayTag = "waypoint";

	//Magic End

	void Start () {
		gen = false;
		ui = GameObject.FindWithTag (loTag).GetComponent<UI>();
		creaturePlant = new List<GameObject>();
	}

	void Update () {
		if (!gen & ui.playing){
			Generate ();
		}
		if (gen & !ui.playing) {
			Clear();
		}
	}

	void Generate(){
		var width = (ui.width / 2) - borderSize;
		var height = (ui.height / 2) - borderSize;
		int l = ui.countOfTypes * ui.countOfCreature;
		int mins = ui.minCreatureSize;
		int maxs = ui.maxCreatureSize;

		for (int cot = 0; cot < ui.countOfTypes; cot++){
			int typeSize = Random.Range(mins, maxs);
			for (int coc = 0; coc < ui.countOfCreature; coc++){
				GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0),Quaternion.Euler(new Vector3(90f, 270f, 270f)));
				p.GetComponent<CreaturePlant>().Generate(cot,typeSize);
				creaturePlant.Add(p);
			}
		}
		gen = true;
	}
	void Clear (){
		var waypoints = GameObject.FindGameObjectsWithTag (wayTag);
		foreach (var w in waypoints) {
			Destroy(w);
		}
		creaturePlant.ForEach (Destroy);
		creaturePlant.Clear ();
	}
}
