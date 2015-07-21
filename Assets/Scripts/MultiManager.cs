using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiManager : MonoBehaviour {
	//to accept gameobject from outside
	public MultiBrowser browser;
	public MultiPublisher publisher;

	//make sure only one copy in memory
	public static MultiBrowser multiBrowser;
	public static MultiPublisher multiPublisher;

	private bool dontDestroyOnLoad = true;

	public static Dictionary<string, string> errors = new Dictionary<string, string>(8);

	void Awake(){
		if (dontDestroyOnLoad) {
			//Makes the object target not be destroyed automatically when loading a new scene.
			DontDestroyOnLoad (this);
		}

		multiBrowser = browser;
		multiPublisher = publisher;

		errors.Clear ();
		InitErrorDictionary (); //for what??
	}

	////----- MultiBrowser
	public static void StartLookup (string serviceType){
		multiBrowser.StartLookup(serviceType);
	}

	public static void StopLookup(){
		multiBrowser.StopLookup ();
	}

	public static void ResolveService(string serviceName){
		multiBrowser.ResolveService (serviceName);
	}

	public static bool ConnectToResolvedService(string serviceName){
		return multiBrowser.ConnectToResolvedService (serviceName);
	}

	////----- MultiPublisher
	public static void PublishService(string serviceName, string serviceType, int ListenPort) {
		//overload if more control is needed
		multiPublisher.PublishService(serviceName, serviceType, ListenPort);
	}

	public static void StopPublishService() {
		multiPublisher.StopPublishService();
	}

	//public static string GetServiceIP(string serviceName){
	//	string IP = Multi.GetServiceIP (serviceName);
	//	return IP;
	//}

	//public static int GetServicePort(string serviceName){
	//	int port = Multi.GetServicePort (serviceName);
	//	return port;
	//}

	//for unity connection test
	public static string initializethisdevice(string serviceName){
		NetworkConnectionError error = Network.InitializeServer(3, 10000, false);
		switch(error){
		case NetworkConnectionError.NoError:
			Debug.Log("initialize a server successfully. (@multiManager)");
			break;
		default:
			Debug.Log("Faile to initialize a server. Error: " + error + ". (@multiManager)");
			break;
		}
		return Network.player.ipAddress;
	}
	
	public static void ShutDown(){
		Multi.ShutDown ();  
		multiBrowser.DidShutDown ();
		multiPublisher.DidShutDown ();
	}

	void OnApplicationQuit(){
		ShutDown();
	}
	
	void InitErrorDictionary (){
		errors.Add("-72000","unknown error");
		errors.Add("-72001","service name already exists");
		errors.Add("-72002","service not found");
		errors.Add("-72003","service could not process the request");
		errors.Add("-72004","invalid argument when creating NSNetService object");
		errors.Add("-72005","client cancelled the action");
		errors.Add("-72006","net service improperly configured");
		errors.Add("-72007","net service has timed out");
	}

	void Start () {
	}

	void Update () {	
	}
}
