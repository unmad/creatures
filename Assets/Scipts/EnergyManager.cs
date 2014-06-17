using UnityEngine;
using System.Collections;

public class EnergyManager : MonoBehaviour {

	int energy;
	int maxEnergy;

	int hp;
	int maxHp;

	void Start () {
		CreatureGenerator cg;
		cg = CreatureGenerator.CG;
		maxEnergy = SendMessage("MaxSize") * cg.sizeToEnergy;
		energy = maxEnergy / 2;
		maxHp = SendMessage("MaxSize");
		hp = maxHp;
	}
	
	void Update () {
		var hpCoef = (float)hp / (float)maxHp;
		var eneCoef = (float)energy / (float)maxEnergy;
		var speedCoef = (hpCoef + eneCoef)/2;
		SendMessage("SpeedCoef", speedCoef);
	}

	public int Energy {get {return energy;}}

	public int MaxEnergy {get {return MaxEnergy;}}

	public int Hp {get {return hp;}}

	public void SetEnergy(int i){
		if (energy + i < maxEnergy){
			energy += i;
		} else {
			energy = maxEnergy;
		}
	}

	public void SetMaxEnergy(int i){maxEnergy = i;}
}
