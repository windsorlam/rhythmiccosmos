using UnityEngine;
using System.Collections;

public class MultiPublisher : MonoBehaviour {

	[SerializeField] myConnection m_myConnection; //for message communication
	
	private bool serviceIsPublished;

	private int ListenPort;

	public void PublishService(string serviceName, string serviceType, int listenPort){
		ListenPort = listenPort;
		serviceIsPublished = true;
		StartServer ();
		if (Network.isServer) {
			Multi.PublishService (serviceType, serviceName, ListenPort);
		} else if (Debug.isDebugBuild) {
			Debug.LogError("Cannot publish a service before the server has been initialized. @multiPublisher");
		}
	}

	public void StartServer(){
		NetworkConnectionError error = Network.InitializeServer(3, ListenPort, false); //no NAT enable
		switch(error){
		case NetworkConnectionError.NoError:
			Debug.Log("----> Initialize a server successfully. @multiPublisher");
			break;
		default:
			Debug.Log("----> Faile to initialize a server. Error: " + error + ". @multiPublisher");
			break;
		}
	}

	public void StopPublishService(){
		if (Network.isServer && serviceIsPublished) {
			Multi.StopPublishService ();
			serviceIsPublished = false;
			Network.Disconnect ();
		} else if (Debug.isDebugBuild) {
			Debug.Log("Cannot stop publishing a service before the server has been initialized. @multiPublisher");
		}
	}

	//access if needed
	public bool ServiceIsPublished(){
		return serviceIsPublished;
	}

	public void DidShutDown(){
		StopPublishService ();
		serviceIsPublished = false;
	}

	////------ callback functionMethods called from native code by UnitySendMessage 
	public void ServiceWillPublish(string servicename){
		//i use custom name as service name to identify opponent
		if(Debug.isDebugBuild){
			Debug.Log("----> ServiceWillPublish is called. @multiPublisher");
			Debug.Log("----> Service will publish : " + servicename + ". @multiPublisher");
		}

		m_myConnection.OnServiceWillPublish();
	}

	public void ServiceDidPublish(string servicename){
		if(Debug.isDebugBuild){
			Debug.Log("----> ServiceDidPublish is called. @multiPublisher");
			Debug.Log("----> Service did published : " + servicename + ". @multiPublisher");
		}

		m_myConnection.OnServicePublished(servicename);
		
		serviceIsPublished = true;
	}
	
	public void ServiceDidStop(string serviceName){
		if(Debug.isDebugBuild){
			Debug.LogWarning("Service did stop publishing. @multiPublisher");
		}

		m_myConnection.OnServicePublishStopped(serviceName);
		
		serviceIsPublished = false;
	}
	
	public void DidNotPublish(string emptyString){
		if(Debug.isDebugBuild){
			Debug.LogWarning("---> Service failed to publish. @multiPublisher");
		}
		
		m_myConnection.OnPublishFailed();
		
		serviceIsPublished = false;
	}


	public void NSNetServiceFailed(string emptyString){
		if(Debug.isDebugBuild){
			Debug.LogWarning("An error occurred when initaliazing NSNetService object. @multiPublisher");
		}
		
		m_myConnection.OnPublishFailed();

		serviceIsPublished = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
