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

	public void Generate(int i){
		int s = (maxSize/100) * i; 
		if (s < maxSize) {
			size = s;
		} else {
			//Debug.Log ("Size > MaxSize in generator");
			size = maxSize;
		}
		transform.localScale = new Vector3 (0f, 0f, 0f);
		float sc = (1 + ((float)size / (float)maxSize) * 2);
		scaleto = sc;
	}

	void Start () {
		logic = GameObject.FindWithTag ("Logic");
		timer = Time.time;
		lastTime = timer;
	}


	void Update () {
		timer = Time.time;
		transform.localScale = Vector3.Slerp (transform.localScale, new Vector3 (scaleto, scaleto, scaleto), 0.01f );
		if (size < 1) Destroy (this.gameObject);
		if (timer - lastTime >= Random.Range (0.5f, 2f)){
			lastTime = timer;
			if (Random.value < 0.3){
				int g = Mathf.RoundToInt (size / 10);
				if (g < 1) g = 1;
				Grow (g);
			}
		}
	}

	void Grow (int i){
		if (size + i <= maxSize) {
			size += i;
			if (size > maxSize/8){
				float x, y;
				int width, height;
				width = logic.GetComponent<UI> ().width;
				height = logic.GetComponent<UI> ().height;
				x = transform.position.x + Random.Range(-3f,3f);
				y = transform.position.y + Random.Range(-3f,3f);
				if (x < -width/2+1){
					x = -width/2 + 2f;
					//Debug.Log ("x < width " + x);
				}
				if (x > width/2-1){
					x = width/2 - 2f;
					//Debug.Log ("x > width " + x);
				}
				if (y < -height/2+1){
					y = -height/2 + 2f;
					//Debug.Log ("y < height " + y);
				}
				if (y > height/2-1){
					y = height/2 - 2f;
					//Debug.Log ("y > height " + y);
				}
				logic.GetComponent<PlantGenerator>().GrowAt(x, y);
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
