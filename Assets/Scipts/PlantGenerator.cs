using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Islands;

public class PlantGenerator : Singleton<PlantGenerator> {

	public GameObject plantPrefab;
	public int soilRichness; // плодородность почвы в процентах 0-99
	public int countOfPlants;
	List<GameObject> plants;
	myUI ui;
	public bool gen;

	//Magic

	public float borderSize = 1.5f;
	float minPlantSize = 0.3f;
	float maxPlantSize = 0.5f;
	float zPos = 0.1f;
	float plantGrowRadius = 5f;

	//For Plant
	public string s = "For Plant";
	public int maxSize = 10000;
	public float minNextTime = 0.1f;
	public float maxNextTime = 0.5f;
	public float chanceToGrowOther = 0.2f;
	public float growRadius = 5f;
	public int initSize;

	//Magic End

	void Start (){
		ui = myUI.Instance;
		gen = false;
	}
	void Update (){
		if (!gen & ui.playing){
			Generate ();
			//countOfPlants = plants.Count;
		}
		if (gen & !ui.playing) {
			Clear();
			//countOfPlants = plants.Count;
		}
	}

	public void GrowAt(float x, float y){
		plants.RemoveAll(o => o == null);
		Vector2 point = new Vector2(x, y);
		int mask = LayerMask.NameToLayer("food");
		var cols = Physics.OverlapSphere (point, plantGrowRadius, mask);

		if (cols.Length < soilRichness * 0.1f){
			initSize = Mathf.RoundToInt((float)maxSize * Random.Range(minPlantSize, maxPlantSize));
			GameObject p = (GameObject)Instantiate(plantPrefab,new Vector3(x, y, zPos),Quaternion.identity);
			p.transform.Rotate(new Vector3 (0f, 0f, Random.Range(0f,360f)));
			plants.Add(p);
		}
	}

	void Generate(){
		int width = Mathf.RoundToInt((ui.width / 2) - borderSize);
		int height = Mathf.RoundToInt((ui.height / 2)  - borderSize);
		plants = new List<GameObject>();
		soilRichness = ui.soilRichness;
		int x,y;
		int l = 0;
		for (x = -width; x < width; x++ ) {
			for (y = -height; y < height; y++ ) {
				if (Random.value < 0.5)
					GrowAt(x+0.1f, y-0.1f);
				if (Random.value < 0.5)
					GrowAt(x-0.1f, y-0.1f);
				if (Random.value < 0.5)
					GrowAt(x+0.1f, y+0.1f);
				if (Random.value < 0.5)
					GrowAt(x-0.1f, y+0.1f);
				l++;
			}
		}
		gen = true;
	}

	void Clear (){
		foreach (var p in plants) {
			if (p != null)
				DestroyObject(p);
		}
		plants.Clear ();
		plants = null;
		gen = false;
	}
}
