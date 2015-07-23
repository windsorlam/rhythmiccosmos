using UnityEngine;
using System.Collections;

public class AutoCollectionTrigger : MonoBehaviour {
	public MainLogic logic;
	public MultiMainLogic multiLogic;
	public EndComboTrigger trigger;


	// Use this for initialization
	void Start () {
		
	}

	GameObject currentCollection;

	//如果光晕漏网，则重置连击数
	void OnTriggerEnter(Collider other)
	{

		if (!logic.boosting ){
			trigger.gameObject.SetActive(true);
			return;
		}

		if (other.collider.gameObject.tag == "Collection") {

			trigger.gameObject.SetActive(false);

			
			logic.hpUI.value += (1 + logic.combo * 0.1f) * 0.2f;  //增加HP
			logic.score += 10 + logic.combo * logic.combo / 10;
			logic.scoreUI.text = ((int)logic.score).ToString();     //加分
			
			GameObject go = Instantiate(logic.disappearFx) as GameObject;// 生成一个光晕销毁后的粒子特效
			go.transform.parent = logic.player.transform;    
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = new Vector3(-90, 0, 0);//调整特效角度

			Destroy(currentCollection); //销毁光晕

			Destroy(go, 1);//1秒钟后销毁此特效
			logic.OnComboAdd();
		}
	}


	void OnTriggerExit(Collider other)
	{
		if (other.collider.gameObject.tag != "Collection") return;   
		currentCollection = null;              
	}
	
}