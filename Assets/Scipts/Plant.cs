using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	public int size;
	public Material[] mats;

	myUI ui;
	PlantGenerator pg;

	int material = 0;
	float timer;
	float lastTime;
	float nextTime;

	void Start () {
		pg = GameObject.FindWithTag ("Logic").GetComponent<PlantGenerator>();
		ui = GameObject.FindWithTag ("Logic").GetComponent<myUI>();
		timer = Time.time;
		lastTime = timer;
		nextTime = Random.Range (pg.minNextTime, pg.maxNextTime);

		int s = Mathf.RoundToInt(pg.initSize); 
		if (s < pg.maxSize) {
			size = s;
		} else {
			size = pg.maxSize;
		}
	}

	void Update () {
		timer = Time.time;
		if (size < 1) Destroy(this.gameObject);
		if (timer - lastTime >= nextTime){
			nextTime = Random.Range (pg.minNextTime, pg.maxNextTime);
			lastTime = timer;
			Grow (pg.soilRichness);
		}
	}

	void Grow (int g){
		if (size + g <= pg.maxSize) 
			size += g;
		 else 
			size = pg.maxSize;

		if (Random.value < pg.chanceToGrowOther){
			float width = ((float)ui.width / 2) - pg.borderSize;
			float height = ((float)ui.height / 2) - pg.borderSize;
			var point = transform.position.ToVector2() + Random.insideUnitCircle * pg.growRadius;
			point.x = Mathf.Clamp(point.x, -width, width);
			point.y = Mathf.Clamp(point.y, -height, height);
			pg.GrowAt(point.x, point.y);
		}

		float sizeCoef = (float)size / (float)pg.maxSize;
		var mat = Mathf.RoundToInt(Mathf.Clamp((sizeCoef * mats.Length) - 1, 0, mats.Length));
		if (material != mat){
			material = mat;
			renderer.material = mats[mat];
		}

	}

	public void EatMe(Eating vEat){

		if (vEat.eat > size){
			SetSize(-1);
			vEat.eat = size;
		} else
			SetSize(-vEat.eat);
		
		vEat.i.SendMessage("SetEnergy", vEat.eat);
	}

	public int GetSize { get { return size; } }

	public void SetSize(int i){
		size += i;
	}
}
