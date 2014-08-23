using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TypeCounter : MonoBehaviour {
	public Text txt;
	myUI ui;

	CreatureGenerator cg;

	void Start () {
		ui = myUI.Instance;
		cg = CreatureGenerator.Instance;
		txt = GetComponent<Text>();
	}

	void Update () {
		if (ui.playing){
			//txt.text = cg.creatures.Count.ToString();
		} else {
			txt.text = ui.countOfTypes.ToString();
		}
	}
}
