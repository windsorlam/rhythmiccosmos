//此脚本附加在Spwaner上，用来生成光晕
using UnityEngine;
using System.Collections;

public class Spwaner :  MonoBehaviour
{
	public MainLogic logic;
    public GameObject elementPrefab;    // 光晕的预设体
    public Transform t;                 //光晕的克隆体预先设置的位置

    public float interval;
	int current;

    float timer = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            timer = 0;
			current = logic.nextHighlight;

            GameObject g = Instantiate(elementPrefab) as GameObject;    //克隆一个光晕
            g.transform.position = t.position;          //光晕的位置与欧拉角和t一致
            g.transform.eulerAngles = t.eulerAngles;
			g.transform.RotateAround(this.transform.position, Vector3.forward, Random.Range((current-1) * 36 , (current+1) * 36));//光晕随机绕Spwaner旋转
        }
	}
}
