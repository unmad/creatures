using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	public int size;
	UI ui;
	PlantGenerator pg;
	public bool grow;

	int maxSize = 5000;
	float timer;
	float lastTime;
	float scaleto;
	float nextTime;

	//Magic
	float borderSize = 1f;
	float minScale = 1f;
	float minNextTime = 0.5f;
	float maxNextTime = 2f;
	float scaleSpeed = 0.01f;
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
		transform.localScale = Vector3.zero;
		float sc = (minScale + ((float)size / maxSize) * 2);
		scaleto = sc;
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
		var scale = Vector3.one * scaleto;
		transform.localScale = Vector3.Slerp (transform.localScale, scale, scaleSpeed);
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
				int width = ui.width/2;
				int height = ui.height/2;
				var point = transform.position.ToVector2() + Random.insideUnitCircle * growRadius;
				point.x = Mathf.Clamp(point.x, -width + borderSize, width - borderSize);
				point.y = Mathf.Clamp(point.y, -height + borderSize, height - borderSize);
				pg.GrowAt(point.x, point.y);
				size -= Mathf.RoundToInt(maxSize * sizeToGrow);
			}

		} else {
			size = maxSize;
		}
		float sizeCoef = (float)size / (float)maxSize;
		float sc = minScale + (sizeCoef * 2);
		scaleto = sc;
	}

	public int GetSize (){
		return size;
	}

	public void SetSize(int i){
		size += i;
	}
}
