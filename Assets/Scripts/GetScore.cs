using UnityEngine;
using System.Collections;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Threading.Tasks;
=======
>>>>>>> df5dec6ed3d0dfb1a34ebda42300fcb2c69e17fb
using Parse;

public class GetScore : MonoBehaviour {

<<<<<<< HEAD
	public  UILabel [] scoreUI;  

	int score;

	IEnumerable<ParseObject> results;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(RunSomeLongLastingTask());
	}
	void LateUpdate(){

		int i = 0;
		if(results != null){
			foreach(var item in results)
			{
				score = (int)item.Get<float>("score");
				scoreUI[i].text =score.ToString();
				i++;
			}
		}

	}

	void GetScores()
	{
		var query = ParseObject.GetQuery("Score")
			.OrderByDescending("score");
		query = query.Limit (10);
		query.FindAsync().ContinueWith(t =>
	                               {
			results = t.Result;
		});
	}

	IEnumerator RunSomeLongLastingTask()
	{	var query = ParseObject.GetQuery("Score").OrderByDescending("score");

		query = query.Limit (10);

		Task queryTask = query.FindAsync ().ContinueWith(t =>
		                                                             {
			results = t.Result;
		});

		while (!queryTask.IsCompleted) {
			yield  return null; // wait until next frame
		}


	}


=======
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
>>>>>>> df5dec6ed3d0dfb1a34ebda42300fcb2c69e17fb
}
