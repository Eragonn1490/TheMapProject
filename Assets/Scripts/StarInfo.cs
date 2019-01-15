using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarInfo : MonoBehaviour
{
	//Public
	[HideInInspector]
	public string starName;
	[HideInInspector]
	public Vector2 starPos;

	//Public Static
	public static RectTransform starInfo;
	public static CanvasScaler cs;

	private void OnMouseEnter()
	{
		starInfo.gameObject.SetActive(true);
		starInfo.position = transform.position;
		StarInfoBox.EditDetails(starName, starPos);
	}

	private void OnMouseExit()
	{
		starInfo.localPosition = Vector3.zero;
		starInfo.gameObject.SetActive(false);
	}
}
