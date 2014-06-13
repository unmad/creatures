using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	public GameObject back;
	public int countOfCreature;
	public int countOfTypes;
	public int soilRichness;
	public int width;
	public int height;
	public GUIText text;
	
	public bool play;
	public bool playing;


	//for plant generator


	//for Creature generator



	void Start (){
		Camera.main.orthographicSize = (float)height/2;
		back.transform.localScale = new Vector3(width, height, 0f);
	}
	
	void OnGUI () {

		if(GUI.Button(new Rect(5, 10, 60, 20), "play")) {
			PlayPause();
		}

		countOfCreature = (int) GUI.HorizontalScrollbar (new Rect (5, 50, 60, 20), countOfCreature, 1, 2, 10);
		countOfTypes = (int) GUI.HorizontalScrollbar (new Rect (5, 70, 60, 20), countOfTypes, 1, 1, 10);
		soilRichness = (int) GUI.HorizontalScrollbar (new Rect (5, 90, 60, 20), soilRichness, 1, 1, 100);
	}
	void Update (){
		if (play)
			play = false;

		if (Input.GetButtonDown ("Horizontal")) {
			countOfTypes += (int)Input.GetAxis ("Horizontal");
			Debug.Log("types " + countOfTypes);
		}
		if (Input.GetButtonDown ("Vertical")) {
			countOfCreature += (int)Input.GetAxis ("Vertical");
			Debug.Log("creatures " + countOfCreature);
		}
		if (Input.GetButtonUp ("Jump")) {
			PlayPause();
		}
	}

	void PlayPause (){
		play = true;
		if (!playing){
			GetComponent<CreatureGenerator>().gen = false;
			GetComponent<PlantGenerator>().gen = false;
			playing = true;
			text.text = "Playing";
		}else{
			playing = false;
			text.text = "Paused";
		}
	}
}