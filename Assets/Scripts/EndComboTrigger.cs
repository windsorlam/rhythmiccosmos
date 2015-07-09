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
<<<<<<< HEAD
			//logic.hpUI.value -= 0.3f;  //增加HP
=======
			logic.hpUI.value -= 0.3f;  //增加HP
>>>>>>> df5dec6ed3d0dfb1a34ebda42300fcb2c69e17fb

		}
	}

    
}
