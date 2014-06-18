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

	float starving = 0.3f;
	float hungry = 0.8f;

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
	}

	void Eat (GameObject target){

		if (energy > maxEnergy * hungry){
			SendMessage("Starving", false);
		}

		if (energy == maxEnergy)
			SendMessage("Think");

		int eatE = eat;

		if (energy + eatE < maxEnergy){
			eatE = eat;
		} else {
			eatE = maxEnergy - energy;
		}

		Eating(eatE, target);
	}

	void Eating (int eat, GameObject target){
		var vEat = new Eating();
		vEat.eat = eat;
		vEat.i = this.gameObject;
		target.SendMessage("EatMe", vEat);
	}

	public void IStarving (){
		if ((float)energy / maxEnergy < starving)
			SendMessage("Starving", true);
		else
			SendMessage("Starving", false);
	}

	public void SetEnergy(int i){energy += i;}

	public void SetMaxEnergy(int i){maxEnergy = i;}

	public void SetEat(int i){eat = i;}

	public void SetHp(int i){hp += i;}

	public void SetMaxHp(int i){maxHp = i;}

}
