using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Net;
using UnityEngine;

public class Analyzer
{
	public static float LENGTH=0.0029f;
	public static double[] noteFreqArr = {0.0,27.5,29.1,30.9,32.7,34.6,36.7,38.9,41.2,43.7,46.2,49.0,51.0,55.0,58.3,61.7,
		65.4,69.3,73.4,77.8,82.4,87.3,92.5,98.0,103.8,110.0,116.5,123.5,
		130.8,138.6,146.8,155.6,164.8,174.6,185.0,196.0,207.7,220.0,233.1,246.9,
		261.6,277.2,293.7,311.1,329.6,349.2,370.0,392.0,415.3,440.0,466.2,493.9,
		523.3,554.4,587.3,622.3,659.3,698.5,740.0,784.0,830.6,880.0,932.3,987.8,
		1047,1109,1175,1245,1319,1397,1480,1568,1661,1760,1865,1976,
		2093,2217,2349,2489,2637,2794,2960,3136,3322,3520,3729,3951,4186};
	public double beatInterval;
	public double melodyInterval;
	public double onsetInterval;
	public double fullBeatInterval;

	private Stream stream;
	private static readonly Encoding DEFAULTENCODE = Encoding.UTF8;

	private Analyzer(){}
	private static Analyzer instance=new Analyzer();
	public static Analyzer Instance{
		get{
			return instance;
		}
	}

	public void Do(string musicPath,string url)
	{
		DataManager dm=DataManager.Instance;
		dm.progress = 0f;
		switch (dm.difficulty) {
		case DataManager.EASY:
			fullBeatInterval=1;
			beatInterval=4;
			melodyInterval=1;
			onsetInterval=1;
			break;
		case DataManager.HARD:
			fullBeatInterval=0;
			beatInterval=2.2;
			melodyInterval=0.2;
			onsetInterval=0;
			break;
		}
		if (isPreset (musicPath)) {
			stream=new FileStream(dm.dataPath+"/"+dm.musicPath+" - rhythm.yaml",FileMode.Open, FileAccess.Read);
		}else{
			Encoding encoding = DEFAULTENCODE;
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString ("x");
			byte[] boundarybytes = Encoding.ASCII.GetBytes ("\r\n--" + boundary + "\r\n");
			byte[] endbytes = Encoding.ASCII.GetBytes ("\r\n--" + boundary + "--\r\n");
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			request.Method = "POST";
			request.KeepAlive = true;
			request.Credentials = CredentialCache.DefaultCredentials;
			using (Stream requestStream = request.GetRequestStream()) {
				string headerTemplate = "Content-Disposition: form-data; filename=\"{0}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
				byte[] buffer = new byte[4096];
				int bytesRead = 0;
				requestStream.Write (boundarybytes, 0, boundarybytes.Length);
				string header = string.Format (headerTemplate, Path.GetFileName (musicPath));
				byte[] headerbytes = encoding.GetBytes (header);                   
				requestStream.Write (headerbytes, 0, headerbytes.Length);
				dm.progress=5f;
				using (FileStream fileStream = new FileStream(musicPath, FileMode.Open, FileAccess.Read)) {                        
					while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
						requestStream.Write (buffer, 0, bytesRead);
						dm.progress+=0.005f;
					}
				}
				requestStream.Write (endbytes, 0, endbytes.Length);
				//Console.WriteLine (stream.Length.ToString ());
			}
			dm.progress=25;
			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			stream=response.GetResponseStream();
		}
		dm.progress = 50;
		StreamReader sr=new StreamReader(stream);
		string line;
		/**
		 * Parsing rhythm.yaml
		 */
		ArrayList beatList=new ArrayList();
		ArrayList onsetList=new ArrayList();
		ArrayList fullBeatList=new ArrayList();
		while ((line=sr.ReadLine())!=null){
			dm.progress+=(float)line.Length*20f/(float)stream.Length;
			if(line.Contains("beats_position")){
				string[] tokens=line.Trim("beats_position: []".ToCharArray()).Split(',');
				double lasttime=0.0;
				double fulllasttime=0.0;
				double time=0.0;
				for(int i=0;i<tokens.Length;i++){
					time=double.Parse(tokens[i]);
					double interval=time-lasttime;
					double fullInterval=time-fulllasttime;
					if(fullInterval>=fullBeatInterval){
						fullBeatList.Add((float)(fullInterval));
						fulllasttime=time;
					}
					if(interval>=beatInterval){
						beatList.Add((float)(interval));
						lasttime=time;
					}
				}
				//Console.WriteLine(i+" "+lasttime+" "+time);
				//controlsPerSec+=beatList.Count/time;
			}else if(line.Contains("onset_times")){
				string[] tokens=line.Trim("onset_times: []".ToCharArray()).Split(',');
				double lasttime=0.0;
				double time=0.0;
				for(int i=0;i<tokens.Length;i++){
					time=double.Parse(tokens[i]);
					double interval=time-lasttime;
					if(interval>=onsetInterval){
						onsetList.Add((float)interval);
						lasttime=time;
					}
				}
				//controlsPerSec+=onsetList.Count/time;
				//Console.WriteLine(i+" "+lasttime+" "+time);
				break;
			}
		}
		sr.Close();
		dm.beatList=beatList;
		dm.onsetList=onsetList;
		//dm.melodyList=melodyList;
		dm.fullBeatList=fullBeatList;
		//dm.difficultyRatio=controlsPerSec;
		dm.progress = 100f;
	}

	private bool isPreset(string musicPath)
	{
		DataManager dm=DataManager.Instance;
		FileStream file = new FileStream (dm.dataPath +"/"+ musicPath,FileMode.Open,FileAccess.Read);
		if (file.CanRead)
			return true;
		file = new FileStream (musicPath, FileMode.Open, FileAccess.Read);
		if (file.CanRead)
			return true;
		return false;
	}
	
	public void Do(string musicPath)
	{
		DataManager dm=DataManager.Instance;
		//dm.progress = 0f;
		switch (dm.difficulty) {
		case DataManager.EASY:
			fullBeatInterval=1;
			beatInterval=4;
			melodyInterval=1;
			onsetInterval=1;
			break;
		case DataManager.HARD:
			fullBeatInterval=0;
			beatInterval=2.2;
			melodyInterval=0.2;
			onsetInterval=0;
			break;
		}
		string melodyPath = Application.persistentDataPath+"/melody.yaml";
		string rhythmPath = Application.persistentDataPath+"/rhythm.yaml";
		if (dm.isMultiPlayerMode||isPreset(musicPath)) {
			melodyPath=dm.dataPath+"/"+dm.musicPath+" - melody.yaml";
			rhythmPath=dm.dataPath+"/"+dm.musicPath+" - rhythm.yaml";
		} else {
			/*Console.WriteLine(dm.dataPath+"/Algorithm/streaming_predominantmelody");
			Console.WriteLine(dm.dataPath+"/Algorithm/streaming_extractor");
			Process.Start (dm.dataPath+"/Algorithm/streaming_predominantmelody", "'" + musicPath + "' melody.yaml");
			Process.Start (dm.dataPath+"/Algorithm/streaming_extractor", "'" + musicPath + "' rhythm.yaml");
			while (true) {
				Process[] pmelody = Process.GetProcessesByName ("streaming_extra");
				for (int i=0; i<300; i++)
					dm.progress += 0.01f;
				Process[] prhythm = Process.GetProcessesByName ("streaming_predo");
				for (int i=0; i<300; i++)
					dm.progress += 0.01f;
				if (pmelody.Length == 0 && prhythm.Length == 0) {
					break;
				}
			}*/

		}
		for (; dm.progress<60; dm.progress+=0.01f)
			;
		double controlsPerSec = 0.0;
		Console.WriteLine ("Start parsing melody");
		try{
			/**
			 * Parsing melody.yaml
			 */
			FileStream fs1 = new FileStream (melodyPath, FileMode.Open);
			StreamReader sr1 = new StreamReader (fs1);
			FileStream fs2=new FileStream(rhythmPath,FileMode.Open);
			StreamReader sr2=new StreamReader(fs2);
			long fileSize=fs1.Length+fs2.Length;
			string line;
			ArrayList melodyList = new ArrayList();
			//Console.WriteLine(list[0]);
			int i;
			while ((line=sr1.ReadLine())!=null) {
				//Console.WriteLine("line");
				dm.progress+=(float)line.Length*80f/(float)fileSize;
				if (line.Contains ("pitch")) {
					//Console.WriteLine("pitch");
					string[] tokens = line.Trim ("pitch: []".ToCharArray ()).Split (',');
					double frequency = 0.0;
					double lasttime=0.0;
					double time=0.0;
					for (i=0; i<tokens.Length; i++) {
						double parsed=double.Parse(tokens[i]);
						//Console.WriteLine(parsed);
						double f = GetFrequency(parsed);
						if (f == frequency||f==0) {
							continue;
						}
						frequency = f;
						time = Analyzer.LENGTH * i;
						//Console.WriteLine(time+":"+frequency);
						if(time-lasttime>melodyInterval){
							melodyList.Add((float)(time-lasttime));
							lasttime=time;
						}
					}
					Console.WriteLine(i+" "+lasttime+" "+time);
					controlsPerSec+=melodyList.Count/time;
					break;
				}
			}
			sr1.Close ();
			Console.WriteLine ("Start parsing rhythm");
			/**
			 * Parsing rhythm.yaml
			 */
			ArrayList beatList=new ArrayList();
			ArrayList onsetList=new ArrayList();
			ArrayList fullBeatList=new ArrayList();
			while ((line=sr2.ReadLine())!=null){
				dm.progress+=(float)line.Length*80f/(float)fileSize;
				if(line.Contains("beats_position")){
					string[] tokens=line.Trim("beats_position: []".ToCharArray()).Split(',');
					double lasttime=0.0;
					double fulllasttime=0.0;
					double time=0.0;
					for(i=0;i<tokens.Length;i++){
						time=double.Parse(tokens[i]);
						double interval=time-lasttime;
						double fullInterval=time-fulllasttime;
						if(fullInterval>=fullBeatInterval){
							fullBeatList.Add((float)(fullInterval));
							fulllasttime=time;
						}
						if(interval>=beatInterval){
							beatList.Add((float)(interval));
							lasttime=time;
						}
					}
					Console.WriteLine(i+" "+lasttime+" "+time);
					controlsPerSec+=beatList.Count/time;
				}else if(line.Contains("onset_times")){
					string[] tokens=line.Trim("onset_times: []".ToCharArray()).Split(',');
					double lasttime=0.0;
					double time=0.0;
					for(i=0;i<tokens.Length;i++){
						time=double.Parse(tokens[i]);
						double interval=time-lasttime;
						if(interval>=onsetInterval){
							onsetList.Add((float)interval);
							lasttime=time;
						}
					}
					controlsPerSec+=onsetList.Count/time;
					Console.WriteLine(i+" "+lasttime+" "+time);
					break;
				}
			}
			sr2.Close();
			dm.beatList=beatList;
			dm.onsetList=onsetList;
			dm.melodyList=melodyList;
			dm.fullBeatList=fullBeatList;
			dm.difficultyRatio=controlsPerSec;
			if(dm.beatList!=null){
				Console.WriteLine("beatlist"+dm.beatList.Count.ToString());
			}
			if(dm.onsetList!=null){
				Console.WriteLine("onsetlist"+dm.onsetList.Count.ToString());
			}
			if(dm.melodyList!=null){
				Console.WriteLine("melodylist"+dm.melodyList.Count.ToString());
			}
			if(dm.fullBeatList!=null){
				Console.WriteLine("fullbeatlist"+dm.fullBeatList.Count.ToString());
			}
			for (; dm.progress<100; dm.progress+=0.01f);
		}catch(IOException ex){
			Console.WriteLine (ex.ToString ());
			return;
		}
	}
	
	public double GetFrequency(double f){
		double diff = double.MaxValue;
		double frequency=0.0;
		foreach (double freq in Analyzer.noteFreqArr) {
			double d=Math.Abs(freq-f);
			if(d<diff){
				diff=d;
				frequency=freq;
			}else{
				break;
			}
		}
		return frequency;
	}


}


