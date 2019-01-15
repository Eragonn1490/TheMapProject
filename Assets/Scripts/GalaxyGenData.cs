using System.Collections;
using System.Collections.Generic;

public enum GalaxyShape { Spiral, Elliptical, Irregular, Ring };
[System.Serializable]
public enum SolarSystemSize { Small, Medium, Large };
[System.Serializable]
public enum SolarSystemType { Full, Blackhole, Empty };

//Galaxy generation parameters
public class GalaxyGenData
{
	//Public
	public GalaxyShape shape;
	public float scale = 5;
	public int minStar = 1000;
	public int maxStar = 1200;
	public float minDist = 0.07f;
	public string saveFileDir;
	public string nameFileDir;

	//Links
	public float linkDist = 0.15f;
	public float linkMaxOffset = 0.05f;
	public int maxLinked = 5;
	public bool requireOneLinked = true;
	public bool linkClusters = true;
	public float clusterLinkDist = 0.3f;
	public int disconnectedClusters = 0;

	//Solar system weights
	public int fullWeight = 300;
	public int blackHoleWeight = 20;
	public int emptyWeight = 20;

	//Spiral specifics
	public int spiralArmCount = 5;
	public float spiralFactor = 35;
	public float spiralMaxArmOffset = 8.5f;
	public float spiralArmOffsetPow = 2;

	//Irregular specifics
	public float irregMaxDist = 1;
	public float irregMinDist = 0.5f;

	//Ring specifics (0-1)
	public float ringSize = 0.5f;
}

//Data used for undiscoverd, ungenerated solar system. 
//These are the parameters used to later generate the actual solar system
[System.Serializable]
public class SolarSystemData
{
	//Public
	public int id;
	public float posX;
	public float posY;
	public string name;
	public SolarSystemSize size;
	public SolarSystemType type;
	public bool generated = false;
	public int[] linkedIDs;
	public int linked;
}

//Used for completely generated solar system
[System.Serializable]
public class SolarSystem
{
	//TODO
}

//Generated galaxy class
[System.Serializable]
public class Galaxy
{
	//Generated solar systems used if solarSystems[i].generated
	public SolarSystemData[] solarSystems;
	public SolarSystem[] generatedSystems;
}
