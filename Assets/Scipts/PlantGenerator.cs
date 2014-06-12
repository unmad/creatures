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

	float minPlantSize = 0.3f;
	float 

	//Magic End

	
	void Start (){
		ui = GameObject.FindWithTag ("Logic").GetComponent<UI>();
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
		if (plants.Count < count * 5){
			GameObject p = (GameObject)Instantiate(plantPrefab,new Vector3(x, y, 0.1f),Quaternion.identity);
			p.transform.Rotate(new Vector3 (0f, 0f, Random.Range(0f,360f)));

			p.GetComponent<Plant>().Generate(Random.Range(0.3f, 0.5f));
			plants.Add(p);
		}
	}

	void Generate(){
		int width, height;
		width = ui.width -2;
		height = ui.height -2;
		plants = new List<GameObject>();
		count = ui.countOfPlant;
		int x,y;
		int l = 0;
		for (x = -width/2; x < width/2; x++ ) {
			for (y = -height/2; y < height/2; y++ ) {
				if (Random.Range(0,range) < count/5){
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
