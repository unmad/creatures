using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThinkerMeatEater : MonoBehaviour {
	
	myUI ui;
	
	float timer;
	float lastTime;
	
	List<Transform> food;
	List<Transform> lifeFood;
	List<Transform> enemy;
	
	Transform target;
	
	Creature im;
	
	CreatureGenerator cg;
	
	//States
	
	float hungryCoef;
	float wantFuckCoef;
	float fearCoef;
	float idleCoef = 0.2f;
	
	bool eatState;
	bool huntState;
	bool fuckState;
	bool runState;
	bool idleState;
	
	//States end
	
	
	//Magic
	
	string wayTag = "waypoint";
	
	//Magic End
	
	void Start () {
		ui = myUI.Instance;
		cg = CreatureGenerator.Instance;
		im = GetComponent<Creature>();
		
		timer = Time.time;
		lastTime = timer;
		
		food = new List<Transform>();
		lifeFood = new List<Transform>();
		enemy = new List<Transform>();
	}
	
	void Update () {
		timer = Time.time;
		if (target == null)
			Think ();
		else 
			SendMessage("MoveTo", target.position);
	}
	
	void Think(){
		UpdateState();
		DestroyWaypoint();
		if (eatState){
			
			if (SearchFood()){ //если голодаем ищем еду
				SendMessage("MoveTo", target.position); //идем к еде
			} else 
				Run("walk"); //если еды нету, гуляем, упорно пытаемся наткнутся на еду
		} else if (huntState){
			if (SearchLifeFood()){
				SendMessage("MoveTo", target.position);
			} else 
				Run("walk");

		} else if (!runState){
			if (im.IsAdult)
				wantFuckCoef = 0.5f; //переписать на более вменяемое!!!
			
			if (fuckState){
				if (SearchFuck()) //нувыпонели
					SendMessage("MoveTo", target.position);
				else {
					Run ("walk"); //ищем приключений
				}
			} else {
				Run("walk");
			}
		} else {
			Run("run"); //убегаем от вражин
		}
	}
	
	public void InPosition(){
		if (target.tag == wayTag){
			DestroyWaypoint();
			Think();
		}else if (food.Contains(target))
			SendMessage("Eat", target.gameObject);
		
		else if (target.tag == "Creature"){
			
			if (fuckState){
				
				if (!im.IsMale)
					cg.GrowAt(im.TypeID, transform.position.x, transform.position.y);
				
				SendMessage("SetEnergy", -5000);
				UpdateState();
			} else if (huntState) {
				SendMessage("Attack", target.gameObject);
			} else Think();
		}
	}
	
	void UpdateState(){
		
		eatState = fuckState = runState = idleState = false;
		
		float maxcoef = Mathf.Max(hungryCoef, wantFuckCoef, fearCoef, idleCoef);
		
		if (maxcoef == hungryCoef){

			if (food.Count > 0){
				eatState = true;
				Debug.Log("eatState");
			}else {
				huntState = true;
				Debug.Log("huntState");
			}

		}else if (maxcoef == wantFuckCoef){
			fuckState = true;
			Debug.Log("fuckState");
		}else if (maxcoef == fearCoef){
			runState = true;
			Debug.Log("runState");
		}else {
			idleState = true;
			Debug.Log("idleState");
		}
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
		if (food.Count > 0){
			Transform t;
			t = Utils.FindNearest(transform, food);
			
			if (t != null){
				DestroyWaypoint();
				target = t;
				return true;
			} else return false;
		} else return false;
	}

	bool SearchLifeFood(){
		if (lifeFood.Count > 0){
			Transform t;
			t = Utils.FindNearest(transform, lifeFood);
			
			if (t != null){
				DestroyWaypoint();
				target = t;
				return true;
			} else return false;
		} else return false;
	}

	bool SearchEnemy (){
		if (enemy.Count > 0){
			Transform t;
			t = Utils.FindNearest(transform, enemy);
			
			if (t != null){
				DestroyWaypoint();
				target = t;
				NewWayPoint(target.position.ToVector2());
				return true;
			} else return false;
		}	else return false;
	}
	
	bool SearchFuck (){
		
		List<Transform>  tars;
		tars = new List<Transform>();
		
		var ts = GameObject.FindGameObjectsWithTag("Creature");
		
		foreach (var t in ts) {
			
			Creature cp = t.GetComponent<Creature>(); 
			int ct = cp.TypeID;
			
			if (ct == im.TypeID && cp.IsMale != im.IsMale && cp.IsAdult){
				tars.Add(t.transform);
			}
		}
		
		if (tars.Count > 0){
			Transform ta = Utils.FindNearest(transform, tars);
			DestroyWaypoint();
			target = ta;
			return true;
		} else 
			return false;
	}
	
	void SeeFood (Transform t){
		if (!food.Contains(t)) 
			food.Add(t);
		if (food.Count == 1)
			Think();
	}

	void SeeLifeFood (Transform t){
		if (!lifeFood.Contains(t)) 
			lifeFood.Add(t);
		if (lifeFood.Count == 1)
			Think();
	}
	
	void SeeEnemy (Transform t){
		if (!enemy.Contains(t)) 
			enemy.Add(t);
		
		fearCoef = 0.8f; //переписать на более вменяемое!!!
		
		Think();
	}
	
	void EscFood (Transform t){
		if (food.Contains(t)) 
			food.Remove(t);
		food.RemoveAll(o => o == null);
	}
	
	void EscEnemy (Transform t){
		if (!enemy.Contains(t)) 
			enemy.Remove(t);
		enemy.RemoveAll(o => o == null);
	}
	
	public void SetFearCoef (float i) {fearCoef = i; }
	
	public void SetWantFuckCoef (float i) {wantFuckCoef = i; }
	
	public void SetHungryCoef (float i) {hungryCoef = i; }
	
}
