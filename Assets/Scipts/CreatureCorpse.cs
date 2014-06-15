using UnityEngine;
using System.Collections;

public class CreatureCorpse : MonoBehaviour {

	int size;
	float timer;
	float lastTime;
	float nextTime = 0.5f;
	public Transform visual;
	
	void Update () {
		timer = Time.time;
		if (timer - lastTime >= nextTime) {
			size --;
			lastTime = timer;
		}
		if (size <= 0)
			Destroy (this.gameObject);
	}

	public void SetSize(int i){
		size += i;
	}


	public int GetSize { get { return size; } }

}
