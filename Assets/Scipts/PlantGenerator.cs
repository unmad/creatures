using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantGenerator : MonoBehaviour {

	public GameObject plantPrefab;
	public int count;
	int range = 100;
	List<GameObject> plants;
	GameObject logic;
	public bool gen;

	
	void Start (){
		logic = GameObject.FindWithTag ("Logic");
		gen = false;
	}
	void Update (){
		if (!gen & logic.GetComponent<UI> ().playing){
			Generate ();
		}
		if (gen & !logic.GetComponent<UI> ().playing) {
			Clear();
		}
	}

	public void GrowAt(float x, float y){
		plants.RemoveAll(delegate (GameObject o) { return o == null; });
		if (plants.Count < count * 5){
			GameObject p = (GameObject)Instantiate(plantPrefab,new Vector3(x, y, 0.1f),Quaternion.identity);
			p.transform.Rotate(new Vector3 (0f, 0f, Random.Range(0f,360f)));

			p.GetComponent<Plant>().Generate(Random.Range(30, 50));
			plants.Add(p);
			//Debug.Log ("Grow at " + x + " " + y + " " + plants.Count);
		} //else Debug.Log("too meny plants!!! " + plants.Count);
	}

	void Generate(){
		int width, height;
		width = logic.GetComponent<UI> ().width -2;
		height = logic.GetComponent<UI> ().height -2;
		plants = new List<GameObject>();
		count = logic.GetComponent<UI>().countOfPlant;
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
		Debug.Log ("Plants " + l);
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
