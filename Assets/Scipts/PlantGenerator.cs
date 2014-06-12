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

	float borderSize = 1.5f;
	float minPlantSize = 0.3f;
	float maxPlantSize = 0.5f;
	float zPos = 0.1f;
	float plantGrowRadius = 5f;

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
		plants.RemoveAll(o => o == null);
		Vector2 point = new Vector2(x, y);
		var cols = Physics.OverlapSphere (point, plantGrowRadius);
		if (cols.Length < count * 0.05){
			GameObject p = (GameObject)Instantiate(plantPrefab,new Vector3(x, y, zPos),Quaternion.identity);
			p.transform.Rotate(new Vector3 (0f, 0f, Random.Range(0f,360f)));
			p.GetComponent<Plant>().Generate(Random.Range(minPlantSize, maxPlantSize));
			plants.Add(p);
		}
	}

	void Generate(){
		int width = Mathf.RoundToInt((ui.width / 2) - borderSize);
		int height = Mathf.RoundToInt((ui.height / 2)  - borderSize);
		plants = new List<GameObject>();
		count = ui.countOfPlant;
		int x,y;
		int l = 0;
		for (x = -width; x < width; x++ ) {
			for (y = -height; y < height; y++ ) {
				if (Random.Range(0,range) < count){
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
