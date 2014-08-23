using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreatureCounter : MonoBehaviour {
	public Text txt;
	CreatureGenerator cg;
	myUI ui;
	
	void Start () {
		ui = myUI.Instance;
		cg = CreatureGenerator.Instance;
		txt = GetComponent<Text>();
	}
	
	void Update () {
		if (ui.playing){
			txt.text = cg.creatures.Count.ToString();
		} else {
			txt.text = ui.countOfCreature.ToString();
		}
	}
}
