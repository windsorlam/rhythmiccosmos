//此脚本附加在UI Root上
//用来处理各种按钮消息
using UnityEngine;
using System.Collections;

public class UIEvents : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnStartClick()
    {
        Application.LoadLevel("MusicChooser");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMainMenu()
    {
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

}
