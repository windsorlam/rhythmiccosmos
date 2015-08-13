using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {



	void Start(){
		AudioSource music = GetComponent<AudioSource> ();
		DataManager dm = DataManager.Instance;
		string path = "file://"+dm.dataPath+"/"+dm.musicPath;
		Debug.Log (path);
		WWW www = new WWW (path);
		music.clip = www.audioClip;
		music.Play();
	}

	void Update () {
//		AudioSource music = GetComponent<AudioSource> ();
//		if (!music.isPlaying  ) {
//			music.Play();
//		}
	}
}
