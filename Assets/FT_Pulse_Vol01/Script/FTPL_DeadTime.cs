using UnityEngine;
using System.Collections;

public class FTPL_DeadTime : MonoBehaviour {
	public float deadTime;

	void Awake () {
		Destroy (gameObject, deadTime);	
	}
	
	void Update () {
	
	}
}
