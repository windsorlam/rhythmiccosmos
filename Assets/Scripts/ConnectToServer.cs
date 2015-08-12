using UnityEngine;
using System.Collections;
using HTTP;

public class ConnectToServer : MonoBehaviour {
	
	static WebSocket m_websocket;

	private static bool networkReady;
	private static bool opponentNetworkReady;

	private static bool playReady;
	private static bool opponentPlayReady;

	private static bool sceneLoaded;
	private static bool opponentSceneLoaded;

	private static bool networkFail;
	private static bool opponentNetworkFail;

	private static bool musicReady;
	private static bool opponentMusicReady;
	
	private MultiMainLogic _mainLogic;

	string role;

	GameObject testButton;
	GameObject playButton;
	GameObject selectButton;
	GameObject waitingLabel;
	GameObject musiclist;
	GameObject slider;
	DataManager dm;
	private bool musicSelected;
	public static bool analysisFinished;

	// Use this for initialization
	void Awake(){
		Debug.Log ("----> [Awake] Start to connect ... ");
		m_websocket = new WebSocket ();

		m_websocket.OnConnect += HandleOnConnect;
		m_websocket.OnDisconnect += HandleOnDisconnect;
		m_websocket.OnTextMessageRecv += HandleOnTextMessageRecv;

		m_websocket.Connect ("https://shared.staging.mossapi.com/connect");

		testButton = GameObject.Find ("sendtest");
		playButton = GameObject.Find("MultiPlayStart");
		selectButton = GameObject.Find ("MultiSelectMusic");
		slider = GameObject.Find ("Slider");
		musiclist = GameObject.Find ("MusicList");
		waitingLabel = GameObject.Find ("WaitingMusic");

		waitingLabel.SetActive (false);
		testButton.SetActive (false);
		selectButton.SetActive (false);
		playButton.SetActive (false);
		musiclist.SetActive (false);

		dm = DataManager.Instance;

		DontDestroyOnLoad (this);
	}

	public void SetSceneLoaded(MultiMainLogic logic){
		sceneLoaded = true;
		_mainLogic = logic;

		JSONObject obj = new JSONObject ();
		obj.AddField("header", "sceneLoaded");
		m_websocket.Send (obj.ToString());
	}

	void OnDestroy(){
		//unlock the delegate
		m_websocket.OnConnect -= HandleOnConnect;
		m_websocket.OnDisconnect -= HandleOnDisconnect;
		m_websocket.OnTextMessageRecv -= HandleOnTextMessageRecv;
	}

	// Update is called once per frame
	void Update () {
		if (dm.musicPath != null && !musicSelected) {
			SendMusicPath(dm.musicPath);
			musicSelected = true;
		}
		if ( musicSelected && analysisFinished ) {
			sendMusicReady();
			analysisFinished = false; // only send once
		}
	}

	void HandleOnTextMessageRecv (string mes)
	{
		Debug.Log ("----> Message received");
		JSONObject message = new JSONObject (mes);

		switch (message ["header"].str) {
		case "client":
			role = "client";
			break;
		case "server":
			role = "server";
			break;
		case "Connected":
			opponentNetworkReady = true;
			Debug.Log("ooooooo connected");

			Debug.Log("role:" + role);
			if (networkReady && role.Equals("server")) {
				selectButton.SetActive (true);
			}else if(networkReady && role.Equals("client")){
				waitingLabel.SetActive (true);
			}
			break;
		case "musicPath":
			//receive selected music from opponent
			string musicPath = message ["path"].str;
			OnMusicSelected (musicPath);
			break;
		case "musicReady":
			OnMusicReady ();
			break;
		case "playReady":
			opponentPlayReady = true;
			if (playReady) {
				playButton.SetActive(true);
				//Application.LoadLevel ("MultiSpace");
			}
			break;
		case "sceneLoaded":
			opponentSceneLoaded = true;
			break;
		case "move":
			string _playerName = message ["playerName"].str;
			float _tunnelOffset = (float)message ["tunnelOffset"].n;
			bool _boosting = message ["boosting"].b; 
			float _energy = (float)message ["energy"].n; 
			float _hp = (float)message ["hp"].n;
			float _score = (float)message ["score"].n;
			_mainLogic.ProccessMoveCommunication (_playerName, _tunnelOffset, _boosting, _energy, _hp, _score);
			break;
		case "fail":
			float _opScore = (float)message["score"].n;
			_mainLogic.ProccessFailUI(true, _opScore);
			break;
		case "Disconnected":
			opponentNetworkFail = true;
			break;
		
		}
	}

	void HandleOnDisconnect ()
	{
		Debug.Log ("----> Network Failed");
		networkFail = true;

		JSONObject obj = new JSONObject ();
		
		obj.AddField ("header", "Disconnected");
		m_websocket.Send(obj.ToString());
	}

	void HandleOnConnect ()
	{
		networkReady = true;
		Debug.Log ("Network Connected");
	}

	public bool isGameReady(){
		return sceneLoaded && opponentSceneLoaded;
	}

	public bool isNetworkFail(){
		return networkFail && opponentNetworkFail;
	}

	//click select music button
	public void OnSelectMusicClick(){
		musiclist.SetActive (true);
	}

	//send selected music to opponent
	public void SendMusicPath(string musicPath){
		JSONObject obj = new JSONObject ();
		obj.AddField ("header", "musicPath");
		obj.AddField ("path", musicPath);

		m_websocket.Send (obj.ToString());
	}

	//received selected music from opponent
	public void OnMusicSelected(string musicPath){
		musicSelected = true;
		waitingLabel.SetActive (false);

		//set music follow opponent
		DataManager dm = DataManager.Instance;
		dm.musicPath = musicPath;

		slider.SetActive (true);
	}

	public void sendMusicReady(){
		musicReady = true;

		JSONObject obj = new JSONObject ();
		obj.AddField ("header", "musicReady");
		m_websocket.Send (obj.ToString());

		if (opponentMusicReady) {
			playButton.SetActive(true);
		}
	}

	//receive music ready from opponent
	public void OnMusicReady(){
		opponentMusicReady = true;

		if (musicReady) {
			playButton.SetActive(true);
		}
	}

	public void SendPlayReady(){
		playReady = true;

		JSONObject obj = new JSONObject();		
		obj.AddField ("header", "playReady");
		m_websocket.Send (obj.ToString());
	
		if (opponentPlayReady) {
			Application.LoadLevel("MultiSpace");
		}
	}
	
	public void SendMoveInfo(string playerName, float tunnelOffset, bool boosting, float energy, float hp, float score){
		if (!opponentSceneLoaded)
			return;

		JSONObject move = new JSONObject ();
		move.Clear ();

		move.AddField ("header", "move");
		move.AddField ("playerName", playerName);
		move.AddField ("tunnelOffset", tunnelOffset);
		move.AddField ("boosting", boosting);
		move.AddField ("energy", energy);
		move.AddField ("hp", hp);
		move.AddField ("score", score);

		m_websocket.Send (move.ToString());
	}

	public void SendFailUI(bool failOP, float score){
		if (!opponentSceneLoaded)
			return;

		JSONObject obj = new JSONObject ();
		obj.AddField ("header", "fail");
		obj.AddField ("score", score);
		m_websocket.Send (obj.ToString());
	}

	public void testFun(){
		JSONObject obj = new JSONObject(JSONObject.Type.OBJECT);
		
		obj.AddField ("header", "test");
		obj.AddField ("test", "for send test");
		
		m_websocket.Send(obj.ToString());
	}

}
