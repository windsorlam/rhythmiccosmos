using UnityEngine;
using System.Collections;

public class RPCLogicHandler : MonoBehaviour
{
	private bool _ready;
	private bool _opponentReady;

	private bool _sceneLoaded;
	private bool _opponentSceneLoaded;

	private bool _networkFail;
	private bool _opponentNetworkFail;

	public bool _musicSet;
	public bool _opponentMusicSet;

	private MultiMainLogic _mainLogic;

	GameObject playButton;
	GameObject selectButton;
	private string music;

	void Awake()
	{
		playButton = GameObject.Find("MultiPlayStart");
		selectButton = GameObject.Find ("MultiSelectMusic");
		DontDestroyOnLoad (this);
	}

	void Update()
	{
		DataManager dm = DataManager.Instance;
		if (dm.musicPath != null) {
			SetMusic(dm.musicPath);
		}
	}

	void GoToGameScene() {
		Application.LoadLevel ("MultiSpace");
	}
	
	public bool isGameReadyToPlay() {
		return _sceneLoaded && _opponentSceneLoaded;
	}

	public void SetSceneLoaded(MultiMainLogic logic) {
		_mainLogic = logic;
		_sceneLoaded = true;
		networkView.RPC ("OnGameSceneLoaded", RPCMode.Others, null);
	}

	public void SelectMusic(){
		GameObject musiclist = GameObject.Find ("MusicList");
		musiclist.SetActive (true);
	}

	public void SetMusic(string music){
		//set music with the id
		myConnection.musicSet = true;
		_musicSet = true;

		DataManager dm = DataManager.Instance;
		dm.musicPath = music;
		networkView.RPC ("OnMusicReady", RPCMode.Others, music);
		if (_opponentMusicSet) {
			playButton.SetActive(true);
		}
	}

	public void SendReady()
	{
		//click the play button
		_ready = true;
		playButton.gameObject.SetActive (false);
		networkView.RPC ("OnGameReady", RPCMode.Others, null);
		if (_opponentReady) {
			// Go to game scene.
			GoToGameScene();
		}
	}
	
	public void SendProccessFailUI(bool failOP, float _score)
	{
		if (!_opponentSceneLoaded)
			return;
		networkView.RPC ("ProccessFailUI", RPCMode.Others, new object[]{failOP, _score});
	}

	public void SendNetworkFailUI(){
		if (!_opponentSceneLoaded)
			return;
		networkView.RPC ("ProccessNetworkFailUI", RPCMode.Others);
	}

	public void SendMoveCommunication(string _playerName, float _tunnelOffset, bool _boosting, float _energy, float _hp, float _score)
	{
		if (!_opponentSceneLoaded)
			return;
		networkView.RPC ("ProccessMoveCommunication", RPCMode.Others, new object[] {
			_playerName,
			_tunnelOffset,
			_boosting,
			_energy,
			_hp,
			_score
		});
	}

	[RPC]
	void OnGameReady()
	{
		_opponentReady = true;
		if (_ready) {
			// Go to game scene.
			GoToGameScene();
		}
	}

	[RPC]
	void OnMusicReady(string _music)
	{
		_opponentMusicSet = true;

		if (!_musicSet) {
			SetMusic (_music);
		}

		//set music analysis result file

		if(_musicSet){
			playButton.SetActive(true);
		}
	}

	[RPC]
	void OnGameSceneLoaded()
	{
		_opponentSceneLoaded = true;
	}

	[RPC]
	void ProccessFailUI(bool failOP, float _score)
	{
		if (_mainLogic != null) {
			_mainLogic.ProccessFailUI (failOP, _score);
		} else {
			Debug.Log("My scene has not loaded yet.");
		}
	}

	[RPC]
	void ProccessNetworkFailUI(){
		if (_mainLogic != null) {
			_mainLogic.ProccessNetworkFailUI ();
		} else {
			Debug.Log("My scene has not loaded yet.");
		}
	}

	[RPC]
	void ProccessMoveCommunication(string _playerName, float _tunnelOffset, bool _boosting, float _energy, float _hp, float _score)
	{
		if (_mainLogic != null) {
			_mainLogic.ProccessMoveCommunication (_playerName, _tunnelOffset, _boosting, _energy, _hp, _score);
		} else {
			Debug.Log("My scene has not loaded yet. ");
		}
	}
}
