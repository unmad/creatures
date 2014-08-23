using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Islands;

public class CreatureType {
	public int id;
	public bool eatMeat;
	public bool eatPlant;
	public int typeSize;
}

public sealed class CreatureGenerator : Singleton<CreatureGenerator> {
	public GameObject creaturePlantPrefab;
	public GameObject creatureMeatPrefab;
	public List<GameObject> creatures;
	myUI ui;
	public bool gen;
	public bool genmeateater;
	public List<CreatureType> CTypes;

	//Magic
	public float borderSize = 1f;
	public float minTypesScale = 0.5f;
	public float maxTypesScale = 1.5f;

	string wayTag = "waypoint";


	//For Creatures
	public string s = "For Creatures";

	public int maxAge = 500;
	public int maxTypesSize = 500;
	public int minTypesSize = 100;
	public int sizeToEnergy = 100;

	public float birthdayTime = 0.5f;
	public float wayPointRange = 5f;
	public float rangeToStop = 0.3f;
	public float speedToRotate = 2f;
	public float sizeRangeCoef = 0.2f;
	public float minSpeedRange = 0.02f;
	public float maxSpeedRange = 0.1f;
	public float sizeToMove = 0.2f;
	//Magic End

	void Start () {
		gen = false;
		ui = myUI.Instance;
		creatures = new List<GameObject>();
	}

	void Update () {
		if (!gen & ui.playing){
			Generate ();
		}
		if (gen & !ui.playing) {
			Clear();
		}
	}

	public void GrowAt(int type, float x, float y){

		if (CTypes[type].eatPlant && !CTypes[type].eatMeat){ //травоядное

			GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(x, y, 0),Quaternion.identity);
			SetInitVar(p, type);
			creatures.Add(p);
		}

		if (!CTypes[type].eatPlant && CTypes[type].eatMeat){ //травоядное
			
			GameObject p = (GameObject)Instantiate(creatureMeatPrefab,new Vector3(x, y, 0),Quaternion.identity);
			SetInitVar(p, type);
			creatures.Add(p);
		}

	}

	void SetInitVar(GameObject cre, int type){
		// забить все начальные переменные через сендмесседж
		// creature
		int typeID = type;
		int age = 0;
		int maxage = maxAge;
		int maxSize = CTypes[type].typeSize;
		int size = Mathf.RoundToInt(maxSize * 0.1f);
		// energy
		int maxEnergy = maxSize * sizeToEnergy;
		int energy = Mathf.RoundToInt(maxEnergy / 2);
		int eat = size;
		int maxHp = size;
		int hp = maxHp;
		//mover
		float maxSpeed = Mathf.Lerp(minSpeedRange, maxSpeedRange, 1f - maxSize / maxTypesSize);
		float speed = maxSpeed;
		float speedtoRotate = speedToRotate;
		float rangetoStop = rangeToStop;
		float speedCoef = 1;
		int energytoMove = Mathf.RoundToInt(sizeToMove * size);
		//visual
		float minScale = Mathf.Lerp(minTypesScale, maxTypesScale, maxSize / maxTypesSize) * 0.3f;
		float maxScale = Mathf.Lerp(minTypesScale, maxTypesScale, maxSize / maxTypesSize);
		float scale = minScale;

		// creature
		cre.SendMessage("SetTypeID", typeID);
		cre.SendMessage("SetAge", age);
		cre.SendMessage("SetMaxAge", maxage);
		cre.SendMessage("SetSize", size);
		cre.SendMessage("SetMaxSize", maxSize);
		// energy
		cre.SendMessage("SetEnergy", energy);
		cre.SendMessage("SetMaxEnergy", maxEnergy);
		cre.SendMessage("SetEat", eat);
		cre.SendMessage("SetHp", hp);
		cre.SendMessage("SetMaxHp", maxHp);
		//mover
		cre.SendMessage("SetSpeed", speed);
		cre.SendMessage("SetMaxSpeed", maxSpeed);
		cre.SendMessage("SetSpeedToRotate", speedtoRotate);
		cre.SendMessage("SetRangeToStop", rangetoStop);
		cre.SendMessage("SetSpeedCoef", speedCoef);
		cre.SendMessage("SetEnergyToMove", energytoMove);
		//visual
		cre.SendMessage("SetScale", scale);
		cre.SendMessage("SetMinScale", minScale);
		cre.SendMessage("SetMaxScale", maxScale);
	}

	void GenerateTypes(){
		CTypes = new List<CreatureType>();

		for (int cot = 0; cot < ui.countOfTypes; cot++){
			CreatureType ct = new CreatureType();
			ct.id = cot;

			if (genmeateater){
				ct.eatMeat = Random.value >= 0.5f;
				ct.eatPlant = Random.value >= 0.5f;
			}else {
				ct.eatMeat = false;
				ct.eatPlant = true;
			}

			ct.typeSize  = Random.Range(minTypesSize, maxTypesSize);

			CTypes.Add(ct);
		}
	}

	void Generate(){
		var width = (ui.width / 2) - borderSize;
		var height = (ui.height / 2) - borderSize;

		GenerateTypes();

		for (var cot = 0; cot < CTypes.Count; cot++){
			for (int coc = 0; coc < ui.countOfCreature; coc++){

				var x = Random.Range(-width, width);
				var y = Random.Range(-height, height);

				GrowAt(cot, x, y);
			}
		}
		gen = true;
	}

	void Clear (){

		var waypoints = GameObject.FindGameObjectsWithTag (wayTag);

		foreach (var w in waypoints) {
			Destroy(w);
		}

		var corpses = GameObject.FindGameObjectsWithTag("Corpse");
		
		foreach (var c in corpses) {
			Destroy(c);
		}

//		foreach (var ct in CTypes){
//			Destroy(ct);
//		}

		creatures.ForEach (Destroy);
		creatures.Clear ();
//		CTypes.ForEach (Destroy);
		CTypes.Clear ();
	}
}
