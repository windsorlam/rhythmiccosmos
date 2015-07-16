// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;


public class Analyzer
{
	public static int LENGTH=3;
	public static double[] noteFreqArr = {0.0,27.5,29.1,30.9,32.7,34.6,36.7,38.9,41.2,43.7,46.2,49.0,51.0,55.0,58.3,61.7,
		65.4,69.3,73.4,77.8,82.4,87.3,92.5,98.0,103.8,110.0,116.5,123.5,
		130.8,138.6,146.8,155.6,164.8,174.6,185.0,196.0,207.7,220.0,233.1,246.9,
		261.6,277.2,293.7,311.1,329.6,349.2,370.0,392.0,415.3,440.0,466.2,493.9,
		523.3,554.4,587.3,622.3,659.3,698.5,740.0,784.0,830.6,880.0,932.3,987.8,
		1047,1109,1175,1245,1319,1397,1480,1568,1661,1760,1865,1976,
		2093,2217,2349,2489,2637,2794,2960,3136,3322,3520,3729,3951,4186};
	
	public Analyzer ()
	{
	}
	
	public void Do(string musicPath)
	{
		string dir= Environment.CurrentDirectory;
		Process.Start (dir+"/Assets/Algorithm/streaming_predominantmelody", "'"+musicPath+"' melody.yaml");
		Process.Start (dir+"/Assets/Algorithm/streaming_extractor", "'"+musicPath+"' rhythm.yaml");
		DataManager dm=DataManager.Instance;
		while (true) {
			Process[] pmelody=Process.GetProcessesByName("streaming_extra");
			dm.progress+=3f;
			Process[] prhythm=Process.GetProcessesByName("streaming_predo");
			dm.progress+=3f;
			if(pmelody.Length==0&&prhythm.Length==0){
				break;
			}
		}
		dm.progress = 90f;
		try{
			/**
			 * Parsing melody.yaml
			 */
			FileStream fs1 = new FileStream ("melody.yaml", FileMode.Open);
			StreamReader sr1 = new StreamReader (fs1);
			FileStream fs2=new FileStream("rhythm.yaml",FileMode.Open);
			StreamReader sr2=new StreamReader(fs2);
			long fileSize=fs1.Length+fs2.Length;
			string line;
			Queue<float> melodyList = new Queue<float>();
			melodyList.Enqueue (0f);
			//Console.WriteLine(list[0]);
			while ((line=sr1.ReadLine())!=null) {
				//Console.WriteLine("line");
				dm.progress+=(float)line.Length*20f/(float)fileSize;
				if (line.Contains ("pitch")) {
					//Console.WriteLine("pitch");
					string[] tokens = line.Trim ("pitch: []".ToCharArray ()).Split (',');
					double frequency = 0.0;
					double lasttime=0.0;
					for (int i=0; i<tokens.Length; i++) {
						double parsed=double.Parse(tokens[i]);
						//Console.WriteLine(parsed);
						double f = GetFrequency(parsed);
						if (f == frequency) {
							continue;
						}
						frequency = f;
						double time = Analyzer.LENGTH * i;
						//Console.WriteLine(time+":"+frequency);
						melodyList.Enqueue ((float)(time-lasttime));
						lasttime=time;
					}
					break;
				}
			}
			sr1.Close ();
			/**
			 * Parsing rhythm.yaml
			 */

			Queue<float> beatList=new Queue<float>();
			Queue<float> onsetList=new Queue<float>();
			while ((line=sr2.ReadLine())!=null){
				dm.progress+=(float)line.Length*20f/(float)fileSize;
				if(line.Contains("beats_position")){
					string[] tokens=line.Trim("beats_position: []".ToCharArray()).Split(',');
					double lasttime=0.0;
					for(int i=0;i<tokens.Length;i++){
						double time=double.Parse(tokens[i]);
						beatList.Enqueue((float)(time-lasttime));
						lasttime=time;
					}
				}else if(line.Contains("onset_times")){
					string[] tokens=line.Trim("onset_times: []".ToCharArray()).Split(',');
					double lasttime=0.0;
					for(int i=0;i<tokens.Length;i++){
						double time=double.Parse(tokens[i]);
						onsetList.Enqueue((float)(time-lasttime));
						lasttime=time;
					}
					break;
				}
			}
			sr2.Close();
			dm.beatList=beatList;
			dm.onsetList=onsetList;
			dm.melodyList=melodyList;
			dm.progress=100f;
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


