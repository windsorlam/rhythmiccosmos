using UnityEngine;
using System.Collections;

public class EndComboTrigger : MonoBehaviour {
    public MainLogic logic;
	// Use this for initialization
	void Start () {
	
	}
    //如果光晕漏网，则重置连击数
    void OnTriggerEnter(Collider other)
    {
		if (other.collider.gameObject.tag == "Collection") 
		{
			logic.ResetCombo ();
			logic.hpUI.value -= 0.2f;
			Time.timeScale = 1;

		}
	}

    
}
