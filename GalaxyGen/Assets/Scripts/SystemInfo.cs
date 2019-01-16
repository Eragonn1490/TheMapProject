using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemInfo : MonoBehaviour
{
    public Text starName;
    public Text userSystemName;//Currently Unused will be used for when a player meets the requirements to rename a solar system
    public Text solarSystemUI;

    public int celestialObjectSize;

	// Use this for initialization
	void Start ()
    {
        if (!starName)
        {
            starName = this.GetComponent<Text>();
        }
        Debug.Log("setting systeminfo for" + this.gameObject.name);
        if (solarSystemUI)
        {
            solarSystemUI.text = starName.text;
        }
        
        celestialObjectSize.ToString();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
