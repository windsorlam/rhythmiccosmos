﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Setting : MonoBehaviour {
	
	public Material[] skyboxMat;
	public Material skyboxMatFinal;
	public static int index = 0;

	public GameObject[] airCrafts;
	public static int planeIndex = 0;

	public static int difficulty = 0;

	public GameObject pivot;
	private float movement = 0 ;

	public Scrollbar sr;
	
	// Use this for initialization
	void Start () {

		sr.value = difficulty;
		//DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (sr.value == 1) {
			difficulty = 1;
		} else {
			difficulty = 0;
		}
		Debug.Log (difficulty);
		DataManager dm = DataManager.Instance;
		dm.difficulty = (byte)difficulty;
	}


	public void OnMove(Vector2 offset)
	{   
		movement += offset.x;
		pivot.transform.RotateAround(pivot.transform.position, Vector3.down, offset.x);
	}


	public void OnTouchEnd()
	{
		pivot.transform.RotateAround(pivot.transform.position, Vector3.down, -movement);
		int temp = (int)Mathf.Round(movement / 90); 
		movement = temp * 90;

		planeIndex += temp;
		if (planeIndex > 3)
			planeIndex = 0;
		else if (planeIndex < 0)
			planeIndex = 3;

		pivot.transform.RotateAround(pivot.transform.position, Vector3.down, movement);
		
		movement = 0;
	}

	public void OnNext()
	{
		index = index == skyboxMat.Length - 1 ? 0 : ++index;
		skyboxMatFinal = skyboxMat [index];
		Camera.main.GetComponent<Skybox> ().material = skyboxMatFinal;
		Debug.Log (index);
	}
	
	public void OnPrevious()
	{
		index = index == 0 ? skyboxMat.Length-1 : --index;
		skyboxMatFinal = skyboxMat [index];
		Camera.main.GetComponent<Skybox> ().material = skyboxMatFinal;
		Debug.Log (index);
	}



}
