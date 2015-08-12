using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

public class GetScore : MonoBehaviour {
	
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



}
