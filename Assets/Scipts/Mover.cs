using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	float speed;
	float maxSpeed;
	float speedToRotate;
	float rangeToStop;
	float speedCoef;

	int energyToMove;



	void Start () {
		
	}

	void Update () {
		
	}

	void MoveTo(Vector3 tar){
		if (!Utils.InRange (transform.position, tar, rangeToStop)) {
			var newRotation = Quaternion.LookRotation(transform.position - tar, Vector3.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, (float)speed * speedToRotate);
			transform.Translate (Vector3.back * speed);
			SendMessage("SetEnergy", - Mathf.RoundToInt(energyToMove * speedCoef));
			Debug.DrawLine (transform.position, tar);
		} else SendMessage("InPosition");
	}

	public void SetSpeed(float i){speed = i;}

	public void SetSpeedCoef(float i){
		speedCoef = i;
		speed = maxSpeed * speedCoef;
	}

	public void SetEnergyToMove(int i){energyToMove = i;}

	public void SetMaxSpeed (float i){maxSpeed = i;}

	public void SetSpeedToRotate(float i){speedToRotate = i;}

	public void SetRangeToStop(float i){rangeToStop = i;}

}
