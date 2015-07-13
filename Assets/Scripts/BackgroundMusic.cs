using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {
	public AudioSource music;
	// Use this for initialization
	void Start () {
		if (!music.isPlaying) {
			music.Play();
		}
	}
}
