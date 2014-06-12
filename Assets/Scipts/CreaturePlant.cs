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
	public int typeID;
	public int age;
	public int maxAge;
	public int size;
	public int maxSize;
	int energy;
	int maxEnergy;
	float speed;
	float maxSpeed;
	public bool isMale;
	public bool alive;
	public bool wantFuck;
	int hp;
	public int maxHp;

	float timer;
	float lastTime;
	List<Transform> food;
	List<Transform> enemy;
	Transform target;

	//Magic

	float birthdayTime = 0.5f;
	float starving = 0.1f;
	float hungry = 0.5f;
	float wayPointRange = 5f;
	float rangeToStop = 0.3f;
	float speedToRotate = 2f;
	float borderSize = 1f;
	float sizeRangeCoef = 0.2f;
	float minScale = 0.5f;
	float minSpeedRange = 0.02f;
	float maxSpeedRange = 0.1f;

	int sizeToEnergy = 100;
	int sizeToHp = 10;
	int energyToMove = 50;
	int energyToEat = 2;

	string plTag = "Plant";
	string plLyr = "Food";
	string wayTag = "waypoint";
	string loTag = "Logic";

	//Magic End
	
	void Start () {
		ui = GameObject.FindWithTag (loTag).GetComponent<UI>();
		timer = Time.time;
		lastTime = timer;
	}

	bool InRange (Vector3 pos,Vector3 tar, float range){
		return Vector3.Distance(tar, pos) <= range;
	}

	void Eat (){
		if (target != null) {
			if (target.gameObject.tag == plTag) {
				var plant = target.gameObject.GetComponent<Plant>();
				if (plant.size < size * energyToEat){
					SetEnergy(plant.size);
					plant.SetSize(-1000);
				} else {
					SetEnergy(size * energyToEat);
					plant.SetSize(- size * energyToEat);
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
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, (float)speed*speedToRotate);
	}

	void MoveTo(Vector3 tar){
		if (!InRange (transform.position, tar, rangeToStop)) {
			LookAt (tar);
			transform.Translate (Vector3.back * speed);
			SetEnergy (- Mathf.RoundToInt(size/energyToMove));
			Debug.DrawLine (transform.position, tar);
		} else	{
			Eat ();
		}
	}

	void Walk(){
		var width = (ui.width - borderSize) / 2;
		var height = (ui.height - borderSize) / 2;

		var point = transform.position.ToVector2() + Random.insideUnitCircle * wayPointRange;
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

	void Update () {
		timer = Time.time;
		if (ImAlive()) {
			var hpCoef = (float)hp/(float)maxHp;
			var eneCoef = (float)energy/(float)maxEnergy;
			var speedCoef = (hpCoef + eneCoef)/2;
			speed = maxSpeed * speedCoef;
			if (timer - lastTime >= birthdayTime) {
				if ((float)energy / (float)maxEnergy < starving )
					hp--;
				lastTime = timer;
				age++;
			}
			if (target != null)
				MoveTo(target.position);
			else Think ();
		} else {
			Die();
		}
	}

	public void Generate (int type,int tSize){
		typeID = type;
		age = 0;
		size = Mathf.RoundToInt( Random.Range ((float)tSize - tSize * sizeRangeCoef, tSize + tSize * sizeRangeCoef));
		maxEnergy = size * sizeToEnergy;
		energy = maxEnergy / 2;
		maxHp = size * sizeToHp;
		speed = Mathf.Lerp(minSpeedRange, maxSpeedRange, 1 - ((float)size / maxSize));
		maxSpeed = speed;

		if (Random.value < 0.5)
			isMale = true;

		alive = true;
		wantFuck = false;
		hp = maxHp;
		food = new List<Transform>();
		GetComponentInChildren<Transform>().localScale = Vector3.one * (((float)size / (float)maxSize) + minScale);
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == plTag){
			Transform tar = col.transform;

			if (!food.Contains(tar)) 
				food.Add(tar);
		}
	}

	void OnTriggerExit (Collider col){
		Transform tar = col.transform;
		if (food.Contains(tar)) food.Remove(tar);
	}

	bool IsMale { get { return isMale; } }

	int TypeID { get { return typeID; } }

	bool Life { get { return alive; } }

	bool WantFuck { get { return wantFuck; } }

	void SetEnergy(int i){
		if (energy + i < maxEnergy){
			energy += i;
		} else {
			energy = maxEnergy;
		}
	}
}
