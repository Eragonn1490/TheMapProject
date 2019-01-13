using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarInfoBox : MonoBehaviour
{
	//Public
	public RectTransform starInfoBox;
	public CanvasScaler canvasScaler;
	public Text nameTxt;
	public Text posTxt;
	public float posMutliplier = 500;
	public float thirdCoord = 0;

	//Singleton
	private static StarInfoBox instance;

	private void Start()
	{
		StarInfo.starInfo = starInfoBox;
		StarInfo.cs = canvasScaler;
		starInfoBox.gameObject.SetActive(false);

		if (instance == null)
			instance = this;
		else
			Debug.Log("Multiple star info box's detected. Please use only one.");
	}

	public static void EditDetails(string name, Vector2 position)
	{
		instance.nameTxt.text = name;

		Vector2 v = position * instance.posMutliplier;
		instance.posTxt.text = "(" + (int)v.x + ") (" + (int)v.y + ")";

		if (instance.thirdCoord != 0)
			instance.posTxt.text += " (" + instance.thirdCoord + ")";
	}
}
