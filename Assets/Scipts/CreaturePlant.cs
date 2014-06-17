using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreaturePlant : MonoBehaviour {

	public GameObject corpsePrefab;
	UI ui;
	int typeID;
	int age;
	int maxAge;
	int size;
	int maxSize;

	bool isMale;
	bool alive;
	bool wantFuck;

	float timer;
	float lastTime;
	List<Transform> food;
	List<Transform> enemy;
	Transform target;
	CreatureGenerator cg;

	//Magic

	public float starving;
	public float hungry;
	
	string wayTag = "waypoint";

	//Magic End
	
	void Start () {
		ui = UI.GUI;
		cg = CreatureGenerator.CG;

		timer = Time.time;
		lastTime = timer;

		age = 0;

		size = Mathf.RoundToInt(maxSize * 0.1f);



		if (Random.value < 0.5)
			isMale = true;

		alive = true;
		wantFuck = false;

		food = new List<Transform>();
		enemy = new List<Transform>();
	}

	void Update () {
		timer = Time.time;
		if (ImAlive()) {

			if (timer - lastTime >= cg.birthdayTime) {
				if ((float)SendMessage("Energy") / (float)SendMessage("MaxEnergy") < starving )
					hp--;
				lastTime = timer;
				Grow();
			}
			if (target != null)
				SendMessage("MoveTo",target.position);
			else Think ();
		} else {
			Die();
		}
	}

	void Eat (){ //переписать
		if (target != null) {
			if (target.gameObject.tag == plTag) {
				var plant = target.gameObject.GetComponent<Plant>();
				if (plant.size < maxSize){
					SendMessage("Energy", plant.size);
					plant.SetSize(-plant.size);
				} else {
					SendMessage("Energy", maxSize);
					plant.SetSize(- maxSize);
				}
			} else DestroyWaypoint();
		}
	}

	bool ImAlive (){ //переписать
		if (age > maxAge) {
			Debug.Log("Die Age");
			return false;
		}
		if (SendMessage("Energy") <= 0){
			Debug.Log("Die Energy");
			return false;
		}
		if (SendMessage ("Hp") <= 0) {
			Debug.Log("Die HP");
			return false;
		}
		return true;
	}

	void Die(){
		Vector3 pos = transform.position;
		GameObject p = (GameObject)Instantiate(corpsePrefab, pos, transform.rotation);
		var vis = p.GetComponent<CreatureCorpse> ();
		vis.visual.transform.localScale = SendMessage("Scale");

		int pSize;

		if (SendMessage("Energy") < 1) 
			pSize = size;
		else
			pSize = size * SendMessage("Energy");

		p.GetComponent<CreatureCorpse> ().SetSize (pSize);
		DestroyWaypoint();
		Destroy (this.gameObject);

	}

	void DestroyWaypoint(){
		if (target != null && target.tag == wayTag)
		Destroy (target.gameObject);
	}
	
	void Run(string s){
		var width = (ui.width / 2) - cg.borderSize;
		var height = (ui.height / 2) - cg.borderSize;
		Vector2 point;

		if (s == "walk") {
			point = target.position.ToVector2 () - transform.position.ToVector2 ();
			point = transform.position.ToVector2 () - point;
		}

		if (s == "run")
			point = transform.position.ToVector2() + Random.insideUnitCircle * cg.wayPointRange;

		point.x = Mathf.Clamp(point.x, -width, width );
		point.y = Mathf.Clamp(point.y, -height, height);
		NewWayPoint (point);
	}

	void NewWayPoint(Vector2 point){

		DestroyWaypoint();

		var wayPoint = new GameObject();
		target = wayPoint.transform;
		target.gameObject.name = wayTag;
		target.gameObject.tag = wayTag;
		target.position = new Vector3(point.x, point.y, 0f);
	}

	bool SearchFood (){
		Transform t;
		t = FindNearest(food);

		if (t != null){
			DestroyWaypoint();
			target = t;
			return true;
		} else return false;
	}

	bool SearchEnemy (){

		Transform t;
		t = FindNearest(enemy);
		
		if (t != null){
			DestroyWaypoint();
			target = t;
			NewWayPoint(target.position.ToVector2());
			return true;
		} else return false;
	}

	void Think(){

		if ((float)SendMessage("Energy") / (float)SendMessage("maxEnergy") > starving) {
			if (SearchEnemy())
				Run("run");
			else if (!SearchFood())
				Run("walk");
		} else if (!SearchFood())
			Run("walk");
	}

	void Grow(){//переписать
		age++;
		if (age < maxAge * 0.15f){
			float ageCoef = ((float)age / maxAge) * 6.66f;
			size = 1 + Mathf.RoundToInt(Mathf.Lerp(0.5f, (float)maxSize, ageCoef));
			visual.localScale = Vector3.one * (Mathf.Lerp(minScale, maxScale, (float)size / maxSize));
			
		} else if (size != maxSize) {
			size = maxSize;
			visual.localScale = Vector3.one * maxScale;
		}
	}

	Transform FindNearest(List<Transform> tars){
		Transform t = null;
		tars.RemoveAll(o => o == null);
		
		if (tars.Count > 0){
			float dis = float.MaxValue;
			float minDis = float.MaxValue;
			
			foreach (var tar in tars) {
				dis = (transform.position - tar.position).sqrMagnitude;
				if (dis < minDis) {
					t = tar;
					minDis = dis;
				}
			}
			return t;
		} else 
			return null;
	}

	void SeeFood (Transform t){
		if (!food.Contains(t.transform)) 
			food.Add(t.transform);
	}

	void SeeEnemy (Transform t){
		if (!enemy.Contains(t.transform)) 
			enemy.Add(t.transform);
	}

	void EscFood (Transform t){
		if (food.Contains(t.transform)) 
			food.Remove(t.transform);
	}

	void EscEnemy (Transform t){
		if (!enemy.Contains(t.transform)) 
			enemy.Remove(t.transform);
	}

	public bool IsMale { get { return isMale; } }

	public int TypeID { get { return typeID; } }

	public bool Life { get { return alive; } }

	public bool WantFuck { get { return wantFuck; } }

	public int Size { get { return size; } }

	public void SetMaxAge (int i) {maxAge = i; }

	public void SetMaxSize (int i){maxSize = i; }

	public void SetTypeID (int i){typeID = i; }



}
