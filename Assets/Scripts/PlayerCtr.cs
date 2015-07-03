using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerCtr : MonoBehaviour {
	public GameObject failText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag=="Exploder") {
			Debug.Log ("death");
		}
		Time.timeScale = 0;
		GetComponent<AudioSource> ().Play ();
		failText.SetActive (true);
	}

	public void OnFailedClick() {
		Time.timeScale = 1;
		Application.LoadLevel (Application.loadedLevel);
	}

}
