using UnityEngine;
using System.Collections;
using Parse;

public class GetScore : MonoBehaviour {

	public UILabel scoreUI;         

	public string score = "test";

	// Use this for initialization
	void Start () {
		//ParseObject gameScore;




		ParseQuery<ParseObject> query = ParseObject.GetQuery("Score");
		query.GetAsync("o3SshLrPsB").ContinueWith(t => { 
			ParseObject gameScore = t.Result;	
			score = gameScore.Get<string>("objectID");
			Debug.Log(score);
			score = "hello";
			Debug.Log(score);
		});

		//score = "world";
		scoreUI.text = score;
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
