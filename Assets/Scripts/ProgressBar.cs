﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

public class ProgressBar : MonoBehaviour {
	public Slider progressBar;
	private bool analysisFinished=false;
	private string ip="";
	delegate void AnalyzerHandler();
	
	private AnalyzerHandler aDelegate;
	private IAsyncResult ar;

	RPCLogicHandler _rpcHandler;
	// Use this for initialization
	void Start () {
		GameObject fb=GameObject.FindWithTag ("Browser");
		fb.SetActive (false);
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create ("http://i.cs.hku.hk/~wclin/ip.txt");
		request.Method="GET";
		HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
		Stream fs = response.GetResponseStream ();
		byte[] buffer=new byte[15];
		byte b=(byte)fs.ReadByte();
		while (b==46||b>47&&b<58) {
			ip+=(char)b;
			b=(byte)fs.ReadByte();
		}
		aDelegate=Analyze;
		ar=aDelegate.BeginInvoke(null,null);
	}
	
	// Update is called once per frame
	void Update () {
		DataManager dm=DataManager.Instance;
		progressBar.value=dm.progress;
		if (analysisFinished) {
			//aDelegate.EndInvoke(ar);
			Debug.Log("-------2");
			if( dm.isMultiPlayerMode ){
				Debug.Log("------1");
				if(UIEvents.LANorWAN == 1){
					_rpcHandler.analysisFinished = true;
				}else if(UIEvents.LANorWAN == 2){
					Debug.Log("0000000");
					Debug.Log("00110");
					//ConnectToServer.analysisFinished = true;
				}
			}else{
				Debug.Log("888888");
				Application.LoadLevel("Space");
			}

		}
	}

	void Analyze(){

		DataManager dm = DataManager.Instance;
		//if(!dm.isMultiPlayerMode)
		//	StartCoroutine(UploadAndDownload ());
		ip = "localhost";
		string url = "http://" + ip + ":8080/analyze";
		Debug.Log (url);
		Analyzer analyzer=new Analyzer();
		analyzer.Do(dm.musicPath,url);

	}

	IEnumerator UploadAndDownload()
	{
		DataManager dm = DataManager.Instance;
		dm.progress = 0f;
		string path = dm.dataPath+dm.musicPath;
		yield return StartCoroutine(UploadFile("https://s.staging.mossapi.com:8080/receive", path, new Dictionary<string, string>()));
		
		StartCoroutine(DownloadFile("https://s.staging.mossapi.com:8080/out1.yaml", dm.dataPath+"rhythm.yaml"));
		StartCoroutine(DownloadFile("https://s.staging.mossapi.com:8080/out2.yaml", dm.dataPath+"melody.yaml"));
		Analyzer analyzer = new Analyzer ();
		analyzer.Do (dm.musicPath);
		analysisFinished = true;
	}
	
	IEnumerator UploadFile(string url, string filePath, Dictionary<string, string> postJson)
	{
		WWWForm form = new WWWForm();
		
		foreach(var post in postJson)
		{
			form.AddField(post.Key, post.Value);
		}
		
		#if !UNITY_WEBPLAYER
		System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
		
		if(fileInfo.Exists)
		{
			var bytes = System.IO.File.ReadAllBytes(filePath);
			form.AddBinaryData("file", bytes, fileInfo.Name, "multipart/form-data");
		}
		
		#endif
		
		using(var www = new WWW(url, form))
		{
			yield return www;
			
			if(www.error == null)
			{
				Debug.Log("Uploaded");
			}
			else
			{
				Debug.Log("Upload Failed");
			}
		}
		DataManager dm = DataManager.Instance;
		dm.progress += 25f;
	}
	
	IEnumerator DownloadFile(string url, string filePath)
	{
		using(var www = new WWW(url))
		{
			yield return www;
			
			if(www.error == null)
			{
				Debug.Log("Download Successfully");
				
				var fileStream = System.IO.File.Create(filePath);
				fileStream.Write(www.bytes, 0, www.bytes.Length);
				fileStream.Close();
			}
			else
			{
				Debug.Log("Download Failed");
			}
		}
		DataManager dm = DataManager.Instance;
		dm.progress += 15f;
	}

}
