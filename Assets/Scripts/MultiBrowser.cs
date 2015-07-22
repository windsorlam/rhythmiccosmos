using UnityEngine;
using System.Collections;

public class MultiBrowser : MonoBehaviour {
	public static string serviceIP;
	public static int servicePort;
	public static string serviceName;

	[SerializeField] myConnection m_myConnection;
	
	private bool browsing;

	public void StartLookup (string serviceType){
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
			Multi.StartLookup(serviceType, "local");
			browsing = true;
		} else if(Debug.isDebugBuild){
			Debug.Log ("Cannot start Multi browser: device is not connected to a network. @multiBrowser");
		}
	}
	
	public void StopLookup(){
		if (browsing) {
			Multi.StopLookup ();
			browsing = false;
		}
	}
	
	public void ResolveService(string serviceName){
		Multi.ResolveService (serviceName);
		if (Debug.isDebugBuild) {
			Debug.Log("Attempting to reslove service: [" + serviceName + "]. @multiBrowser");
		}
	}

	public void GetServiceIP(string serviceName){
		serviceIP = Multi.GetServiceIP(serviceName);
	}

	public void GetServicePort(string serviceName){
		servicePort = Multi.GetServicePort(serviceName);
	}
	
	public bool ConnectToResolvedService(string serviceName){
		bool success = false;
		GetServiceIP(serviceName);
		GetServicePort(serviceName);
		NetworkConnectionError error = Network.Connect(serviceIP, servicePort);
		switch(error){
		case NetworkConnectionError.NoError:
			Debug.Log("----> MultiBrowser.ConnectToResolvedService called. Connect server successfully. @multiBrowser");
			success = true;
			break;
		default:
			Debug.Log("----> MultiBrowser.ConnectToResolvedService called. Client Connecting Status:" + error + ". @multiBrowser");
			success = false;
			break;
		}
		return success;
	}

	public void DidShutDown (){
		StopLookup ();
		browsing = false;
		serviceName = "";
	}

	////------- callback function. Methods called from native code by UnitySendMessage 
	public void BrowserWillSearch(string emptyString){
		if(Debug.isDebugBuild){
			Debug.Log("Browsing for services has started");
		}
		browsing = true;
		
		Debug.Log("----> Browser will search. @multiBrowser");
		
		m_myConnection.OnBrowsingStarted (true);
	}

	public void BrowserDidNotSearch(string errorCode){
		if(Debug.isDebugBuild){
			Debug.LogError("Cannot browse services : " + MultiManager.errors[errorCode] + ". @multiBrowser");
		}
		browsing = false;

		m_myConnection.OnBrowsingFailed();
	}

	public void BrowserDidStop(string emptyString){
		if(Debug.isDebugBuild){
			Debug.Log("----> Browsing for services has stopped. @multiBrowser");
		}
		browsing = false;
		
		m_myConnection.OnBrowsingStopped();
	}

	public void FoundService(string foundserviceName){
		if(Debug.isDebugBuild){
			Debug.Log("Multi service ["+foundserviceName+"] was found. @multiBrowser");
		}
		serviceName = foundserviceName;

		m_myConnection.OnServiceFound(foundserviceName);
	}

	public void DidResolveAddress(string emptyString){
		if(Debug.isDebugBuild){
			Debug.Log("Service resolved successfully, retrieving IPs and port. @multiBrowser");
		}

		m_myConnection.OnServiceResolved ();
	}
	
	public void FailedToResolveAddress(string errorCode){
		if(Debug.isDebugBuild){
			Debug.LogError("Cannot resolve service : " + MultiManager.errors[errorCode] + ". @multiBrowser");
		}
		
		m_myConnection.OnResolvingFailed();
	}

	public void LostService(string servicename){
		if(Debug.isDebugBuild){
			Debug.Log("Multi service [" + servicename + "] was lost. @multiBrowser");
		}
		serviceName = "";

		m_myConnection.OnServiceLost(servicename);
	}
	
	//Accessors if needed
	public static string ReturnService(){
		string serviceCopy;
		serviceCopy = serviceName;
		return serviceCopy;
	}
	
	public bool IsBrowsing(){
		return browsing;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
