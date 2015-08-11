using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	public Slider progressBar;
	private bool analysisFinished=false;
	delegate void AnalyzerHandler();
	
	private AnalyzerHandler aDelegate;
	private IAsyncResult ar;

	RPCLogicHandler _rpcHandler;
	ConnectToServer _connectToServer;
	// Use this for initialization
	void Start () {
		GameObject fb=GameObject.FindWithTag ("Browser");
		fb.SetActive (false);
		aDelegate=Analyze;
		ar=aDelegate.BeginInvoke(null,null);
	}
	
	// Update is called once per frame
	void Update () {
		DataManager dm=DataManager.Instance;
		progressBar.value=dm.progress;
		if (analysisFinished) {
			aDelegate.EndInvoke(ar);
			if( dm.isMultiPlayerMode ){
				if(_rpcHandler != null){
					_rpcHandler.analysisFinished = true;
				}else if(_connectToServer != null){
					_connectToServer.analysisFinished = true;
				}
			}else{
				Application.LoadLevel("Space");
			}

		}
	}

	void Analyze(){
		Analyzer analyzer=new Analyzer();
		DataManager dm = DataManager.Instance;
		string path = dm.musicPath;
		analyzer.Do(path);
		analysisFinished = true;
	}
}
