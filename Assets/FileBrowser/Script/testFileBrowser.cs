using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class testFileBrowser : MonoBehaviour {
	//skins and textures
	public GUISkin skin;
	public Texture2D file,folder,back,drive;
	//public GUIStyle cancelStyle,selectStyle;
	public GameObject slider;
	
	
	//	string[] layoutTypes = {"Type 0","Type 1"};
	//initialize file browser
	
	DataManager dm=DataManager.Instance;
	
	FileBrowser fb ;
	// Use this for initialization
	void Start () {
		fb=new FileBrowser(dm.isMultiPlayerMode);
		//setup file browser style
		fb.guiSkin = skin; //set the starting skin
		//set the various textures
		fb.fileTexture = file; 
		fb.directoryTexture = folder;
		fb.backTexture = back;
		fb.driveTexture = drive;
		//fb.cancelStyle = cancelStyle;
		//fb.selectStyle = selectStyle;
		//show the search bar
		fb.showSearch = true;
		//search recursively (setting recursive search may cause a long delay)
		fb.searchRecursively = true;
	}
	
	void OnGUI(){
		/*GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label("Layout Type");
		fb.setLayout(GUILayout.SelectionGrid(fb.layoutType,layoutTypes,1));
		GUILayout.Space(10);
		//select from available gui skins
		GUILayout.Label("GUISkin");
		foreach(GUISkin s in skins){
			if(GUILayout.Button(s.name)){
				fb.guiSkin = s;
			}
		}
		GUILayout.Space(10);
		fb.showSearch = GUILayout.Toggle(fb.showSearch,"Show Search Bar");
		fb.searchRecursively = GUILayout.Toggle(fb.searchRecursively,"Search Sub Folders");
		GUILayout.EndVertical();
		GUILayout.Space(10);
		//GUILayout.Label("Selected File: "+output);
		GUILayout.EndHorizontal();*/
		//draw and display output
		if(fb.draw()){ //true is returned when a file has been selected
			//the output file is a member if the FileInfo class, if cancel was selected the value is null
			//output = (fb.outputFile==null)?"cancel hit":fb.outputFile.ToString();
			if(fb.outputFile==null){
				Application.LoadLevel("Main");
			}else{
				slider.SetActive (true);
				string path=fb.outputFile.ToString();
				FileInfo file=fb.outputFile;
				dm.musicPath=file.Name;
				#if(UNITY_ANDROID)
				dm.absPath="/sdcard/storage/"+path;
#else
				dm.absPath=path;
				#endif
			}
		}
		
	}
	
	
	
	
	
}
