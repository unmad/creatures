using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

	public GameObject corpsePrefab;
	
	UI ui;
	
	int typeID;
	int age;
	int maxAge;
	int size;
	int maxSize;
	
	bool isMale;
	bool isAdult;
	bool alive;
	
	float timer;
	float lastTime;

	CreatureGenerator cg;

	// Use this for initialization
	void Start () {
		cg = CreatureGenerator.Instance;

		age = 0;
		isAdult = false;
		
		size = Mathf.RoundToInt(maxSize * 0.1f);
		
		
		
		if (Random.value < 0.5)
			isMale = true;
		
		alive = true;
		
		SendMessage("SetMaxEnergy", maxSize * cg.sizeToEnergy);
		SendMessage("SetMaxHp", maxSize);
	}
	
	// Update is called once per frame
	void Update () {
		timer = Time.time;
		if (age < maxAge) {
			if (timer - lastTime >= cg.birthdayTime) {
				lastTime = timer;
				Grow();
			}			
		} else Die();
	}

	void Grow(){
		age++;
		if (age < maxAge * 0.15f){
			float ageCoef = ((float)age / maxAge) * 6.66f;
			size = Mathf.RoundToInt(Mathf.Lerp((float)maxSize * 0.5f, (float)maxSize, ageCoef)); //какаята страшная магия О_о
			SendMessage("SetScale", (float)size / maxSize);
			
		} else if (!isAdult) {
			isAdult = true;
			//size = maxSize;
			//SendMessage("SetScale",1);
		}
	}

	void Die(){
		Vector3 pos = transform.position;
		GameObject p = (GameObject)Instantiate(corpsePrefab, pos, transform.rotation);
		int pSize;
		pSize = size;
		p.GetComponent<CreatureCorpse> ().SetSize (pSize);
		SendMessage("DestroyWaypoint");
		Destroy (this.gameObject);
		
	}

	public void SetAge (int i) {age = i; }
	
	public void SetMaxAge (int i) {maxAge = i; }
	
	public void SetSize (int i){size = i; }
	
	public void SetMaxSize (int i){maxSize = i; }
	
	public void SetTypeID (int i){typeID = i; }

	public bool IsMale { get { return isMale; } }
	
	public bool IsAdult { get { return isAdult; } }
	
	public int TypeID { get { return typeID; } }
	
	public bool Life { get { return alive; } }
	
	public int Size { get { return size; } }
}
