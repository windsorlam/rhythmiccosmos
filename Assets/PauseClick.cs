using UnityEngine;
using System.Collections;

public class PauseClick : MonoBehaviour {

	public bool paused = false;
	private float time;

	void Update()
	{
		if (Time.timeScale < 1.4 && !paused)
		{
			Time.timeScale += 0.0002f;
		}
		
		Debug.Log (Time.timeScale);
	}


	void OnClick ()
	{
		if(!paused)
		{
			time =Time.timeScale;
			Time.timeScale =0;
			paused = true;
		}
		else
		{
			Time.timeScale=time;
			paused=false;
		}

	}

}
