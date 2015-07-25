//此脚本附加在Spwaner上，用来生成光晕
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spwaner :  MonoBehaviour
{
	public MainLogic logic;
	public MultiMainLogic multiLogic;

    public GameObject elementPrefab;    // 光晕的预设体
    public Transform t;                 //光晕的克隆体预先设置的位置
	public Queue<float> haloIntervalQueue = new Queue<float>();  //interval to create halo
	public float speed;

	int current;

    float timer = 0;
	// Use this for initialization
	void Start () {
		DataManager dm=DataManager.Instance;
		haloIntervalQueue = dm.onsetList;
	}
	
	// Update is called once per frame
	void Update () {
		if(UIEvents.multiMode == true) {
			speed = multiLogic.currentSpeed;
		} else {
			speed = logic.currentSpeed;
		}

        timer += Time.deltaTime;
        if (timer > haloIntervalQueue.Peek())
        {
			haloIntervalQueue.Dequeue();
            timer = 0;
			if(UIEvents.multiMode == true) {
				current = multiLogic.nextHighlight;
			} else {
				current = logic.nextHighlight;
			}

            GameObject g = Instantiate(elementPrefab) as GameObject;    //克隆一个光晕
            g.transform.position = t.position;          //光晕的位置与欧拉角和t一致
            g.transform.eulerAngles = t.eulerAngles;
			g.GetComponent<ElementMovement>().speed = -speed;
			g.transform.RotateAround(this.transform.position, Vector3.forward, Random.Range(current * 36 , (current+1) * 36));//光晕随机绕Spwaner旋转
        }
	}
}
