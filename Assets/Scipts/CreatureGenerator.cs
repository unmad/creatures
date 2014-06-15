using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureGenerator : MonoBehaviour {
	public GameObject creaturePlantPrefab;
	public List<GameObject> creaturePlant;
	UI ui;
	public bool gen;

	//Magic
	public float borderSize = 1f;
	public float minTypesScale = 0.5f;
	public float maxTypesScale = 1.5f;

	string loTag = "Logic";
	string wayTag = "waypoint";


	//For Creatures
	public string s = "For Creatures";

	public int maxAge = 500;
	public int maxTypeSize = 500;
	public int minTypeSize = 100;
	public int sizeToEnergy = 100;

	public float birthdayTime = 0.5f;
	public float wayPointRange = 5f;
	public float rangeToStop = 0.3f;
	public float speedToRotate = 2f;
	public float sizeRangeCoef = 0.2f;
	public float minSpeedRange = 0.02f;
	public float maxSpeedRange = 0.1f;

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

		for (int cot = 0; cot < ui.countOfTypes; cot++){
			float typeScale = Random.Range(minTypesScale, maxTypesScale);
			int typeSize = Random.Range(minTypeSize, maxTypeSize);
			float minscale = typeScale * 0.3f;
			float maxscale = typeScale;
			float typeSizeCoef = 1f - ((float)typeSize / (float)maxTypeSize);
			float typeSpeed = Mathf.Lerp(minSpeedRange, maxSpeedRange, typeSizeCoef);
			//Debug.Log("typeSpeed " + typeSpeed);

			for (int coc = 0; coc < ui.countOfCreature; coc++){
				GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0),Quaternion.Euler(new Vector3(90f, 270f, 270f)));
				var cre = p.GetComponent<CreaturePlant>();
				cre.SetMaxAge(maxAge);
				cre.SetMaxSize(typeSize);
				cre.SetMinScale(minscale);
				cre.SetMaxScale(maxscale);
				cre.SetTypeID(cot);
				cre.SetMaxSpeed(typeSpeed);
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
		var corpses = GameObject.FindGameObjectsWithTag("Corpse");
		foreach (var c in corpses) {
			Destroy(c);
		}
	}
}
