using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class ShowHighScores : MonoBehaviour {
	
	IEnumerable<ParseObject> results;
	private Vector2 scrollViewVector = Vector2.zero;
	
	// Use this for initialization
	void Start () {
		GetScores();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		if (ParseUser.CurrentUser != null)
		{
		// Make a group on the center of the screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 150, Screen.height / 2 - 132, 300, 400));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

		// Begin the ScrollView
		scrollViewVector = GUI.BeginScrollView (new Rect (0, 0, 300, 300), scrollViewVector, new Rect (0, 0, 300, 1000));

		// We'll make a box so you can see where the group is on-screen.
		GUI.Box (new Rect (0,0,300,1000), "High Scores:");
			var y = 30;
			if(results != null){
	    	foreach(var score in results)
				{
					GUI.Label (new Rect (115, y, 100, 30), score.Get<string>("playerName") + " " + score.Get<string>("score"));
					y = y + 30;
				}
			}
		// End the ScrollView
		GUI.EndScrollView();
		// End the group we started above. This is very important to remember!
		GUI.EndGroup ();
		if (GUI.Button (new Rect (Screen.width / 2 - 40, Screen.height / 2 + 170,80,30), "Main Menu"))
		{
			 Application.LoadLevel("Demo");
		}
		}
		else
		{
			// show the signup or login screen
			 Application.LoadLevel("Demo");
		}
		
	}
	
	void GetScores()
	{
		var query = ParseObject.GetQuery("GameScore")
			.OrderByDescending("score");
			query.FindAsync().ContinueWith(t =>
			{
			    results = t.Result;
			});
	}
}
