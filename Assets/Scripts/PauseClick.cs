using UnityEngine;
using System.Collections;

public class PauseClick : MonoBehaviour {

	public bool paused = false;
	private float time;
	AudioSource music;
	float timer = 0;
	float startPoint = 175 / 80;

	void Start()
	{
		music = Camera.main.GetComponent<AudioSource> ();
	}


	void Update()
	{
		timer += Time.deltaTime; 
//		if (Time.timeScale < 1.5 && !paused)
//		{
//			Time.timeScale += 0.0002f;
//		}
		if (!music.isPlaying && !paused  && timer > startPoint) 
		{
			music.Play();
		}

	}


	void OnClick ()
	{
		if(!paused)
		{
			time =Time.timeScale;
			Time.timeScale = 0;
			paused = true;
			music.Pause();
		}
		else
		{
			Time.timeScale=time;
			paused=false;
			music.Play();
		}

	}

}
