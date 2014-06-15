﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public static class VectorExtension{
//	public static Vector2 ToVector2 (this Vector3 v3){
//		return new Vector2 (v3.x, v3.y);
//	}
//}

public class CreaturePlant : MonoBehaviour {

	public GameObject corpsePrefab;
	UI ui;
	int typeID;
	int age;
	int maxAge;
	int size;
	int maxSize;
	public Transform visual;
	int energy;
	int maxEnergy;
	float speed;
	float maxSpeed;
	bool isMale;
	bool alive;
	bool wantFuck;
	int hp;
	int maxHp;

	float timer;
	float lastTime;
	List<Transform> food;
	List<Transform> enemy;
	Transform target;
	CreatureGenerator cg;

	//Magic

	public float starving;
	public float hungry;
	float minScale;
	float maxScale;

	string plTag = "Plant";
	//string plLyr = "Food";
	string wayTag = "waypoint";
	string me = "MeatEater";

	//Magic End
	
	void Start () {
		var logic = GameObject.FindWithTag ("Logic");
		ui = logic.GetComponent<UI>();
		cg = logic.GetComponent<CreatureGenerator>();

		if (cg == null) 
			Debug.Log ("cg = NULL");
		timer = Time.time;
		lastTime = timer;

		age = 0;

		size = Mathf.RoundToInt(maxSize * 0.1f);
		maxEnergy = maxSize * cg.sizeToEnergy;
		energy = maxEnergy / 2;
		maxHp = maxSize;
		speed = maxSpeed;

		if (Random.value < 0.5)
			isMale = true;

		alive = true;
		wantFuck = false;
		hp = maxHp;
		food = new List<Transform>();
		enemy = new List<Transform>();
		visual.localScale = Vector3.one * (Mathf.Lerp(minScale, maxScale, (float)size / maxSize));
	}

	bool InRange (Vector3 pos,Vector3 tar, float range){
		return Vector3.Distance(tar, pos) <= range;
	}

	void Eat (){
		if (target != null) {
			if (target.gameObject.tag == plTag) {
				var plant = target.gameObject.GetComponent<Plant>();
				if (plant.size < maxSize){
					SetEnergy(plant.size);
					plant.SetSize(-plant.size);
				} else {
					SetEnergy(maxSize);
					plant.SetSize(- maxSize);
				}
			} else DestroyWaypoint();
		}
	}

	bool ImAlive (){
		if (age > maxAge) {
			Debug.Log("Die Age");
			return false;
		}
		if (energy <= 0){
			Debug.Log("Die Energy");
			return false;
		}
		if (hp <= 0) {
			Debug.Log("Die HP");
			return false;
		}
		return true;
	}

	void Die(){
		Vector3 pos = transform.position;
		GameObject p = (GameObject)Instantiate(corpsePrefab, pos, transform.rotation);
		var vis = p.GetComponent<CreatureCorpse> ();
		vis.visual.transform.localScale = visual.transform.localScale;

		int pSize;

		if (energy < 1) 
			pSize = size;
		else
			pSize = size * energy;

		p.GetComponent<CreatureCorpse> ().SetSize (pSize);
		DestroyWaypoint();
		Destroy (this.gameObject);

	}

	void DestroyWaypoint(){
		if (target != null && target.tag == wayTag)
		Destroy (target.gameObject);
	}

	void LookAt(Vector3 tar){
		var newRotation = Quaternion.LookRotation(transform.position - tar, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, (float)speed * cg.speedToRotate);
	}

	void MoveTo(Vector3 tar){
		if (!InRange (transform.position, tar, cg.rangeToStop)) {
			LookAt (tar);
			transform.Translate (Vector3.back * speed);
			SetEnergy (- Mathf.RoundToInt(size * speed));
			Debug.DrawLine (transform.position, tar);
		} else	{
			Eat ();
		}
	}

	void Walk(){
		var width = (ui.width / 2) - cg.borderSize;
		var height = (ui.height / 2) - cg.borderSize;

		var point = transform.position.ToVector2() + Random.insideUnitCircle * cg.wayPointRange;
		point.x = Mathf.Clamp(point.x, -width, width );
		point.y = Mathf.Clamp(point.y, -height, height);
		NewWayPoint(point);
	}

	void Run(){
		var width = (ui.width / 2) - cg.borderSize;
		var height = (ui.height / 2) - cg.borderSize;

		var point = target.position.ToVector2 () - transform.position.ToVector2();
		point = transform.position.ToVector2() - point;
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

		food.RemoveAll(o => o == null);
		if (food.Count > 0){

			DestroyWaypoint();
			MinDisTarget (food);
			return true;
		} else 
			return false;
	}

	void MinDisTarget (List<Transform> tars){

		float dis = float.MaxValue;
		float minDis = float.MaxValue;
		foreach (var tar in tars) {
			dis = (transform.position - tar.position).sqrMagnitude;
			if (dis < minDis) {
				target = tar;
				minDis = dis;
			}
		}

	}

	bool SearchEnemy (){
		enemy.RemoveAll(o => o == null);

		if (enemy.Count > 0){
			MinDisTarget (enemy);
			DestroyWaypoint();

			NewWayPoint(target.transform.position.ToVector2());
			return true;
		}
		return false;
	}

	void Think(){

		if ((float)energy / (float)maxEnergy > starving) {
			if (SearchEnemy())
				Run();
			else if (!SearchFood())
				Walk();
		} else if (!SearchFood())
			Walk();
	}

	void Grow(){
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

	void Update () {
		timer = Time.time;
		if (ImAlive()) {
			var hpCoef = (float)hp/(float)maxHp;
			var eneCoef = (float)energy/(float)maxEnergy;
			var speedCoef = (hpCoef + eneCoef)/2;
			speed = maxSpeed * speedCoef;
			if (timer - lastTime >= cg.birthdayTime) {
				if ((float)energy / (float)maxEnergy < starving )
					hp--;
				lastTime = timer;
				Grow();
			}
			if (target != null)
				MoveTo(target.position);
			else Think ();
		} else {
			Die();
		}
	}

	void OnTriggerEnter (Collider col) {
		Transform tar = col.transform;
		if (col.gameObject.tag == plTag){
			if (!food.Contains(tar)) 
				food.Add(tar);
		} else if (col.gameObject.tag == me){
			//if (col.gameObject.GetComponent<CreatureMeat>().Size > size * 2) //может меня сожрать
				if (!enemy.Contains(col.transform))
					enemy.Add(col.transform);
		}
	}

	void OnTriggerExit (Collider col){
		Transform tar = col.transform;

		if (col.gameObject.tag == plTag)
			if (food.Contains(tar)) food.Remove(tar);

		if (col.gameObject.tag == me)
			if (enemy.Contains(tar)) enemy.Remove(tar);
	}

	public bool IsMale { get { return isMale; } }

	public int TypeID { get { return typeID; } }

	public bool Life { get { return alive; } }

	public bool WantFuck { get { return wantFuck; } }

	public int Size { get { return size; } }

	public void SetMaxAge (int i){
		maxAge = i;
	}
	public void SetMaxSize (int i){
		maxSize = i;
	}
	public void SetMinScale (float i){
		minScale = i;
	}
	public void SetMaxScale (float i){
		maxScale = i;
	}
	public void SetTypeID (int i){
		typeID = i;
	}

	public void SetMaxSpeed (float i){
		maxSpeed = i;
	}


	public void SetEnergy(int i){
		if (energy + i < maxEnergy){
			energy += i;
		} else {
			energy = maxEnergy;
		}
	}
}
