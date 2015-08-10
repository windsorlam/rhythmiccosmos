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
using System.Collections;

public class DataManager
{
	public ArrayList beatList = null;
	public ArrayList onsetList = null;
	public ArrayList melodyList = null;
	public string musicPath = null;
	public bool isMultiPlayerMode=false;

	public static string HARD="Hard";
	public static string MEDIUM="Medium";
	public static string EASY="Easy";
	
	private DataManager ()
	{
	}
	
	private static DataManager instance=new DataManager();
	public static DataManager Instance{
		get{
			return instance;
		}
	}

	public float progress=0.0f;
	public double difficultyRatio=0.0;
	public string difficulty{
		get{
			if(difficultyRatio>5){
				return HARD;
			}else{
				if(difficultyRatio<1){
					return EASY;
				}else{
					return MEDIUM;
				}
			}
		}
	}
}