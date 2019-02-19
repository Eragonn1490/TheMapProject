using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class GalaxyCreator : MonoBehaviour
{
	//Public
	public GameObject starPrefab;
	public string galaxyFile;
	public bool createOnStart = true;

	[HideInInspector]
	public Galaxy galaxy;

	//Private
	private GameObject galaxyObj;

	private void Start()
	{
		if (createOnStart)
			CreateGalaxy();

		GameObject preview = GameObject.Find("Galaxy Preview");
		if (preview != null)
			Destroy(preview);
	}

	public void CreateGalaxy()
	{
		if (galaxyObj != null)
			Destroy(galaxyObj);

		if (galaxyFile == "")
		{
			Debug.LogWarning("Galaxy not provided. Could not create galaxy.");
			return;
		}

		if (starPrefab == null)
		{
			Debug.LogWarning("Star prefab not provided. Could not create galaxy.");
			return;
		}

		//Create galaxy parent
		galaxyObj = new GameObject("Galaxy");

        //Load galaxy
        if (GalaxyHelper.instance.gennedGalaxy.isGenned)
        {
            Debug.Log("setting galaxy to preloaded");
            galaxy = GalaxyHelper.instance.gennedGalaxy;
        }
        else
        {
            Debug.Log("loading galaxy from file");
            TextAsset gal = Resources.Load<TextAsset>(galaxyFile);
            Stream stream = new MemoryStream(gal.bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            galaxy = formatter.Deserialize(stream) as Galaxy;
            galaxy.isGenned = true;
            GalaxyHelper.instance.gennedGalaxy = galaxy; //load to the helper for keeping data.
        }
        
        

        //Create each star
        List<SolarSystemData> sysGets = new List<SolarSystemData>();
		foreach (SolarSystemData s in galaxy.solarSystems)
		{
            sysGets.Add(s);
			
		}
        for (int i = 0; i < sysGets.Count; i++)
        {
            GameObject obj = Instantiate(starPrefab);
            obj.transform.position = new Vector2(sysGets[i].posX, sysGets[i].posY);
            obj.transform.SetParent(galaxyObj.transform);
            StarInfo sInfo = obj.GetComponent<StarInfo>();
            sInfo.starName = sysGets[i].name;
            sInfo.starPos = new Vector3(sysGets[i].posX, sysGets[i].posY);
            sInfo.starID = sysGets[i].id;
            sInfo.starLinkIDs = new int[sysGets[i].linkedIDs.Length];
            sInfo.starLinkPositions = new Vector2[sysGets[i].linkedIDs.Length];
            for (int j = 0; j < sysGets[i].linkedIDs.Length; j++)
            {
                sInfo.starLinkIDs[j] = sysGets[i].linkedIDs[j];
                sInfo.starLinkPositions[j] = new Vector2(sysGets[sInfo.starLinkIDs[j]].posX, sysGets[sInfo.starLinkIDs[j]].posY);
            }
        }
        

    }
}
