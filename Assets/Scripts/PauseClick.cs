using UnityEngine;
using System.Collections;

public class PauseClick : MonoBehaviour {

	public bool paused = false;
	private float time;
	AudioSource music;
	float timer = 0;
	float startPoint = 180.5f / 80;

	void Start()
	{
		music = Camera.main.GetComponent<AudioSource> ();
		StartCoroutine( PlaySoundAfterDelay( music, startPoint ) );

	}


	void Update()
	{
	

//		if (!music.isPlaying && Time.time > startPoint) 
//		{
//			music.Play();
//		}

	}


	IEnumerator PlaySoundAfterDelay( AudioSource audioSource, float delay )
	{
		if( audioSource == null )
			yield break;
		yield return new WaitForSeconds( delay );
		audioSource.Play();
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
