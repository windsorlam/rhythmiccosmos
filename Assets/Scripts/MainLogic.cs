//此脚本attach在player这个物体上

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VacuumShaders.CurvedWorld;    //shader插件，用来扭曲隧道
using DG.Tweening;                  //Dotween插件
using Parse;

public class MainLogic : MonoBehaviour {
	
	public GameObject elementPrefab0;
	public GameObject elementPrefab1;
	public GameObject elementPrefab2;
	public GameObject elementPrefab3;
	public GameObject elementPrefab4;
	public GameObject elementPrefab5;
	public GameObject elementPrefab6; 
	public GameObject elementPrefab7;
	public GameObject elementPrefab8;
	public GameObject elementPrefab9;

	public Material[] skyboxMats;
	public GameObject[] airCrafts;

	public Material polygonMat;         //10边形的材质
	public Material highlightMat;       //轨道高亮的材质
	public Material[] originalMat;        //轨道原本的材质
	
	public int nextHighlight = 0;           //高亮的轨道索引, next time
	
	int currentHighlight = 0;        //高亮的轨道索引, current
	
	Queue highlight = new Queue();   //Highlight to change
	
	int nextDir = 1;              //下次高亮轨道的方向：-1 right ，1 left
	
	public int currentTrack = 0;        //当前飞机所在轨道
	
	float interval;                     //每间隔多少秒克隆一个隧道
	
	float highlightChangeInterval;      //间隔多少秒, currentHighLight change to nextHighLight
	
	float timer = 0;                    //克隆隧道计时器
	
	float dirTimer = 0;                 //改变隧道扭曲方向的计时器
	
	float highlightTimer = 0;
	
	float scoreHighlightTimer = 0;
	
	public float dirInterval = 5;       //每隔5秒改变一次隧道方向
	
	public float highlightInterval = 4; //Change highlight tracks

	public Queue<float> highlightIntervalQueue = new Queue<float>(); 
	
	public float currentSpeed = 80;     //当前隧道移动速度
	
	public float currentOffset = 25;    //当前隧道的间隔
	
	GameObject current;                 //当前隧道
	
	public CurvedWorld_GlobalController curveCtr;   //扭曲shader总控
	
	public GameObject pivot;            //中心点，飞船是围绕此中心点旋转
	
	GameObject player;                  //玩家飞船
	
	public float playerSpeed;           //飞船的旋转速度
	
	public GameObject disappearFx;      //光晕消失后的特效
	
	public GameObject comboUI;          //连击UI
	
	public UISlider hpUI;               //HP进度条
	
	public UILabel finalScoreUI;        
	
	public UISlider energyUI;           //Energy进度条
	
	public UILabel scoreUI;             //分数UI

	public Spwaner spawaner;            //光晕生成器
	
	float score;                          //分数
	
	int combo;                          //连击   
	
	public Transform tracers;       //面板物体"Tracers"
	
	bool boosting = false;      //是否狂热

	public GameObject failUI;
	
	public GameObject feverUI;
	
	public GameObject easyTouchControlsCanvas;
	
	public GameObject[] WarnUI; //0:向左的UI，1：向右的UI
	
	public UISlider warnTimerBar; //提示时间条
	// Use this for initialization
	void Start () {
		interval = currentOffset / currentSpeed;    //克隆隧道的间隔时间等于隧道之间的间隔除以隧道的移动速度
		
		highlightChangeInterval = 150 / currentSpeed; 
		
		player = GameObject.FindGameObjectWithTag("Player");    //找到玩家飞船的GameObject
		
		GetCurrentTrack();  //找到玩家飞船所属的轨道
		
		ResetCombo();   //连击重置
		
		//随机偏移方向
		nextDir = Random.Range(0, 2) == 0 ? -1 : 1;

		Camera.main.GetComponent<Skybox> ().material = skyboxMats[Setting.index]; 
		airCrafts [Setting.planeIndex].SetActive (true);

		DataManager dm=DataManager.Instance;
		highlightIntervalQueue =  dm.beatList;

		//提示轨道偏移
		//        if (nextDir == -1)
		//        {
		//            DOTween.To(() => WarnUI[0].GetComponent<UISprite>().color, x => WarnUI[0].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 1), 0.1f).SetEase(Ease.Linear).SetDelay(1.9f).OnComplete(OnTimeStart);
		//            DOTween.To(() => WarnUI[0].GetComponent<UISprite>().color, x => WarnUI[0].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 0), dirInterval - 2).SetEase(Ease.Linear).SetDelay(2);
		//            
		//        }
		//        else
		//        {
		//            DOTween.To(() => WarnUI[1].GetComponent<UISprite>().color, x => WarnUI[1].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 1), 0.1f).SetEase(Ease.Linear).SetDelay(1.9f).OnComplete(OnTimeStart);
		//            DOTween.To(() => WarnUI[1].GetComponent<UISprite>().color, x => WarnUI[1].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 0), dirInterval - 2).SetEase(Ease.Linear).SetDelay(2);
		//        }
		
	}
	
	void OnTimeStart()
	{
		warnTimerBar.gameObject.SetActive(true);
		warnTimerBar.value = 1;
		DOTween.To(() => warnTimerBar.value, x => warnTimerBar.value = x, 0, dirInterval - 2).SetEase(Ease.Linear).OnComplete(OnTimeEnd);
	}
	void OnTimeEnd()
	{
		warnTimerBar.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		CheckHp ();

		CheckTrack();

		// gravity 
		player.transform.RotateAround(pivot.transform.position, Vector3.forward, Time.deltaTime * playerSpeed * Input.acceleration.x * 8);
		GetCurrentTrack();
		
		timer += Time.deltaTime;        //克隆隧道计时器 + 距上次Update函数到此次Update函数所流逝的时间
		dirTimer += Time.deltaTime;     //改变隧道扭曲方向的计时器 + 同时
		highlightTimer += Time.deltaTime; 
		scoreHighlightTimer += Time.deltaTime;


		GenerateEnviroment ();

		CheckFever ();

	}
	
	void LateUpdate()
	{
		UpdateHighlight ();
	}
	

	void GenerateEnviroment ()
	{
		switch (nextHighlight) {  //clone tunnel, elementProfab# as input
		case 0:
			CloneTunnel (elementPrefab0);
			break;
		case 1:
			CloneTunnel (elementPrefab1);
			break;
		case 2:
			CloneTunnel (elementPrefab2);
			break;
		case 3:
			CloneTunnel (elementPrefab3);
			break;
		case 4:
			CloneTunnel (elementPrefab4);
			break;
		case 5:
			CloneTunnel (elementPrefab5);
			break;
		case 6:
			CloneTunnel (elementPrefab6);
			break;
		case 7:
			CloneTunnel (elementPrefab7);
			break;
		case 8:
			CloneTunnel (elementPrefab8);
			break;
		case 9:
			CloneTunnel (elementPrefab9);
			break;
		default:
			print ("Incorrect elementPrefab input.");
			break;
		}

	}


	void CheckHp ()
	{
		if (hpUI.value <= 0  || highlightIntervalQueue.Count==1)
		{
			failUI.SetActive(true);
			finalScoreUI.text = ((int)score).ToString();
			Camera.main.transform.parent = null;
			player.SetActive(false);
			easyTouchControlsCanvas.SetActive(false);
			spawaner.gameObject.SetActive(false);
			hpUI.gameObject.SetActive(false);
			scoreUI.gameObject.SetActive(false);
			energyUI.gameObject.SetActive(false);
			comboUI.GetComponent<UILabel>().color = new Color(1, 125/255f, 0, 0);
			
			ParseObject testObject = new ParseObject("Score");
			testObject["score"] = score;
			testObject.SaveAsync();
			
			return;
		}
	}

	void CheckFever()
	{
		//检查能量条是否满足狂热
		if (energyUI.value == 1 && !boosting)
		{
			feverUI.SetActive(true);
			boosting = true;
			DOTween.To(() => energyUI.value, x => energyUI.value = x, 0, 8).SetEase(Ease.Linear).OnComplete(EndBoosting);
			DOTween.To(() => energyUI.GetComponent<UISprite>().color, x => energyUI.GetComponent<UISprite>().color = x, new Color(1, 1, 1, 0.5f), 0.5f).SetLoops(16, LoopType.Yoyo).SetEase(Ease.Linear);
			currentSpeed *= 3;
			GameObject[] elements1 = GameObject.FindGameObjectsWithTag("Element");
			foreach (GameObject e in elements1)
			{
				e.GetComponent<ElementMovement>().speed = currentSpeed;
			}
			GameObject[] elements2 = GameObject.FindGameObjectsWithTag("Collection");
			foreach (GameObject e in elements2)
			{
				e.GetComponent<ElementMovement>().speed = -currentSpeed;
			}
			
			interval = currentOffset / currentSpeed;
			dirInterval /= 2;
			//spawaner.interval /= 2;
			playerSpeed *= 2f;
		}
	}



	void CloneTunnel (GameObject element)
	{
		
		if (timer > interval)   //当克隆隧道计时器大于克隆隧道的间隔时间，则克隆一个隧道
		{
			current = Instantiate(element) as GameObject; //克隆体保存在current此变量里
			current.transform.position = new Vector3(53, -8.1f, 275);//将新隧道放在 （53，-8.1f,275)这个位置
			current.GetComponent<ElementMovement>().speed = currentSpeed;
			timer = 0;      //计时器复位
		}
		
		if (dirTimer > dirInterval) //同上，改变扭曲方向
		{
			dirTimer = 0;
			ChangeDir(dirInterval-0.5f, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-100, 100)));//随机在3个轴上进行扭曲 x:-10-10,y:-10-10,z:-100-100
		}
		
		if (highlightTimer > highlightIntervalQueue.Peek()) // Change highlight
		{
			highlightIntervalQueue.Dequeue();
			highlightTimer = 0;
			System.Random rd = new System.Random();
			nextDir = rd.Next(0,5)-2;
			//nextDir = Random.Range(0, 2) == 0 ? -1 : 1;
			//Debug.Log (nextDir);
			
			nextHighlight = (nextHighlight - nextDir);
			if (nextHighlight > 9) nextHighlight = 0;
			else if (nextHighlight < 0) nextHighlight = 9;
			
			highlight.Enqueue(nextHighlight);
			scoreHighlightTimer = 0; 
			//ChangeHighlight(nextHighlight-nextDir);   //同时改变轨道的高亮
		}
		
		if (scoreHighlightTimer > highlightChangeInterval)
		{
			scoreHighlightTimer = -10000f;
			currentHighlight = (int)highlight.Dequeue();
			
			
		}
		
		
	}
	
	
	//终止狂热
	void EndBoosting()
	{
		feverUI.SetActive (false);
		boosting = false;
		currentSpeed /= 3;
		dirInterval *= 2;
		//spawaner.interval *= 2;
		playerSpeed /= 2f;
		interval = currentOffset / currentSpeed;

		GameObject[] elements = GameObject.FindGameObjectsWithTag("Element");
		foreach (GameObject e in elements)
		{
			e.GetComponent<ElementMovement>().speed = currentSpeed;
		}

		GameObject[] elements2 = GameObject.FindGameObjectsWithTag("Collection");
		foreach (GameObject e in elements2)
		{
			e.GetComponent<ElementMovement>().speed = -currentSpeed;
		}

		hpUI.value += 0.2f;
	} 
	
	//检查是否在高亮轨道
	void CheckTrack()
	{   
		if(  (currentHighlight + 1)%10 == currentTrack || (currentHighlight - 1) == currentTrack  
		   || currentHighlight  == currentTrack ||(currentHighlight==0 && currentTrack==9)) // -1 mod 10 != 9   ????
		{
			hpUI.value += (1 + Time.deltaTime *combo * 0.1f) * Time.deltaTime * 0.2f;  //增加HP
			score += Time.deltaTime * (10 + combo / 10);
			scoreUI.text = ((int)score).ToString();     //加分
		} 
		else if(!boosting)
		{
			hpUI.value -= Time.deltaTime * Time.deltaTime * 10f;  //HP减少
		}
	}
	
	
	
	void UpdateHighlight()
	{
		if (current)
		{
			current.GetComponent<MeshRenderer>().material = polygonMat; //current的材质必须在LateUpdate里更新，此乃Porbuilder插件的BUG
			
			//current.transform.FindChild(nextHighlight.ToString()).GetComponent<MeshRenderer>().material = highlightMat;  //新隧道要找到nextHighlight索引的轨道，使之高亮
			
			//current.transform.FindChild( ((nextHighlight+1)%10).ToString()).GetComponent<MeshRenderer>().material = highlightMat;
		}
	}
	
	public void OnJoyStickMove(Vector2 stickPos)
	{
		player.transform.RotateAround(pivot.transform.position, Vector3.forward, Time.deltaTime * playerSpeed * stickPos.x);//当移动虚拟摇杆，则玩家飞船绕着中心点旋转
		GetCurrentTrack();  //计算最近的轨道
	}
	
	void ChangeDir(float sec, Vector3 dir)
	{
		//DOTween.Rewind();//dotween里所有的缓动都停止
		DOTween.To(() => curveCtr._V_CW_X_Bend_Size_GLOBAL, x => curveCtr._V_CW_X_Bend_Size_GLOBAL = x, dir.x, sec);//隧道在x轴上的扭曲
		DOTween.To(() => curveCtr._V_CW_Y_Bend_Size_GLOBAL, x => curveCtr._V_CW_Y_Bend_Size_GLOBAL = x, dir.y, sec);//y
		DOTween.To(() => curveCtr._V_CW_Z_Bend_Size_GLOBAL, x => curveCtr._V_CW_Z_Bend_Size_GLOBAL = x, dir.z, sec);//z
	}
	
	//idx取值范围为0-9，对应隧道上的10条轨道
	void ChangeHighlight(int idx)
	{
		if (idx > 9) idx = 0;
		else if (idx < 0) idx = 9;
		//隧道的tag是Element,此语句是找到当前时间里的所有隧道
		GameObject[] elements = GameObject.FindGameObjectsWithTag("Element");
		
		foreach (GameObject e in elements)
		{
			//把当前的高亮轨道复原
			e.transform.FindChild(nextHighlight.ToString()).GetComponent<MeshRenderer>().material = originalMat[0];
			e.transform.FindChild( ((nextHighlight+1)%10).ToString()).GetComponent<MeshRenderer>().material = originalMat[1];
		}
		
		//高亮轨道的索引变成idx
		nextHighlight = idx;
		
		foreach (GameObject e in elements)
		{
			//保存nextHighlight对应的轨道的材质
			originalMat[0] = e.transform.FindChild(nextHighlight.ToString()).GetComponent<MeshRenderer>().material;
			originalMat[1] = e.transform.FindChild(((nextHighlight + 1) % 10).ToString()).GetComponent<MeshRenderer>().material;
			//高亮nextHighlight对应的隧道
			e.transform.FindChild(nextHighlight.ToString()).GetComponent<MeshRenderer>().material = highlightMat;
			e.transform.FindChild(((nextHighlight + 1) % 10).ToString()).GetComponent<MeshRenderer>().material = highlightMat;
		}
		
		//随机偏移方向
		nextDir = Random.Range(0, 2) == 0 ? -1 : 1;
		
		//提示轨道偏移
		if (nextDir == -1)
		{
			DOTween.To(() => WarnUI[0].GetComponent<UISprite>().color, x => WarnUI[0].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 1), 0.1f).SetEase(Ease.Linear).SetDelay(1.9f).OnComplete(OnTimeStart);
			DOTween.To(() => WarnUI[0].GetComponent<UISprite>().color, x => WarnUI[0].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 0), dirInterval - 2).SetEase(Ease.Linear).SetDelay(2);
			
		}
		else
		{
			DOTween.To(() => WarnUI[1].GetComponent<UISprite>().color, x => WarnUI[1].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 1), 0.1f).SetEase(Ease.Linear).SetDelay(1.9f).OnComplete(OnTimeStart);
			DOTween.To(() => WarnUI[1].GetComponent<UISprite>().color, x => WarnUI[1].GetComponent<UISprite>().color = x, new Color(1, 0, 0, 0), dirInterval - 2).SetEase(Ease.Linear).SetDelay(2);
		}
	}
	
	
	
	//用来保存收集物，即光晕
	GameObject currentCollection;
	
	void OnTriggerEnter(Collider other)
	{
		if (other.collider.gameObject.tag != "Collection") return;//当触发器进入的物体不是光晕则返回
		currentCollection = other.gameObject;   //保存当前光晕
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.collider.gameObject.tag != "Collection") return;   //同上
		currentCollection = null;               //当前光晕设为空
	}
	
	//按下右下角的按钮则调用此函数
	public void OnPress()
	{
		if (currentCollection == null) return;
		Destroy(currentCollection); //销毁光晕
		
		hpUI.value += (1 + combo * 0.1f) * 0.2f;  //增加HP
		score += 10 + combo * combo / 10;
		scoreUI.text = ((int)score).ToString();     //加分
		
		GameObject go = Instantiate(disappearFx) as GameObject;// 生成一个光晕销毁后的粒子特效
		go.transform.parent = this.transform;    //移动特效到飞机上
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = new Vector3(-90, 0, 0);//调整特效角度
		Destroy(go, 1);//1秒钟后销毁此特效
		OnComboAdd();
	}
	
	//连击复位
	public void ResetCombo()
	{
		combo = 0;
		comboUI.GetComponent<UILabel>().text = combo.ToString();
		comboUI.GetComponent<UILabel>().color = new Color(1, 125/255f, 0, 0);
	}
	
	
	//增加连击
	public void OnComboAdd()
	{
		//DOTween.Rewind();
		combo++;
		comboUI.GetComponent<UILabel>().text = combo.ToString();
		comboUI.GetComponent<UILabel>().color = new Color(1, 125/255f, 0, 1);
		DOTween.To(() => comboUI.GetComponent<UILabel>().fontSize, x => comboUI.GetComponent<UILabel>().fontSize = x, 70, 0.2f).SetEase(Ease.Linear);
		DOTween.To(() => comboUI.GetComponent<UILabel>().fontSize, x => comboUI.GetComponent<UILabel>().fontSize = x, 30, 0.1f).SetDelay(0.2f).SetEase(Ease.Linear);
		UISprite bgUI = comboUI.transform.FindChild("BG").FindChild("BG").GetComponent<UISprite>();
		DOTween.To(() => bgUI.GetComponent<UISprite>().color, x => bgUI.GetComponent<UISprite>().color = x, new Color(1, 1, 1, 1), 0.2f).SetEase(Ease.Linear);
		DOTween.To(() => bgUI.GetComponent<UISprite>().color, x => bgUI.GetComponent<UISprite>().color = x, new Color(1, 1, 1, 100 / 255f), 0.1f).SetDelay(0.2f).SetEase(Ease.Linear);
		
		hpUI.value += combo / 1000;
		
		energyUI.value += (1f + 2f / combo)/10f;
		
		
		score += (100 + combo)*(boosting ? 2 : 1);
		
		scoreUI.text = ((int)score).ToString();
	}
	
	
	
	//获得离玩家最近的那条轨道
	void GetCurrentTrack()
	{
		float minDis = 1000;
		float dis;
		Transform minTrack = null ;
		foreach (Transform t in tracers)
		{
			dis = Vector3.Distance(t.position,this.transform.position);
			if (dis < minDis)
			{
				minTrack = t;
				minDis = dis;
			}
		}
		
		currentTrack = System.Int32.Parse(minTrack.gameObject.name);
	}
}
