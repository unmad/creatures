using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureType : MonoBehaviour {
	public int id;
	public bool eatMeat;
	public bool eatPlant;
	public float typeScale;
	public int typeSize;
	public float minscale;
	public float maxscale;
	public float typeSpeed;
}

public sealed class CreatureGenerator : MonoBehaviour {
	public GameObject creaturePlantPrefab;
	public List<GameObject> creaturePlant;
	UI ui;
	public bool gen;
	List<CreatureType> CTypes;

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

	static readonly CreatureGenerator cg = new CreatureGenerator();
	
	static CreatureGenerator() { }  
	
	CreatureGenerator() { }  
	
	public static CreatureGenerator CG  
	{  
		get  
		{  
			return cg;  
		}  
	}  

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

	void GrowAt(int type, float x, float y){

		if (CTypes[type].eatPlant && !CTypes[type].eatMeat){ //травоядное

			GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(x, y, 0),Quaternion.identity);
			var cre = p.GetComponent<CreaturePlant>();
			SetInitVar(cre, maxAge, CTypes[type].typeSize, CTypes[type].minscale, CTypes[type].maxscale, type, CTypes[type].typeSpeed);
			creaturePlant.Add(p);
		}

		if (CTypes[type].eatPlant && CTypes[type].eatMeat){ //всеядное
			
			GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(x, y, 0),Quaternion.identity);
			var cre = p.GetComponent<CreaturePlant>();
			SetInitVar(cre, maxAge, CTypes[type].typeSize, CTypes[type].minscale, CTypes[type].maxscale, type, CTypes[type].typeSpeed);
			creaturePlant.Add(p);
		}

		if (!CTypes[type].eatPlant && CTypes[type].eatMeat){ //плотоядное
			
			GameObject p = (GameObject)Instantiate(creaturePlantPrefab,new Vector3(x, y, 0),Quaternion.identity);
			var cre = p.GetComponent<CreaturePlant>();
			SetInitVar(cre, maxAge, CTypes[type].typeSize, CTypes[type].minscale, CTypes[type].maxscale, type, CTypes[type].typeSpeed);
			creaturePlant.Add(p);
		}

	}

	void SetInitVar(CreaturePlant cre, int maxAge, int maxSize, float minScale, float maxScale, int typeID, float maxSpeed){	//заменить на общий класс кричера
		cre.SetMaxAge(maxAge);
		cre.SetMaxSize(maxSize);
		cre.SetMinScale(minScale);
		cre.SetMaxScale(maxScale);
		cre.SetTypeID(typeID);
		cre.SetMaxSpeed(maxSpeed);
	}

	void GenerateTypes(){
		CTypes = new List<CreatureType>();

		for (int cot = 0; cot < ui.countOfTypes; cot++){
			CreatureType ct = new CreatureType();
			ct.id = cot;
			ct.eatMeat = false;
			ct.eatPlant = true;
			ct.typeScale = Random.Range(minTypesScale, maxTypesScale);
			ct.typeSize  = Random.Range(minTypeSize, maxTypeSize);
			ct.minscale = ct.typeScale * 0.3f;
			ct.maxscale = ct.typeScale;
			float typeSizeCoef = 1f - ((float)ct.typeSize / (float)maxTypeSize);
			ct.typeSpeed = Mathf.Lerp(minSpeedRange, maxSpeedRange, typeSizeCoef);

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

		creaturePlant.ForEach (Destroy);
		creaturePlant.Clear ();
		CTypes.ForEach (Destroy);
		CTypes.Clear ();
	}
}
