using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public bool gender;
	public bool life;
	public bool wantFuck;
	int hp;
	public int maxHp;

	float timer;
	float lastTime;
	List<Transform> foods;
	List<Transform> enemy;
	Transform target;
	
	void Start () {
		logic = GameObject.FindWithTag ("Logic");
		timer = Time.time;
		lastTime = timer;
	}

	bool InRange (Vector3 pos,Vector3 tar, float range){
		if ((Mathf.Abs (pos.x - tar.x) < range) && (Mathf.Abs (pos.y - tar.y) < range)) {
					return true;
			} else
					return false;
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
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * speed * 100);
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
		float x, y;

		x = Random.Range (-5f, 5f);
		y = Random.Range (-5f, 5f);

		float maxX, maxY, minX, minY;
		maxX = width / 2;
		maxY = height / 2;
		minX = -width / 2;
		minY = -height / 2;

		float newX = transform.position.x + x;
		float newY = transform.position.y + y;

		//Debug.Log ("newx = " + newX + " newy = " + newY);

		if (newX > maxX){
			newX = maxX - 1f;
		}
		if (newX < minX){
			newX = minX + 1f;
		}
		if (newY > maxY){
			newY = maxY - 1f;
		}
		if (newY < minY){
			newY = minY + 1f;
		}

		x = newX - transform.position.x;
		y = newY - transform.position.y;

		//Debug.Log ("x= " + x + "y= " + y);

		if (target != null && target.name == "waypoint")
			DestroyWaypoint();
		var wayPoint = new GameObject();
		target = wayPoint.transform;
		target.gameObject.name = "waypoint";
		target.gameObject.tag = "waypoint";
		target.position = transform.position;
		target.Translate(new Vector3(x, y, 0f));
	}

	bool SearchFood (){
		foods.RemoveAll(delegate (Transform o) { return o == null; });
		if (foods.Count > 0){
			float dis = 99999f;
			float dis2 = 99999f;
			foreach (var tar in foods) {
				if (tar != null){
					dis = (Vector3.Distance(transform.position,tar.position));
					if (dis < dis2){
						if (target != null && target.name == "waypoint")
							DestroyWaypoint();
						target = tar;
						dis2 = dis;
					}
				}
			}
			return true;
		} else return false;
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
			gender = true;
		life = true;
		wantFuck = false;
		hp = maxHp;
		foods = new List<Transform>();
		GetComponentInChildren <Transform> ().localScale = new Vector3 ((float)size / (float)maxSize, (float)size / (float)maxSize, (float)size / (float)maxSize);
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.name == "Plant(Clone)"){
			Transform tar = col.transform;
			if (!foods.Contains(tar)) foods.Add(tar);
		}
	}

	void OnTriggerExit (Collider col){
		Transform tar = col.transform;
		if (foods.Contains(tar)) foods.Remove(tar);
	}

	bool GetGender(){
		return gender;
	}
	int GetTypeID(){
		return typeID;
	}
	bool GetLife (){
		return life;
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
