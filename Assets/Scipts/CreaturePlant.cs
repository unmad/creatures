using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class VectorExtension{
	public static Vector2 ToVector2 (this Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}
}

public class CreaturePlant : MonoBehaviour {

	public GameObject logic;
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
	
	void Start () {
		logic = GameObject.FindWithTag ("Logic");
		timer = Time.time;
		lastTime = timer;
	}

	bool InRange (Vector3 pos,Vector3 tar, float range){
		return Vector3.Distance(tar, pos) <= range;
	}

	void Eat (){
		if (target != null) {
			if (target.gameObject.name == "Plant(Clone)") {
				var plant = target.gameObject.GetComponent<Plant>();
				if (plant.size < size * 2){
					SetEnergy(plant.size);
					plant.SetSize(-1000);
				} else {
					SetEnergy(size * 2);
					plant.SetSize(- size * 2);
				}
			} else if ((target.gameObject.name == "waypoint")){
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
		if (target != null && target.name == "waypoint")
				Destroy (target.gameObject);
		Destroy (this.gameObject);
	}

	void DestroyWaypoint(){
		Destroy (target.gameObject);
	}

	void LookAt(Vector3 tar){
		var newRotation = Quaternion.LookRotation(transform.position - tar, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, (float)speed*2);
	}

	void MoveTo(Vector3 tar){
		if (!InRange (transform.position, tar, 0.3f)) {
			LookAt (tar);
			transform.Translate (Vector3.back * speed);
			SetEnergy (- Mathf.RoundToInt(size/10));
			Debug.DrawLine (transform.position, tar);
		} else	{
			Eat ();
		}
	}

	void Walk(){
		int width = logic.GetComponent<UI> ().width;
		int height = logic.GetComponent<UI> ().height;

		var point = transform.position.ToVector2() + Random.insideUnitCircle * 5;
		point.x = Mathf.Clamp(point.x, (-width/2)+1, (width/2)-1);
		point.y = Mathf.Clamp(point.y, (-height/2)+1, (height/2)-1);

		if (target != null && target.name == "waypoint")
			DestroyWaypoint();

		var wayPoint = new GameObject();
		target = wayPoint.transform;
		target.gameObject.name = "waypoint";
		target.gameObject.tag = "waypoint";
		target.position = new Vector3(point.x, point.y, 0f);
	}

	bool SearchFood (){
		//var food = Physics.OverlapSphere(transform.position, 5);


		food.RemoveAll(o => o == null);
		if (food.Count > 0){
			float dis = float.MaxValue;
			float minDis = float.MaxValue;

			if (target != null && target.name == "waypoint")
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
		if ((float)energy / (float)maxEnergy < 0.5){
			if (!SearchFood())
				Walk();
		} else	Walk();
	}

	void Update () {
		timer = Time.time;
		if (ImAlive()) {
			speed = maxSpeed * ((((float)hp/(float)maxHp)+((float)energy/(float)maxEnergy))/2);
			if (timer - lastTime >= 0.5f) {
				if ((float)energy / (float)maxEnergy < 0.1f )
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
		size = Random.Range ((tSize / 4) * 3, tSize);
		maxEnergy = size * 100;
		energy = maxEnergy / 2;
		maxHp = size * 10;
		speed = (10 - (size / 50)) * 0.02f;
		maxSpeed = speed;

		if (Random.value < 0.5)
			isMale = true;

		alive = true;
		wantFuck = false;
		hp = maxHp;
		food = new List<Transform>();
		GetComponentInChildren<Transform>().localScale = Vector3.one * (((float)size / (float)maxSize) + 0.5f);
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.name == "Plant(Clone)"){
			Transform tar = col.transform;
			if (!food.Contains(tar)) food.Add(tar);
		}
	}

	void OnTriggerExit (Collider col){
		Transform tar = col.transform;
		if (food.Contains(tar)) food.Remove(tar);
	}

	bool IsMale { get { return isMale; } }

	int GetTypeID(){
		return typeID;
	}

	bool GetLife (){
		return alive;
	}

	bool GetWantFuck(){
		return wantFuck;
	}

	void SetEnergy(int i){
		if (energy + i < maxEnergy){
			energy += i;
		} else {
			energy = maxEnergy;
		}
	}
}
