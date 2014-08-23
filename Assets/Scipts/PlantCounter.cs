using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlantCounter : MonoBehaviour {
	public Text txt;
	myUI ui;
	PlantGenerator pg;

	void Start () {
		ui = myUI.Instance;
		pg = PlantGenerator.Instance;
		txt = GetComponent<Text>();
	}

	void Update () {
		if (ui.playing){
			txt.text = pg.countOfPlants.ToString();
		} else {
			txt.text = ui.soilRichness.ToString();
		}
	}
}
