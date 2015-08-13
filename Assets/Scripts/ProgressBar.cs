using UnityEngine;
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

	private DataManager dm;
	// Use this for initialization
	void Start () {
		GameObject fb=GameObject.FindWithTag ("Browser");
		fb.SetActive (false);
		dm = DataManager.Instance;
		/*HttpWebRequest request = (HttpWebRequest)WebRequest.Create ("http://i.cs.hku.hk/~wclin/ip.txt");
		request.Method="GET";
		HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
		Stream fs = response.GetResponseStream ();
		byte[] buffer=new byte[15];
		byte b=(byte)fs.ReadByte();
		while (b==46||b>47&&b<58) {
			ip+=(char)b;
			b=(byte)fs.ReadByte();
		}*/
		if (!dm.isMultiPlayerMode) {
			Debug.Log ("debug log start analysis");
			Console.WriteLine ("console start analysis");
			StartCoroutine (UploadAndDownload ());
		} else {
			aDelegate = Analyze;
			ar = aDelegate.BeginInvoke (null, null);
		}
	}
	
	// Update is called once per frame
	void Update () {
		progressBar.value=dm.progress;
		//Debug.Log ("progress"+dm.progress);
		if (analysisFinished) {
			Debug.Log("finished!!");
			if( dm.isMultiPlayerMode ){
				if(UIEvents.LANorWAN == 1){
					RPCLogicHandler.analysisFinished = true;
				}else if(UIEvents.LANorWAN == 2){
					//ConnectToServer.analysisFinished = true;
				}
			}else{
				Debug.Log("finished!! single mode");
				Application.LoadLevel("Space");
			}

		}
	}

	void Analyze(){
		Analyzer analyzer = Analyzer.Instance;
		analyzer.Do (dm.musicPath);
		aDelegate.EndInvoke (ar);
		analysisFinished = true;
		Debug.Log("finished");
		/*ip = "localhost";
		string url = "http://" + ip + ":8080/analyze";
		Debug.Log (url);*/
	}

	IEnumerator UploadAndDownload()
	{
		Debug.Log ("up and down");
		dm.progress = 0f;
		yield return StartCoroutine(UploadFile("http://s.staging.mossapi.com:8080/receive", dm.dataPath+"/"+dm.musicPath, new Dictionary<string, string>()));
		
		StartCoroutine(DownloadFile("http://s.staging.mossapi.com:8080/out1.yaml", Application.persistentDataPath+"/rhythm.yaml"));
		StartCoroutine(DownloadFile("http://s.staging.mossapi.com:8080/out2.yaml", Application.persistentDataPath+"/melody.yaml"));
		Analyzer analyzer = Analyzer.Instance;
		analyzer.Do (dm.musicPath);
		analysisFinished = true;
		aDelegate.EndInvoke(ar);
	}
	
	IEnumerator UploadFile(string url, string filePath, Dictionary<string, string> postJson)
	{
		Debug.Log ("upload");
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
				Debug.Log("Upload Failed: "+www.error);
			}
		}
		dm.progress += 25f;
	}
	
	IEnumerator DownloadFile(string url, string filePath)
	{
		Debug.Log ("download");
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
				Debug.Log("Download Failed: "+www.error);
			}
		}
		dm.progress += 15f;
	}

}
