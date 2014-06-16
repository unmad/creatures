using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureBase : MonoBehaviour {
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


}
