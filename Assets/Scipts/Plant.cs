using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	public int size;
	public bool grow;
	public Material[] mats;

	UI ui;
	PlantGenerator pg;

	int maxSize = 5000;
	int material = 0;
	float timer;
	float lastTime;
	float nextTime;

	//Magic
	float borderSize = 1f;
	float minNextTime = 0.5f;
	float maxNextTime = 2f;
	float chanceToGrowOther = 0.2f;
	float sizeToGrow = 0.1f;
	float growRadius = 3f;

	string loTag = "Logic";

	//Magic End

	public void Generate(float i){
		int s = Mathf.RoundToInt(maxSize * i); 
		if (s < maxSize) {
			size = s;
		} else {
			size = maxSize;
		}
	}

	void Start () {
		pg = GameObject.FindGameObjectWithTag (loTag).GetComponent<PlantGenerator>();
		ui = GameObject.FindWithTag (loTag).GetComponent<UI>();
		timer = Time.time;
		lastTime = timer;
		nextTime = Random.Range (minNextTime, maxNextTime);
	}

	void Update () {
		timer = Time.time;
		if (size < 1) Destroy(this.gameObject);
		if (timer - lastTime >= nextTime){
			nextTime = Random.Range (minNextTime, maxNextTime);
			lastTime = timer;
			if (Random.value < chanceToGrowOther){
				int g = Mathf.RoundToInt(Mathf.Clamp((float)size * sizeToGrow, 1, float.MaxValue));
				Grow (g);
			}
		}
	}

	void Grow (int g){
		if (size + g <= maxSize) {
			size += g;
			if (size > maxSize/8){
				int width = Mathf.RoundToInt(((float)ui.width / 2) - borderSize);
				int height = Mathf.RoundToInt(((float)ui.height / 2) - borderSize);
				var point = transform.position.ToVector2() + Random.insideUnitCircle * growRadius;
				point.x = Mathf.Clamp(point.x, -width, width);
				point.y = Mathf.Clamp(point.y, -height, height);
				pg.GrowAt(point.x, point.y);
			}

		} else {
			size = maxSize;
		}
		float sizeCoef = (float)size / (float)maxSize;
		var mat = Mathf.RoundToInt(Mathf.Clamp((sizeCoef * mats.Length) - 1, 0, mats.Length));
		if (material != mat){
			material = mat;
			renderer.material = mats[mat];
		}

	}

	public int GetSize (){
		return size;
	}

	public void SetSize(int i){
		size += i;
	}
}
