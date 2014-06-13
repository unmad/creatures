using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class VectorExtension{
	public static Vector2 ToVector2 (this Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}
}

public class CreaturePlant : MonoBehaviour {

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
		//speed = Mathf.Lerp(cg.minSpeedRange, cg.maxSpeedRange, 1 - ((float)size / maxSize));
		speed = maxSpeed;

		if (Random.value < 0.5)
			isMale = true;

		alive = true;
		wantFuck = false;
		hp = maxHp;
		food = new List<Transform>();
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
			} else if ((target.gameObject.tag == wayTag)){
				DestroyObject(target.gameObject);
			}
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
		if (target != null && target.tag == wayTag)
				Destroy (target.gameObject);
		Destroy (this.gameObject);
	}

	void DestroyWaypoint(){
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

		if (target != null && target.tag == wayTag)
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
			float dis = float.MaxValue;
			float minDis = float.MaxValue;

			if (target != null && target.tag == wayTag)
				DestroyWaypoint();

			foreach (var tar in food) {
				dis = (transform.position - tar.position).sqrMagnitude;
				if (dis < minDis){
					target = tar;
					minDis = dis;
				}
			}

			return true;
		} else 
			return false;
	}

	void Think(){
		if ((float)energy / (float)maxEnergy < hungry){
			if (!SearchFood())
				Walk();
		} else	Walk();
	}

	void Grow(){
		age++;
		// размер = 0,5 => 1 при возрасте = 0 => 0,15
		if (age < maxAge * 0.15f){
			float ageCoef = ((float)age / maxAge) * 6.66f;
			//Debug.Log(ageCoef);
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
		if (col.gameObject.tag == plTag){
			Transform tar = col.transform;

			if (!food.Contains(tar)) 
				food.Add(tar);
		}
	}

	void OnTriggerExit (Collider col){
//		Transform tar = col.transform;
//		if (food.Contains(tar)) food.Remove(tar);
	}

	bool IsMale { get { return isMale; } }

	int TypeID { get { return typeID; } }

	bool Life { get { return alive; } }

	bool WantFuck { get { return wantFuck; } }

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


	void SetEnergy(int i){
		if (energy + i < maxEnergy){
			energy += i;
		} else {
			energy = maxEnergy;
		}
	}
}
