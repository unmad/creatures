using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureGenerator : MonoBehaviour {
	public GameObject creaturePlantPrefab;
	public List<GameObject> creaturePlant;
	UI ui;
	public bool gen;
	
	void Start () {
		gen = false;
		ui = GameObject.FindWithTag ("Logic").GetComponent<UI>();
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
		int width, height;
		width = ui.width;
		height = ui.height;
		int l = ui.countOfTypes * ui.countOfCreature;
		int mins = ui.minCreatureSize;
		int maxs = ui.maxCreatureSize;

		for (int cot = 0; cot < ui.countOfTypes; cot++){
			int typeSize = Random.Range(mins, maxs);
			for (int coc = 0; coc < ui.countOfCreature; coc++){
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
