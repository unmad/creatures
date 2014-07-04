using UnityEngine;
using System.Collections;
using Islands;

public static class VectorExtension{
	public static Vector2 ToVector2 (this Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}
}

public sealed class UI : Singleton<UI> {

	public GameObject back;
	public int countOfCreature;
	public int countOfTypes;
	public int soilRichness;
	public int width;
	public int height;
	public GUIText text;
	
	public bool play = false;
	public bool playing = false;

	PlantGenerator pg;
	CreatureGenerator cg;

	void Start (){
		pg = PlantGenerator.Instance;
		cg = CreatureGenerator.Instance;

		Camera.main.orthographicSize = (float)height/2;
		back.transform.localScale = new Vector3(width, height, 0f);
	}
	
	void OnGUI () {

		if(GUI.Button(new Rect(5, 10, 80, 20), "play")) {
			PlayPause();
		}

		countOfCreature = (int) GUI.HorizontalScrollbar (new Rect (5, 50, 80, 20), countOfCreature, 1, 2, 10);
		countOfTypes = (int) GUI.HorizontalScrollbar (new Rect (5, 70, 80, 20), countOfTypes, 1, 1, 10);
		soilRichness = (int) GUI.HorizontalScrollbar (new Rect (5, 90, 80, 20), soilRichness, 1, 5, 100);

		cg.genmeateater = GUI.Toggle(new Rect(5, 110, 80, 20), cg.genmeateater, "meat eater");
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
			pg.gen = false;
			cg.gen = false;
			playing = true;
			text.text = "Playing";
		}else{
			playing = false;
			text.text = "Paused";
		}
	}
}