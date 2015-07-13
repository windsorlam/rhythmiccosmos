using UnityEngine;
using System.Collections;

public class GetMusic : MonoBehaviour {

	void OnEnable () {
		DataManager dm = DataManager.Instance;
		string musicPath = dm.musicPath;
		WWW www = new WWW (musicPath);
		//yield return www;
		audio.clip = www.audioClip;
	}
}
