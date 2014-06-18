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
	bool starving;

	float timer;
	float lastTime;

	List<Transform> food;
	List<Transform> enemy;

	Transform target;

	CreatureGenerator cg;

	//Magic

	string wayTag = "waypoint";

	//Magic End
	
	void Start () {
		ui = UI.Instance;
		cg = CreatureGenerator.Instance;

		timer = Time.time;
		lastTime = timer;

		age = 0;

		size = Mathf.RoundToInt(maxSize * 0.1f);



		if (Random.value < 0.5)
			isMale = true;

		alive = true;
		wantFuck = false;
		starving = false;

		SendMessage("SetMaxEnergy", maxSize * cg.sizeToEnergy);
		SendMessage("SetMaxHp", maxSize);

		food = new List<Transform>();
		enemy = new List<Transform>();
	}

	void Update () {
		timer = Time.time;
		if (age < maxAge) {
			if (timer - lastTime >= cg.birthdayTime) {
				lastTime = timer;
				Grow();
				SendMessage("IStarving");
			}
	
			if (target == null)
				Think ();
			else SendMessage("MoveTo", target.position);

		} else Die();
	}

	void Think(){
		DestroyWaypoint();
		if (starving){

			if (SearchFood()){ //если голодаем ищем еду
				SendMessage("MoveTo", target.position); //идем к еде
			} else 
				Run("walk"); //если еды нету, гуляем, упорно пытаемся наткнутся на еду

		} else if (!SearchEnemy()){

			if (wantFuck){
//				if (SearchFuck) //нувыпонели
//					MoveTo();
//				else 
//					Run ("walk"); //ищем приключений
			} else 
				Run("walk");

		} else 
			Run("run"); //убегаем от вражин
	}

	public void InPosition(){
		if (target.tag == wayTag){
			DestroyWaypoint();
			Think();
		}
		else if (food.Contains(target))
			SendMessage("Eat", target.gameObject);
//		else if (fuckers.Contains(target))
//			SendMessage("Fuck");
	}

	void Die(){
		Vector3 pos = transform.position;
		GameObject p = (GameObject)Instantiate(corpsePrefab, pos, transform.rotation);
		//var vis = p.GetComponent<CreatureCorpse> ();
		//vis.visual.transform.localScale = size / maxSize;

		int pSize;

		//if (SendMessage("Energy") < 1) 
			pSize = size;
		//else
		//	pSize = size * SendMessage("Energy");

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
		Vector2 point = new Vector2(0f, 0f);

		if (s == "run") {
			point = target.position.ToVector2 () - transform.position.ToVector2 ();
			point = transform.position.ToVector2 () - point;
		}

		if (s == "walk")
			point = transform.position.ToVector2() + Random.insideUnitCircle * cg.wayPointRange;

		point.x = Mathf.Clamp(point.x, -width, width );
		point.y = Mathf.Clamp(point.y, -height, height);
		NewWayPoint (point);
		SendMessage("MoveTo", target.position);
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
		t = Utils.FindNearest(transform, food);

		if (t != null){
			DestroyWaypoint();
			target = t;
			return true;
		} else return false;
	}

	bool SearchEnemy (){

		Transform t;
		t = Utils.FindNearest(transform, enemy);
		
		if (t != null){
			DestroyWaypoint();
			target = t;
			NewWayPoint(target.position.ToVector2());
			return true;
		} else return false;
	}



	void Grow(){
		age++;
		if (age < maxAge * 0.15f){
			float ageCoef = ((float)age / maxAge) * 6.66f;
			size = Mathf.RoundToInt(Mathf.Lerp((float)maxSize * 0.5f, (float)maxSize, ageCoef)); //какаята страшная магия О_о
			SendMessage("SetScale", (float)size / maxSize);
			
		} else if (size != maxSize) {
			size = maxSize;
			SendMessage("SetScale",1);
		}
	}

	void SeeFood (Transform t){
		if (!food.Contains(t.transform)) 
			food.Add(t.transform);
		if (food.Count < 1)
			SendMessage("Think");
	}

	void SeeEnemy (Transform t){
		if (!enemy.Contains(t.transform)) 
			enemy.Add(t.transform);
		Think();
	}

	void EscFood (Transform t){
		if (food.Contains(t.transform)) 
			food.Remove(t.transform);
		food.RemoveAll(o => o == null);
	}

	void EscEnemy (Transform t){
		if (!enemy.Contains(t.transform)) 
			enemy.Remove(t.transform);
		enemy.RemoveAll(o => o == null);
	}

	public bool IsMale { get { return isMale; } }

	public int TypeID { get { return typeID; } }

	public bool Life { get { return alive; } }

	public bool WantFuck { get { return wantFuck; } }

	public int Size { get { return size; } }

	public void Starving (bool i) {starving = i; }

	public void SetAge (int i) {age = i; }

	public void SetMaxAge (int i) {maxAge = i; }

	public void SetSize (int i){size = i; }

	public void SetMaxSize (int i){maxSize = i; }

	public void SetTypeID (int i){typeID = i; }
	
}
