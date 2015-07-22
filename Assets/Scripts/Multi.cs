using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Multi {
	// interface to native code (implementation) 
	[DllImport ("__Internal")]
	private static extern void _StartLookup (string serviceType, string domain);
	
	[DllImport ("__Internal")]
	private static extern void _StopLookup ();

	//Resolving
	[DllImport ("__Internal")]
	private static extern void _ResolveService (string serviceName);
	
	[DllImport ("__Internal")]
	private static extern string _GetServiceAddress (string serviceName);
	
	[DllImport ("__Internal")]
	private static extern int _GetServicePort (string serviceName);

	//Publication
	[DllImport ("__Internal")]
	private static extern void _PublishService (string serviceType, string serviceName, int port);
	
	[DllImport ("__Internal")]
	private static extern void _StopPublishService ();
	  
	[DllImport ("__Internal")]
	private static extern void _ShutDown ();

	public static void StartLookup(string serviceType, string domain){
		//Multi.StartLookup(serviceType, "local");
		if (Application.platform != RuntimePlatform.OSXEditor)
			_StartLookup(serviceType, domain);
	}

	//Multi.StopLookup ();
	public static void StopLookup(){
		if (Application.platform != RuntimePlatform.OSXEditor)
			_StopLookup();
	}

	//Multi.ResolveService (serviceName);
	public static void ResolveService(string serviceName){
		if (Application.platform != RuntimePlatform.OSXEditor)
			_ResolveService(serviceName);
	}
	
	//Multi.GetServiceIP(serviceName);
	public static string GetServiceIP(string serviceName){
		if (Application.platform != RuntimePlatform.OSXEditor){
			string IP = _GetServiceAddress(serviceName);
			return IP;
		}
		else return "No address get. @multi";
	}
	
	//Multi.GetServicePort(serviceName);
	public static int GetServicePort(string serviceName){
		if (Application.platform != RuntimePlatform.OSXEditor){
			int Port = _GetServicePort(serviceName);
			Debug.Log("----> retreive port:" + Port + ". @multi");
			return Port;
		}
		else return 10000;
	}

	//Multi.PublishService (serviceType, customName, Network.player.port);
	public static void PublishService(string serviceType, string serviceName, int port){
		if (Application.platform != RuntimePlatform.OSXEditor)
			_PublishService(serviceType, serviceName, port);
		Debug.Log ("multi publish server successful. @multi");
	}

	//Multi.StopPublishService ();
	public static void StopPublishService (){
		if (Application.platform != RuntimePlatform.OSXEditor){
			_StopPublishService();
		}
	}
	 
	//Multi.ShutDown();
	public static void ShutDown(){
		if (Application.platform != RuntimePlatform.OSXEditor){
			_ShutDown();

			if(Debug.isDebugBuild){
				Debug.Log ("----> Shutting down Multi services. @multi");
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

