//此脚本附加在光晕和隧道上，让光晕和隧道向后移动，这样玩家相对就往前移动了
using UnityEngine;
using System.Collections;

public class ElementMovement : MonoBehaviour {
    public float speed ;     //移动速度
    public float deadTime;  //销毁时间

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, deadTime);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward * speed * Time.deltaTime);
	}
}
