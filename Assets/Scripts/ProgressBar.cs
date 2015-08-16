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
	
	//private AnalyzerHandler aDelegate;
	//private IAsyncResult ar;
	public bool client;
	private DataManager dm;
	// Use this for initialization
	void Start () {

		if (UIEvents.LANorWAN == 1) {
			//if (RPCLogicHandler.role.Equals ("client")) {
				//client = true;
			//} else {
				//client = false;
			//}
		} else if (UIEvents.LANorWAN == 2) {
			if (ConnectToServer.role.Equals ("client")) {
				client = true;
			} else {
				client = false;
			}
		}
		dm = DataManager.Instance;
		Debug.Log(dm.isMultiPlayerMode + "and" + client);

		if (!(dm.isMultiPlayerMode&&client)) {
			Debug.Log(dm.isMultiPlayerMode + "and" + client);
			GameObject fb = GameObject.FindWithTag ("Browser");
			fb.SetActive (false);
		}
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
		Debug.Log ("multi " + dm.isMultiPlayerMode);
		Debug.Log ("preset? " + isPreset (dm.musicPath));
		if (dm.isMultiPlayerMode) {
			dm.absPath=Application.streamingAssetsPath+"/"+dm.musicPath;
		}
		if (isPreset(dm.musicPath)) {
			Debug.Log("start analysis");
			StartCoroutine(Analyze());
			//aDelegate = Analyze;
			//ar = aDelegate.BeginInvoke (null, null);
			
		} else {
			Debug.Log ("start up&down*analysis");
			StartCoroutine (UploadAndDownload ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		progressBar.value=dm.progress;
		//Debug.Log ("progress"+dm.progress);
		if (analysisFinished) {
			Debug.Log("finished!!");
			foreach(float i in dm.beatList){
				Debug.Log("b"+i);
			}
			foreach(float i in dm.onsetList){
				Debug.Log("onset"+i);
			}
			Debug.Log("abs "+dm.absPath);
			Debug.Log("name "+dm.musicPath);
			if( dm.isMultiPlayerMode ){
				if(UIEvents.LANorWAN == 1){
					//RPCLogicHandler.analysisFinished = true;
				}else if(UIEvents.LANorWAN == 2){
					//ConnectToServer.analysisFinished = true;
				}
			}else{
				Debug.Log("finished!! single mode");
				Application.LoadLevel("Space");
			}
			
		}
	}
	
	IEnumerator Analyze(){
		Analyzer analyzer = Analyzer.Instance;
		analyzer.Do ();
		//aDelegate.EndInvoke (ar);
		analysisFinished = true;
		Debug.Log("finished");
		yield return 0;
		/*ip = "localhost";
		string url = "http://" + ip + ":8080/analyze";
		Debug.Log (url);*/
	}
	
	IEnumerator UploadAndDownload()
	{
		Debug.Log ("up and down");
		dm.progress = 0f;
		yield return StartCoroutine(UploadFile("http://s.staging.mossapi.com:8080/receive", dm.absPath, new Dictionary<string, string>()));
		
		StartCoroutine(DownloadFile("http://s.staging.mossapi.com:8080/out1.yaml", Application.persistentDataPath+"/rhythm.yaml"));
		yield return StartCoroutine(DownloadFile("http://s.staging.mossapi.com:8080/out2.yaml", Application.persistentDataPath+"/melody.yaml"));
		Analyzer analyzer = Analyzer.Instance;
		analyzer.Do ();
		analysisFinished = true;
		//aDelegate.EndInvoke(ar);
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
	private bool isPreset(string musicPath)
	{
		DataManager dm=DataManager.Instance;
		FileStream file;
		try{
			file = new FileStream (Application.streamingAssetsPath +"/"+ musicPath,FileMode.Open,FileAccess.Read);
			
		}catch(Exception e){
			return false;
		}
		return file.CanRead;
	}
}
