using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	public int size;
	public GameObject logic;
	public bool grow;

	int maxSize = 5000;
	float timer;
	float lastTime;
	float scaleto;
	float nextTime;

	public void Generate(int i){
		int s = (maxSize/100) * i; 
		if (s < maxSize) {
			size = s;
		} else {
			//Debug.Log ("Size > MaxSize in generator");
			size = maxSize;
		}
		transform.localScale = new Vector3 (0f, 0f, 0f);
		float sc = (1 + ((float)size / maxSize) * 2);
		scaleto = sc;
	}

	void Start () {
		logic = GameObject.FindWithTag ("Logic");
		timer = Time.time;
		lastTime = timer;
		nextTime = Random.Range (0.5f, 2f);
	}


	void Update () {
		timer = Time.time;
		var scale = Vector3.one * scaleto;
		transform.localScale = Vector3.Slerp (transform.localScale, scale, 0.01f);
		if (size < 1) Destroy(this.gameObject);
		if (timer - lastTime >= nextTime){
			nextTime = Random.Range (0.5f, 2f);
			lastTime = timer;
			if (Random.value < 0.3){
				int g = Mathf.RoundToInt(Mathf.Clamp((float)size / 10, 1, float.MaxValue));
				Grow (g);
			}
		}
	}

	void Grow (int i){
		if (size + i <= maxSize) {
			size += i;
			if (size > maxSize/8){
				int width, height;
				width = logic.GetComponent<UI> ().width;
				height = logic.GetComponent<UI> ().height;
				var point = transform.position.ToVector2() + Random.insideUnitCircle * 3;
				point.x = Mathf.Clamp(point.x, (-width/2)+1, (width/2)-1);
				point.y = Mathf.Clamp(point.y, (-height/2)+1, (height/2)-1);
				logic.GetComponent<PlantGenerator>().GrowAt(point.x, point.y);
				size -= maxSize/100;
			}

		} else {
			size = maxSize;
		}
		float sc = (1 + ((float)size / (float)maxSize) * 2);
		scaleto = sc;
	}

	public int GetSize (){
		return size;
	}

	public void SetSize(int i){
		size += i;
	}
}
