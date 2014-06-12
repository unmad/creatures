using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantGenerator : MonoBehaviour {

	public GameObject plantPrefab;
	public int count;
	int range = 100;
	List<GameObject> plants;
	UI ui;
	public bool gen;

	//Magic

	float borderSize = 1f;
	float minPlantSize = 0.3f;
	float maxPlantSize = 0.5f;
	float zPos = 0.1f;
	
	int plantCountCoef = 5;

	string loTag = "Logic";

	//Magic End

	
	void Start (){
		ui = GameObject.FindWithTag (loTag).GetComponent<UI>();
		gen = false;
	}
	void Update (){
		if (!gen & ui.playing){
			Generate ();
		}
		if (gen & !ui.playing) {
			Clear();
		}
	}

	public void GrowAt(float x, float y){
		plants.RemoveAll(delegate (GameObject o) { return o == null; });
		if (plants.Count < count * ui.height){
			GameObject p = (GameObject)Instantiate(plantPrefab,new Vector3(x, y, zPos),Quaternion.identity);
			p.transform.Rotate(new Vector3 (0f, 0f, Random.Range(0f,360f)));

			p.GetComponent<Plant>().Generate(Random.Range(minPlantSize, maxPlantSize));
			plants.Add(p);
		}
	}

	void Generate(){
		int width = Mathf.RoundToInt((ui.width - borderSize) / 2);
		int height = Mathf.RoundToInt((ui.height - borderSize) / 2);
		plants = new List<GameObject>();
		count = ui.countOfPlant;
		int x,y;
		int l = 0;
		for (x = -width; x < width; x++ ) {
			for (y = -height; y < height; y++ ) {
				if (Random.Range(0,range) < count / plantCountCoef){
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
