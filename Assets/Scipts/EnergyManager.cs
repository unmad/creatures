using UnityEngine;
using System.Collections;

public class EnergyManager : MonoBehaviour {

	int energy;
	int maxEnergy;

	void Start () {
	
	}
	
	void Update () {
	
	}

	public int Energy {get {return energy;}}

	public void SetEnergy(int i){energy = i;}

	public void SetMaxEnergy(int i){maxEnergy = i;}
}
