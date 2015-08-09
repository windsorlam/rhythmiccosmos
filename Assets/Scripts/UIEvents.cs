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
			Destroy(GameObject.Find("RPCLogicHandler"));
			Application.LoadLevel ("MultiConnection");
		} else {
			Application.LoadLevel("Space");
		}
	}


	public void OnStartClick()
    {
        Application.LoadLevel("ModeChoose");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMainMenu()
    {
		if (multiMode) {
			Destroy(GameObject.Find("RPCLogicHandler"));
			multiMode = false;
		}
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
		DataManager dm = DataManager.Instance;
		dm.isMultiPlayerMode = true;
		Application.LoadLevel ("MultiConnection");
	}
	
	public void OnSingleClick() 
	{
		multiMode = false;
		DataManager dm = DataManager.Instance;
		dm.isMultiPlayerMode = false;
		Application.LoadLevel ("MusicChooser");
	}
}
