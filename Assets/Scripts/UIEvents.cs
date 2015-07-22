//此脚本附加在UI Root上
//用来处理各种按钮消息
using UnityEngine;
using System.Collections;

public class UIEvents : MonoBehaviour {
	public static bool multiMode = false;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnRestartClick()
	{
		if (multiMode) {
			MultiManager.ShutDown();
			Application.LoadLevel ("MultiSpace");
		} else {
			Application.LoadLevel("Space");
		}
	}


	public void OnStartClick()
    {
        Application.LoadLevel("MultiStart");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMainMenu()
    {
		multiMode = false;
        Application.LoadLevel("Main");
    }

	public void OnHighScore()
	{
		Application.LoadLevel("HighScore");
	}

	public void OnPlayClick()
	{
		Application.LoadLevel ("Space");
	}

	public void OnSettingClick()
	{
		Application.LoadLevel ("Setting");
	}

	public void OnMultiClick()
	{
		multiMode = true;
		Application.LoadLevel ("MultiSpace");
	}

	public void OnSingleClick() 
	{
		multiMode = false;
		Application.LoadLevel ("MusicChooser");
	}

}
