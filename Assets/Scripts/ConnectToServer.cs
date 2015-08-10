﻿using UnityEngine;
using System.Collections;
using HTTP;

public class ConnectToServer : MonoBehaviour {
	
	static WebSocket m_websocket;

	private static bool networkReady;
	private static bool opponentNetworkReady;

	private static bool playReady;
	private static bool opponentPlayReady;

	private static bool networkFail;
	private static bool opponentNetworkFail;

	private static bool musicSet;
	private static bool opponentMusicSet;

	private MultiMainLogic _mainLogic;

	GameObject playButton;
	GameObject selectButton;
	GameObject musiclist;
	private string music;
	DataManager dm;
	private bool musicSelected;

	// Use this for initialization
	void Awake(){
		Debug.Log ("----> [Awake] Start to connect ... ");
		m_websocket = new WebSocket ();

		m_websocket.OnConnect += HandleOnConnect;
		m_websocket.OnDisconnect += HandleOnDisconnect;
		m_websocket.OnTextMessageRecv += HandleOnTextMessageRecv;

		m_websocket.Connect ("https://shared.staging.mossapi.com/connect");

		playButton = GameObject.Find("MultiPlayStart");
		selectButton = GameObject.Find ("MultiSelectMusic");
		musiclist = GameObject.Find ("MusicList");
		playButton.SetActive (false);

		dm = DataManager.Instance;

		DontDestroyOnLoad (this);
	}

	void OnDestroy(){
		//unlock the delegate
		m_websocket.OnConnect -= HandleOnConnect;
		m_websocket.OnDisconnect -= HandleOnDisconnect;
		m_websocket.OnTextMessageRecv -= HandleOnTextMessageRecv;
	}

	void HandleOnTextMessageRecv (string mes)
	{
		Debug.Log("----> Message received");
		JSONObject message = new JSONObject (mes);
		//Debug.Log ("MessageRecv: " + message);
		string mm = message["header"].str;
		string mmm = message["header"].ToString ();

		switch (message["header"].str) {
		case "Connected":
			opponentNetworkReady = true;
			if(networkReady){
				selectButton.SetActive (true);
			}
			break;
		case "playReady":
			opponentPlayReady = true;
			if(playReady){
				Application.LoadLevel("MultiSpace");
			}
			break;
		case "move":
			string _playerName = message["playerName"].str;
			float _tunnelOffset = (float)message["tunnelOffset"].n;
			bool _boosting = message["boosting"].b; 
			float _energy = (float)message["energy"].n; 
			float _hp = (float)message["hp"].n;
			float _score = (float)message["score"].n;
			_mainLogic.ProccessMoveCommunication(_playerName, _tunnelOffset, _boosting, _energy, _hp, _score);
			break;
		case "Disconnected":
			opponentNetworkFail = true;
			break;
		case "setMusic":
			opponentMusicSet = true;
			string music = message["Music"].str;
			SetMusic(music);
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
		JSONObject obj = new JSONObject ();

		obj.AddField ("header", "Connected");
		m_websocket.Send(obj.ToString());

		if (opponentNetworkReady) {
			selectButton.SetActive (true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (dm.musicPath != null && !musicSelected) {
			SetMusic(dm.musicPath);
			musicSelected = true;
		}
	}

	public static bool isGameReady(){
		return playReady && opponentPlayReady;
	}

	public static bool isNetworkFail(){
		return networkFail && opponentNetworkFail;
	}
	
	public void testFun(){
		JSONObject obj = new JSONObject(JSONObject.Type.OBJECT);
		
		obj.AddField ("header", "test");
		obj.AddField ("test", "for send test");

		m_websocket.Send(obj.ToString());
	}

	public void OnSelectMusic(){
		musiclist.gameObject.SetActive (true);
	}

	public void SetMusic(string music){
		//set music with the id
		if (!musicSet) {
			musicSet = true;
			dm.musicPath = music;

			JSONObject obj = new JSONObject ();
			obj.AddField ("header", "setMusic");
			obj.AddField ("Music", music);
			m_websocket.Send (obj.ToString());
		}
		if (opponentMusicSet) {
			playButton.SetActive(true);
		}
	}

	public void OnPlayReady(){
		playReady = true;

		JSONObject obj = new JSONObject();
		
		obj.AddField ("header", "playReady");

		m_websocket.Send (obj.ToString());

		if (opponentPlayReady) {
			Application.LoadLevel("MultiSpace");
		}
	}

	//SendMoveCommunication(playerName, tunnelOffset, boosting, energy, hp, score);
	//string _playerName, float _tunnelOffset, bool _boosting, float _energy, float _hp, float _score)
	public static void SendMoveInfo(string playerName, float tunnelOffset, bool boosting, float energy, float hp, float score){
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

}