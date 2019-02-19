using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class GalaxyGen
{
    
	static float debugCubeScale = 0.05f;

	//Returns false on fail
	public static bool GenGalaxy(GalaxyGenData data)
	{
        //Randomly choose star count
        Random.InitState(data.randSeed);
        int starCount = Random.Range(data.minStar, data.maxStar);

		//Generate based on shape
		List<Vector2> positions;
		switch (data.shape)
		{
			case GalaxyShape.Spiral:
				positions = GenSpiralGalaxy(starCount, data);
				break;
			case GalaxyShape.Elliptical:
				positions = GenEllipticalGalaxy(starCount, data);
				break;
			case GalaxyShape.Irregular:
				positions = GenIrregularGalaxy(starCount, data);
				break;
			case GalaxyShape.Ring:
				positions = GenRingGalaxy(starCount, data);
				break;
			default:
				positions = new List<Vector2>();
				break;
		}

		//Remove points that are too close
		RemoveTooClose(ref positions, data);

		//Name file to array of names
		string[] names = File.ReadAllLines(data.nameFileDir);

		//Take each position into a solar system class
		SolarSystemData[] solarSystems = new SolarSystemData[positions.Count];
		for (int i = 0; i < solarSystems.Length; i++)
		{
			solarSystems[i] = new SolarSystemData();
			solarSystems[i].id = i;
            solarSystems[i].generationSeed = data.randSeed;
            solarSystems[i].posX = positions[i].x;
			solarSystems[i].posY = positions[i].y;
			solarSystems[i].name = names[Random.Range(0, names.Length)];
			solarSystems[i].size = (SolarSystemSize)Random.Range(0, 2);
			solarSystems[i].type = GenRandomType(data);
			solarSystems[i].linkedIDs = new int[data.maxLinked];
		}

		//Setup links
		SetupLinks(ref solarSystems, data);

		//Link clusters if needed
		if (data.linkClusters)
		{
			List<List<int>> clusters = FindClusters(ref solarSystems, data);
			LinkClusters(ref solarSystems, clusters, data);
		}

		//Create galaxy
		Galaxy galaxy = new Galaxy();
        galaxy.randSeed = data.randSeed;
		galaxy.solarSystems = solarSystems;
		galaxy.generatedSystems = new SolarSystem[solarSystems.Length];
        for (int i = 0; i < galaxy.solarSystems.Length; i++)
        {
            galaxy.generatedSystems[i] = SetupSystemsFromGen(galaxy.solarSystems[i]);
            
        }

		//Save galaxy
		//if (!DataSerializer.CheckExistence(data.saveFileDir))
			DataSerializer.Save(galaxy, data.saveFileDir);
		//else
		//	return false;

		//Refresh assets
		AssetDatabase.Refresh();

		//Return success
		return true;
	}

    public static SolarSystem SetupSystemsFromGen(SolarSystemData data)
    {
        var sysGen = GameObject.FindObjectOfType<Generation_Solar_System>();
        
        
        var systemGet = sysGen.GetSystemFromGen((int)(data.posX), (int)(data.posY), data.generationSeed);


        return systemGet;
    }

	public static void PreviewData(GalaxyGenData data)
	{
        //Randomly choose star count
        Random.InitState(data.randSeed);
        int starCount = Random.Range(data.minStar, data.maxStar);

		//Generate based on shape
		List<Vector2> positions;
		switch (data.shape)
		{
			case GalaxyShape.Spiral:
				positions = GenSpiralGalaxy(starCount, data);
				break;
			case GalaxyShape.Elliptical:
				positions = GenEllipticalGalaxy(starCount, data);
				break;
			case GalaxyShape.Irregular:
				positions = GenIrregularGalaxy(starCount, data);
				break;
			case GalaxyShape.Ring:
				positions = GenRingGalaxy(starCount, data);
				break;
			default:
				positions = new List<Vector2>();
				break;
		}

		//Remove too close
		RemoveTooClose(ref positions, data);

		//Place the objects
		PlaceDebugPoints(positions);
	}

	private static List<Vector2> GenSpiralGalaxy(int starCount, GalaxyGenData data)
	{
		//Create empty points
		List<Vector2> points = new List<Vector2>(starCount);

		//Randomly generate each point's position inside of radius and star shape
		for (int i = 0; i < starCount; i++)
		{
			//Find random distance from center
			float dist = Random.Range(0, data.scale);

			//Find random arm angle
			int arm = Random.Range(0, data.spiralArmCount);
			float theta = arm * (360 / data.spiralArmCount);

			//Apply random offset
			float off = Random.Range(0, data.spiralMaxArmOffset);
			off = Mathf.Pow(off, data.spiralArmOffsetPow);
			if (Random.Range(0, 2) == 1) off *= -1;
			off *= 1 / dist;
			theta += off;

			//Rotate position based on distance and spiral factor
			theta += dist * data.spiralFactor;

			//Find point using angle and distance
			points.Add(new Vector2(
				Mathf.Cos(theta * Mathf.Deg2Rad),
				Mathf.Sin(theta * Mathf.Deg2Rad)));
			points[i] *= dist;
		}

		return points;
	}

	private static List<Vector2> GenEllipticalGalaxy(int starCount, GalaxyGenData data)
	{
		//Create empty points
		List<Vector2> points = new List<Vector2>(starCount);
		points.AddRange(new Vector2[starCount]);

		//Randomly generate each point's position inside of radius
		for (int i = 0; i < starCount; i++)
			points[i] = Random.insideUnitCircle * data.scale;

		return points;
	}

	private static List<Vector2> GenIrregularGalaxy(int starCount, GalaxyGenData data)
	{
		//Create empty points
		List<Vector2> points = new List<Vector2>(starCount);
		points.AddRange(new Vector2[starCount]);

		//Place each point x dist away from last point
		points[0] = Vector2.zero;
		for (int i = 1; i < starCount; i++)
		{
			//Gen angle
			float theta = Random.Range(0, 360);

			//Get dist
			float dist = Random.Range(data.irregMinDist, data.irregMaxDist);

			//Get offset vector
			Vector2 off = Vector2.zero;
			off.x = Mathf.Cos(theta * Mathf.Deg2Rad);
			off.y = Mathf.Sin(theta * Mathf.Deg2Rad);
			off *= dist;

			//Offset our new pos from last pos
			points[i] = points[i - 1] + off;

			//Redo if out of bound
			if (points[i].magnitude > data.scale)
				i--;
		}

		return points;
	}

	private static List<Vector2> GenRingGalaxy(int starCount, GalaxyGenData data)
	{
		//Create empty points
		List<Vector2> points = new List<Vector2>(starCount);
		points.AddRange(new Vector2[starCount]);

		//Randomly generate each point's position inside of radius and outside of inner rad
		for (int i = 0; i < starCount; i++)
		{
			points[i] = Random.insideUnitCircle * data.scale;
			if (points[i].magnitude < data.scale * data.ringSize)
				i--;
		}

		return points;
	}

	private static void PlaceDebugPoints(List<Vector2> points)
	{
		if (points == null)
			return;

		//Remove existing preview object
		GameObject preview = GameObject.Find("Galaxy Preview");
		if (preview != null)
			Object.DestroyImmediate(preview);

		//Create new preview object
		preview = new GameObject("Galaxy Preview");

		//Create each star under preview object
		foreach (Vector2 v in points)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = v;
			cube.transform.localScale = new Vector3(1, 1, 1) * debugCubeScale;
			cube.transform.SetParent(preview.transform);
		}
	}

	private static SolarSystemType GenRandomType(GalaxyGenData data)
	{
		int rand = Random.Range(0, data.fullWeight + data.blackHoleWeight + data.emptyWeight);

		if (rand < data.fullWeight)
			return SolarSystemType.Star;
		else if (rand < data.blackHoleWeight)
			return SolarSystemType.Blackhole;
		else if (rand < data.emptyWeight)
			return SolarSystemType.Empty;
		else
			return SolarSystemType.Star;
	}

	private static void SetupLinks(ref SolarSystemData[] systems, GalaxyGenData data)
	{
		//Setting up links is a slow process. 
		//Each system must iterate through others to check if its close enough.
		//This also includes a large amount of random numbers that would be optimal to remove
		//randomness in the reach.

		//Iterate through systems
		for (int i = 0; i < systems.Length; i++)
		{
			//Dont waste time if at max
			if (systems[i].linked >= data.maxLinked)
				continue;

			//Get random length that this star is able to reach
			float reach = data.linkDist + Random.Range(0, data.linkMaxOffset);

			//Check all other stars and link if close
			for (int j = i + 1; j < systems.Length; j++)
			{
				//Dont waste time if the other is at max
				if (systems[j].linked >= data.maxLinked)
					continue;

				//Stop if im at max
				if (systems[i].linked >= data.maxLinked)
					break;

				//Get distance
				float dist = Mathf.Sqrt(
					Mathf.Pow(systems[i].posX - systems[j].posX, 2) + 
					Mathf.Pow(systems[i].posY - systems[j].posY, 2));

				//Link eachother if within range
				if (dist < reach)
				{
					systems[i].linkedIDs[systems[i].linked] = systems[j].id;
					systems[i].linked++;

					systems[j].linkedIDs[systems[j].linked] = systems[i].id;
					systems[j].linked++;
				}
			}

			//Find nearest to link to if it needs
			if (data.requireOneLinked && systems[i].linked < 1)
			{
				int minID = -1;
				float minDist = int.MaxValue;
				for (int j = 0; j < systems.Length; j++)
				{
					//Get distance
					float dist = Mathf.Sqrt(
						Mathf.Pow(systems[i].posX - systems[j].posX, 2) +
						Mathf.Pow(systems[i].posY - systems[j].posY, 2));

					//Keep track if its the shortest yet
					if (dist < minDist && systems[j].linked < data.maxLinked && j != i)
					{
						minDist = dist;
						minID = j;
					}
				}

				//Link to closest
				systems[i].linkedIDs[systems[i].linked] = systems[minID].id;
				systems[i].linked++;

				systems[minID].linkedIDs[systems[minID].linked] = systems[i].id;
				systems[minID].linked++;
			}
		}
	}

	private static void RemoveTooClose(ref List<Vector2> points, GalaxyGenData data)
	{
		if (data.minDist <= 0)
			return;

		for (int i = 0; i < points.Count; i++)
		{
			for (int j = i + 1; j < points.Count; j++)
			{
				//Get distance
				float dist = Vector2.Distance(points[i], points[j]);

				//Remove
				if (dist < data.minDist)
				{
					points.RemoveAt(j);
					j--;
				}
			}
		}
	}

	private static List<List<int>> FindClusters(ref SolarSystemData[] systems, GalaxyGenData data)
	{
		List<List<int>> clusters = new List<List<int>>();

		for (int i = 0; i < systems.Length; i++)
		{
			//If its not in a cluster, gen the cluster
			if (!IsInClusters(i, clusters))
			{
				List<int> cluster = new List<int>();
				GetCluster(i, ref systems, ref cluster);
				clusters.Add(cluster);
			}
		}

		return clusters;
	}

	private static bool IsInClusters(int id, List<List<int>> clusters)
	{
		//Return true if it can find the id anywhere
		for (int i = 0; i < clusters.Count; i++)
		{
			for (int j = 0; j < clusters[i].Count; j++)
			{
				if (clusters[i][j] == id)
					return true;
			}
		}
		return false;
	}

	private static void GetCluster(int id, ref SolarSystemData[] systems, ref List<int> cur)
	{
		//Add this
		cur.Add(id);

		//Iterate over linked
		for (int i = 0; i < systems[id].linked; i++)
		{
			//Get cluster if not yet added
			if (!IsInCluster(systems[id].linkedIDs[i], cur))
				GetCluster(systems[id].linkedIDs[i], ref systems, ref cur);
		}
	}

	private static bool IsInCluster(int id, List<int> cluster)
	{
		for (int i = 0; i < cluster.Count; i++)
			if (id == cluster[i]) return true;
		return false;
	}

	private static void LinkClusters(ref SolarSystemData[] systems, List<List<int>> clusters, GalaxyGenData data)
	{
		//Get the largest cluster
		int major = 0;
		for (int i = 0; i < clusters.Count; i++)
		{
			if (clusters[i].Count > clusters[major].Count)
				major = i;
		}

		//Combine clusters until there is only 1 left
		int closest = -1;
		float minDist = float.MaxValue;
		int clusterStar = 0;
		float dist = -1;
		int star = -1;
		int closestCluster = -1;
		for (int k = clusters.Count; k > 1 + data.disconnectedClusters; k--)
		{
			minDist = float.MaxValue;

			//Get the closest point outside of cluster
			for (int i = 0; i < systems.Length; i++)
			{
				//Skip if its in cluster
				if (IsInCluster(i, clusters[major]))
					continue;

				//Skip if too many links
				if (systems[i].linked >= data.maxLinked)
					continue;

				//Keep track if its the closest point
				dist = GetDistFromCluster(ref systems, i, clusters[major], ref star, data);
				if (dist < minDist)
				{
					closest = i;
					minDist = dist;
					clusterStar = star;
				}

				//Stop if close enough
				if (dist < data.clusterLinkDist)
				{
					closest = i;
					minDist = dist;
					clusterStar = star;
					break;
				}
			}

			//Add link (closest and cluster star)
			systems[closest].linkedIDs[systems[closest].linked] = systems[clusterStar].id;
			systems[closest].linked++;

			systems[clusterStar].linkedIDs[systems[clusterStar].linked] = systems[closest].id;
			systems[clusterStar].linked++;

			//Get closest's cluster
			closestCluster = FindContainingCluster(closest, clusters);

			//Connect clusters
			clusters[major].AddRange(clusters[closestCluster]);
			clusters.RemoveAt(closestCluster);

			//Adjust major cluster index if needed
			if (closestCluster < major)
				major--;
		}
	}

	private static float GetDistFromCluster(ref SolarSystemData[] systems, int id, List<int> cluster, ref int clusterStar, GalaxyGenData data)
	{
		int closest = -1;
		float minDist = float.MaxValue;
		float dist = -1;
		for (int i = 0; i < cluster.Count; i++)
		{
			//Skip if too many links
			if (systems[cluster[i]].linked >= data.maxLinked)
				continue;

			//Use if closest so far
			dist = 
				Mathf.Pow(systems[cluster[i]].posX - systems[id].posX, 2) +
				Mathf.Pow(systems[cluster[i]].posY - systems[id].posY, 2);

			if (dist < minDist)
			{
				closest = cluster[i];
				minDist = dist;
			}
		}

		clusterStar = closest;
		return minDist;
	}

	private static int FindContainingCluster(int id, List<List<int>> clusters)
	{
		for (int i = 0; i < clusters.Count; i++)
		{
			for (int j = 0; j < clusters[i].Count; j++)
			{
				if (clusters[i][j] == id)
					return i;
			}
		}
		return -1;
	}
}
