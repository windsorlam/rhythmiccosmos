using UnityEngine;
using System.Collections;

public class myConnection : MonoBehaviour {
	////---- UI part variable *******************************************
	private GUIStyle labelStyle = new GUIStyle ();  //GUIStyle of label
	//private string statusLabel = "Waiting to start ... "; //display the connection status
	private string GUIStatus = "init"; //controler of the UI 

	////---- Publish/Browsing Service part variable *********************
	public int ListenPort = 10000;
	public string publishServiceName = "myService"; //name of published service
	public string publishServiceType = "_myService._tcp"; //type of published service
	public string browseServiceType = "_myService._tcp"; //type of browsing
	private string foundServiceName = "";//services have been found
	
	////---- All status variable ****************************************
	private bool broStatus = false; //the status of browsing -- yes/no
	private bool pubStatus = false; //the status of publish
	private bool found = false; //the status of wether found any service
	private bool justStop = false; //use when just want stop browsing, do not want to publish service

	////---- RPC part1, for communication *******************************
	//string sendMessage = "";
	//string recMessage = "";

	GameObject playButton;
	GameObject selectButton;
	GameObject waitingLabel;
	////---- RPC part2, for sync 2 fighters *****************************

	public bool dontDestroyOnLoad = true;
	public bool connected = false;
	public static bool musicSet = false;

	//click a button to start match players, only 2 players at the same time,
	//and the first one start become the server side, initialize the server and publish service
	//the latter start searching service, resolve service and connect to service
	//then they could communicate with each other.
	
	void Start () {
		//set label GUI
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;

		playButton = GameObject.Find("MultiPlayStart");
		selectButton = GameObject.Find ("MultiSelectMusic");
		waitingLabel = GameObject.Find ("WaitingMusic");
		selectButton.gameObject.SetActive (false);
		playButton.gameObject.SetActive (false);
		waitingLabel.SetActive (false);

		if (dontDestroyOnLoad) {
			//Makes the object target not be destroyed automatically when loading a new scene.
			DontDestroyOnLoad (this);
		}

		broStatus = false;
		StartMatch();

		//GUIStatus = "start";
		//GUIStatus = "testStart"; 

		//Reset message
		//ResetMessage ();
	}

	//reset all send and received message box
	//void ResetMessage(){
		//recMessage = "";
		//sendMessage = "";
	//}

    ////---- Part1: browsing, resolve and connect (as a client)
	void StartMatch(){
		switch (Network.peerType) {
		case NetworkPeerType.Disconnected:
			StartBrowsing();
			break;
		default:
			break;
		}
	}

	void StartBrowsing(){
		//start look up first depend on serviceType
		if (!broStatus) {
			broStatus = true;
			MultiManager.StartLookup (browseServiceType); 
			Debug.Log ("----> Start Look up.");
			//ResetMessage();

			StartCoroutine(WaitAndPrint(4.0f));
		}else {
			Debug.Log("----> Browsing service has been started.");
		}
	}

	void StopBrowsing(){
		if (broStatus) {
			broStatus = false;
			MultiManager.StopLookup ();
			Debug.Log ("----> Stop browsing.");
			
			//ResetMessage ();
		} else {
			Debug.Log("----> Browsing service has been stoped.");
		}

	}

	void StopConnection(){
		if (Network.peerType == NetworkPeerType.Disconnected) {
			Debug.Log("----> No connection exsiting.");
		} else {
			Network.Disconnect ();
			GUIStatus = "start";
			Debug.Log ("----> Stop Connection.");
			
			//ResetMessage();
		}
	}

	void OnClient(){
		//playButton.SetActive(true);
		if (!connected) {
			//GUIStatus = "start";
			found = false;
		}
		waitingLabel.SetActive (true);
	}

	void OnServer(){
		if (Network.connections.Length == 1) {
			if (!musicSet) {
				selectButton.SetActive (true);  //select Music
			}
		}
	}

	////----- Part2 Publish service (as a server)
	void PublishService(){
		if (!pubStatus) {
			pubStatus = true;
			MultiManager.PublishService (publishServiceName, publishServiceType, ListenPort);
			Debug.Log ("----> Start publishing");

			//ResetMessage ();
		} else {
			Debug.Log("----> Publish service has been started.");
		}
	}

	void StopPublish(){
		if (pubStatus) {
			pubStatus = false;
			MultiManager.StopPublishService ();
			Debug.Log ("----> Stop publishing.");
			
			//ResetMessage();
		} else {
			Debug.Log("----> Publish service has been stoped.");
		}

	}

	//receive method should declare [RPC]
	/*
	[RPC]
	void ReceiveMessage(string msg, NetworkMessageInfo info){
		recMessage = "Sender:" + info.sender + "Message:" + msg;
	}
	*/

	void Update(){
		switch (Network.peerType) {
		case NetworkPeerType.Disconnected:
			//GUIStatus = "start";
			//GUIStatus = "testStart";
			break;
		case NetworkPeerType.Server:
			//GUIStatus = "server";
			//GUIStatus = "testServer";
			OnServer();
			break;
		case NetworkPeerType.Client:
			//GUIStatus = "client";
			//GUIStatus = "testClient";
			OnClient();
			break;
		case NetworkPeerType.Connecting:
			//GUIStatus = "connecting";
			break;
		}
	}

	////************************* client part callback function ***********************************
	//method call by xcode through unitySendMessage
	public void OnBrowsingStarted(bool started){
		Debug.Log ("----> OnBrowsingStarted called");
		if (started) {
			broStatus = true;
			//statusLabel = "Start Browsing ... ";
			GUIStatus = "browsing";
			Debug.Log ("----> Browsing Started ... ");
			Debug.Log("=== GUIStatus: "+ GUIStatus);
		} else {
			Debug.Log ("---> Browsing Not Started yet ... ");
			//statusLabel = "Browsing haven't been started yet";
		}
	}
	
	//browsing could not initialize correctly
	public void OnBrowsingFailed(){
		broStatus = false;
		//statusLabel = "Can not start browsing ... ";
		Debug.Log ("---->OnBrowsingFailed called. Can not start browsing ... ");
	}
	
	public void OnServiceFound(string servicename){
		found = true;
		Debug.Log ("----> OnServiceFound called.");
		foundServiceName = servicename;
		//statusLabel = "Service: [" + foundServiceName + "] is found";
		StopBrowsing ();//stop browsing as soon as found a service
	}

	public void OnBrowsingStopped(){
		broStatus = false;
		Debug.Log ("---> OnBrowsingStopped called");
		//as the browsing has been stopped, then can start to publish a service
		if (justStop) {
			GUIStatus = "start";
			Debug.Log ("---> Stop browsing successfully with justStop. (OnBrowsingStopped)");
		}else if (found) {
			//when a service is founs, stop browsing and connect to the service
			//statusLabel = "Going to connect a  service ... ";
			Debug.Log ("---> Connect To Service (OnBrowsingStopped)");
			MultiManager.ResolveService(foundServiceName);
		} else {
			//when no service existing, publish a service
			//statusLabel = "Going to publish a server service ... ";
			Debug.Log ("----> Start to publish a service (OnBrowsingStopped)");
			PublishService();
		}
		found = false; //reset found to false
		justStop = false;
	}

	//When trying to connect, should resolve a service first to retrieve data.
	public void OnResolvingFailed(){
		//statusLabel = "Can not resolve service ... ";
		GUIStatus = "start";
		broStatus = false;
		found = false;
		Debug.Log ("----> OnResolvingFailed called.");
	}
	
	public void OnServiceResolved(){
		//statusLabel = "Connecting to a resolved service.";
		Debug.Log ("----> OnServiceResolved called, connecting to a resolved service");
		if (MultiManager.ConnectToResolvedService (foundServiceName)) {
			GUIStatus = "client";
			connected = true;
		}
	}

	public void OnServiceLost(string service){
		found = false;
		broStatus = false;
		GUIStatus = "start";
		Debug.Log ("----> OnServiceLost called.");
		//statusLabel = "service lost ... ";
		connected = false;
	}
	
	////************************* server part callback function ***********************************
	public void OnServiceWillPublish(){
		pubStatus = true;
		//statusLabel = "Service is going to publish";
		Debug.Log ("----> OnServiceWillPublish called.");
	}
	
	public void OnServicePublished(string serviceName){
		pubStatus = true;
		GUIStatus = "server";
		//statusLabel = "Service: [" + publishServiceName + "] has been publish successfully. Waiting for client ...";

		Debug.Log ("----> OnServicePublished called ============= ");
		Debug.Log ("ipAddress: " + Network.player.ipAddress);
		Debug.Log ("Port: " + Network.player.port);
		Debug.Log ("External Port: " + Network.player.externalPort);
		Debug.Log ("--------------------------------------------- ");
	}

	public void OnPublishFailed(){
		pubStatus = false;
		//statusLabel = "Publish failed. Going to stop publish servie.";
		StopPublish ();
		GUIStatus = "start";
		Debug.Log ("----> OnPublishFailed ... ");
	}

	public void OnServicePublishStopped(string serviceName){
		//statusLabel = "Service: [" + publishServiceName + "] has been stopped";
		Debug.Log ("----> OnServicePublishStopped called.");
		GUIStatus = "start";
		pubStatus = false;
	}

	IEnumerator WaitAndPrint(float waitTime){
		// pause execution for waitTime seconds
		Debug.Log ("----> coroutine Waiting ... ");
		yield return new WaitForSeconds(waitTime);
		if(!found && !connected){
			StopBrowsing();
			PublishService();
		}
	}

	/*
	//define the GUI
	void OnGUI(){
		//indicate the status of connection: searching -> waiting -> done -> stop

		//label display connection status
		GUILayout.Label (statusLabel, labelStyle);

		//main UI depend on value of GUIStatus
		switch (GUIStatus) {
		case "start":
			ResetMessage(); //reset all message each time before establish a connection

			if(GUILayout.Button("Start") && !broStatus){
				//start to match a service for player depends on service type and name
				StartMatch(); 
			}
			break;
		case "client":
			GUILayout.Label("Connected server: ["+ foundServiceName + "]");
			if (GUILayout.Button ("Disconnect") && Network.peerType == NetworkPeerType.Client) {
				StopConnection();
			}

			//send and receive message from connected peer through unity RPC
			GUILayout.Box(recMessage);	
			sendMessage = GUILayout.TextArea (sendMessage);
			if (GUILayout.Button ("Send")) {
				networkView.RPC ("ReceiveMessage", RPCMode.All, sendMessage);
			}
			break;
		case "server":
			GUILayout.Label("Server is running: ["+ publishServiceName + "]");
			if (GUILayout.Button ("Stop Service") && Network.peerType == NetworkPeerType.Server) {
				Debug.Log("-----> stop service. onserver");
				StopPublish();
			}
			GUILayout.Label("Our Clients:");
			int length = Network.connections.Length; 
			for(int i=0; i<length; i++) {
				GUILayout.Label ("Client" + i);
				GUILayout.Label ("Client IP" + Network.connections [i].ipAddress);  
				GUILayout.Label ("Client Port" + Network.connections [i].port);
				GUILayout.Label ("------------------------------------------------");
			}
	
			//send and receive message from connected peer through unity RPC
			GUILayout.Box(recMessage);
			sendMessage = GUILayout.TextArea (sendMessage);
			if (GUILayout.Button ("Send")) {
				networkView.RPC ("ReceiveMessage", RPCMode.All, sendMessage);
			}
			break;
		case "connecting":
			GUILayout.Label("Connectiong, please waiting ...");
			break;
		case "browsing":
			if( GUILayout.Button ("Stop Browsing") ){
				justStop = true;
				StopBrowsing();
				Debug.Log("----> GUIStatus: browsing ... ");
			}
			break;
		//========= for unity connection test ============================================
		case "testStart":
			string thisIP = Network.player.ipAddress;
			int thisPort = 10000;
			if (GUILayout.Button ("Start web server")) {
				thisIP = MultiManager.initializethisdevice(publishServiceName);
			}
			if (GUILayout.Button ("Start web client")) {
				NetworkConnectionError error = Network.Connect (thisIP, thisPort);
				switch(error){
				case NetworkConnectionError.NoError:
					Debug.Log("connect server successfully");
					break;
				default:
					Debug.Log("Client Connecting Status:"+error);
					break;
				}
			}
			break;
		case "testClient":
			GUILayout.Label("ip: " + Network.player.ipAddress);
			GUILayout.Label("port: " + Network.player.port);
			GUILayout.Label("external port: " + Network.player.externalPort);
			playButton.SetActive(true);
			break;
		case "testServer":
			GUILayout.Label("ip: " + Network.player.ipAddress);
			GUILayout.Label("port: " + Network.player.port);
			GUILayout.Label("external port: " + Network.player.externalPort);
			int len = Network.connections.Length; 
			GUILayout.Label("Our Clients:");
			for(int i=0; i<len; i++) {
				GUILayout.Label ("Client" + i);
				GUILayout.Label ("Client IP" + Network.connections [i].ipAddress);  
				GUILayout.Label ("Client Port" + Network.connections [i].port);
				GUILayout.Label ("----------------------------------------------");
			}
			if( len > 0 ){
				playButton.SetActive(true);
			}
			break;
		//================================================================================
		}
	}
	*/
}