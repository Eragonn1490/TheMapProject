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
		TextAsset gal = Resources.Load<TextAsset>(galaxyFile);
		Stream stream = new MemoryStream(gal.bytes);
		BinaryFormatter formatter = new BinaryFormatter();
		galaxy = formatter.Deserialize(stream) as Galaxy;

		//Create each star
		foreach (SolarSystemData s in galaxy.solarSystems)
		{
			GameObject obj = Instantiate(starPrefab);
			obj.transform.position = new Vector2(s.posX, s.posY);
			obj.transform.SetParent(galaxyObj.transform);
			StarInfo i = obj.GetComponent<StarInfo>();
			i.starName = s.name;
			i.starPos = new Vector3(s.posX, s.posY);
		}
	}
}
