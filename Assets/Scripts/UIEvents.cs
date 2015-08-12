//此脚本附加在UI Root上
//用来处理各种按钮消息
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIEvents : MonoBehaviour {
	public static bool multiMode = false;
	public static int LANorWAN; //1 LAN  2 WAN
	public static string playerName;

	public InputField nameInputField;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnRestartClick()
	{
		if (multiMode) {
			if(LANorWAN == 1){
				Destroy (GameObject.Find ("RPCLogicHandler"));
				Application.LoadLevel ("MultiConnection");
				}else if (LANorWAN == 2){
					Application.LoadLevel ("InternetConnection");
				}	
		}else {
			Application.LoadLevel("Space");
		}
	}


	public void OnStartClick()
    {
		Application.LoadLevel ("InputName");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMainMenu()
    {
		if (multiMode) {
			multiMode = false;
			LANorWAN = 0;
			if(LANorWAN == 1){
				Destroy(GameObject.Find("RPCLogicHandler"));
			}
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
		Application.LoadLevel ("ConnectionMethodChooser");
	}

	public void OnEndEditClick(){
		if (nameInputField.text.Length <= 0) {
			return;
		} else {
			playerName = nameInputField.text;
			Application.LoadLevel("ModeChoose");
		}
	}

	public void OnLANClick(){
		multiMode = true;
		LANorWAN = 1;
		DataManager dm = DataManager.Instance;
		dm.isMultiPlayerMode = true;
		Application.LoadLevel ("MultiConnection");
	}

	public void OnWANClick(){
		multiMode = true;
		LANorWAN = 2;
		DataManager dm = DataManager.Instance;
		dm.isMultiPlayerMode = true;
		Application.LoadLevel ("InternetConnection");
	}
	
	public void OnSingleClick() 
	{
		multiMode = false;
		LANorWAN = 0;

		DataManager dm = DataManager.Instance;
		dm.isMultiPlayerMode = false;
		Application.LoadLevel ("MusicChooser");
	}
}
