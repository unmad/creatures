using UnityEngine;
using System.Collections;

public class Eating {
	
	public GameObject i;
	public int eat;
}

public class EnergyManager : MonoBehaviour {

	int energy;
	int maxEnergy;
	int eat;
	int hp;
	int maxHp;

	float timer;
	float lastTime;

	public float starving;
	public float hungry;

	CreatureGenerator cg;

	void Start () {
		cg = CreatureGenerator.Instance;
		energy = maxEnergy / 2;
		hp = maxHp;
	}
	
	void Update () {
		timer = Time.time;
		var hpCoef = (float)hp / (float)maxHp;
		var eneCoef = (float)energy / (float)maxEnergy;
		var speedCoef = (hpCoef + eneCoef)/2;
		SendMessage("SetSpeedCoef", speedCoef);

		if (timer - lastTime >= cg.birthdayTime) {
			if ((float)energy / maxEnergy < starving )
				hp--;
			lastTime = timer;
		}

		if (energy / maxEnergy < starving)
			SendMessage("starving", true);
	}

	void Eat (GameObject target){
		if (target != null) {

			int eatE = eat;

			if (energy + eatE < maxEnergy){
				eatE = eat;
			} else {
				eatE = maxEnergy - energy;
			}

			Eating(eatE, target);
		}
	}

	void Eating (int eat, GameObject target){
		var vEat = new Eating();
		vEat.eat = eat;
		target.SendMessage("EatMe", this.gameObject);
	}



	public void SetEnergy(int i){
		energy += i;
	}

	public void SetMaxEnergy(int i){maxEnergy = i;}

	public void SetEat(int i){eat = i;}

	public void SetHp(int i){hp += i;}

	public void SetMaxHp(int i){maxHp = i;}

}
