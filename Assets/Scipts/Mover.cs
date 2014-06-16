using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	float speed;
	float MaxSpeed;
	float speedToRotate;
	float rangeToStop;
	int energyToMove;


	void Start () {
		speedToRotate = CreatureGenerator.CG.speedToRotate;
		rangeToStop = CreatureGenerator.CG.rangeToStop;
	}

	void Update () {
	
	}

	void MoveTo(Vector3 tar){
		if (!InRange (transform.position, tar, rangeToStop)) {
			var newRotation = Quaternion.LookRotation(transform.position - tar, Vector3.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, (float)speed * speedToRotate);
			transform.Translate (Vector3.back * speed);
			SendMessage("SetEnergy", - energyToMove);
			//Debug.DrawLine (transform.position, tar);
		}
	}

	bool InRange (Vector3 pos,Vector3 tar, float range){
		return Vector3.Distance(tar, pos) <= range;
	}

	void SetSpeed(float i){speed = i;}

	void SetEnergyToMove(int i){energyToMove = i;}

}
