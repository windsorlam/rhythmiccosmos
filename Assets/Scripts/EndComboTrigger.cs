using UnityEngine;
using System.Collections;

public class EndComboTrigger : MonoBehaviour {
    public MainLogic logic;
	public MultiMainLogic multiLogic;
	// Use this for initialization
	void Start () {
	
	}
    //如果光晕漏网，则重置连击数
    void OnTriggerEnter(Collider other)
    {
		if (other.collider.gameObject.tag == "Collection" && UIEvents.multiMode == false) {
			logic.ResetCombo ();
			logic.hpUI.value -= 0.2f;
			Time.timeScale = 1;
		} else if (other.collider.gameObject.tag == "Collection" && UIEvents.multiMode == true) {
			multiLogic.ResetCombo ();
			multiLogic.hpUI.value -= 0.2f;
			Time.timeScale = 1;
		}
	}

    
}
