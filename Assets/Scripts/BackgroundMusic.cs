using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

	void Start(){
		AudioSource music = GetComponent<AudioSource> ();
		DataManager dm = DataManager.Instance;
		string musicPath = dm.musicPath;
		WWW www = new WWW ("file://"+musicPath);
		music.clip = www.audioClip;
		music.Play ();
	}

	void Update () {
		AudioSource music = GetComponent<AudioSource> ();
		if (!music.isPlaying) {
			music.Play();
		}
	}
}
