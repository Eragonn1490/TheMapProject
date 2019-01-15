using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkRenderer : MonoBehaviour
{
	//Public
	public GalaxyCreator galCreator;
	public float lineWidth;
	public bool drawDebugLinks = false;

	//Private
	private bool firstUpdate;
	private MeshFilter filter;
	private Mesh m;

	private void Start()
	{
		filter = GetComponent<MeshFilter>();
		m = new Mesh();
		filter.mesh = m;
		GenMesh();
	}

	private void Update()
	{
		if (!firstUpdate)
		{
			//Mesh generation needs to happen after start, but only once
			GenMesh();
			firstUpdate = true;
		}

		if (drawDebugLinks)
			DrawDebugLinks();
	}

	private void DrawDebugLinks()
	{
		for (int i = 0; i < galCreator.galaxy.solarSystems.Length; i++)
		{
			for (int j = 0; j < galCreator.galaxy.solarSystems[i].linked; j++)
			{
				//Get the id of the other solar system
				int endID = galCreator.galaxy.solarSystems[i].linkedIDs[j];

				//Continue if we have already been through the others links
				if (endID < i)
					continue;

				//Get start and end solar systems
				Vector3 start = new Vector3(
					galCreator.galaxy.solarSystems[i].posX, 
					galCreator.galaxy.solarSystems[i].posY);

				Vector3 end = new Vector3(
					galCreator.galaxy.solarSystems[endID].posX,
					galCreator.galaxy.solarSystems[endID].posY);

				Debug.DrawLine(start, end, Color.red);
			}
		}
	}

	private void GenMesh()
	{
		Galaxy g = galCreator.galaxy;

		//Need a list for vertices first
		///Start out with a capacity assuming each star has at least 1 (4 verts/quad) (6 inds/quad)
		List<Vector3> verts = new List<Vector3>(g.solarSystems.Length * 4);
		List<int> inds = new List<int>(g.solarSystems.Length * 6);

		for (int i = 0; i < g.solarSystems.Length; i++)
		{
			for (int j = 0; j < g.solarSystems[i].linked; j++)
			{
				//Get the id of the other solar system
				int endID = g.solarSystems[i].linkedIDs[j];

				//Continue if we have already been through the others links
				if (endID < i)
					continue;

				//Get start and end solar systems
				Vector2 start = new Vector2(
					g.solarSystems[i].posX,
					g.solarSystems[i].posY);

				Vector2 end = new Vector2(
					g.solarSystems[endID].posX,
					g.solarSystems[endID].posY);

				//Add two verts at start
				Vector2 dir = (start - end).normalized * (lineWidth / 2);
				verts.Add(start + new Vector2(-dir.y, dir.x)); //i: Count - 4
				verts.Add(start + new Vector2(dir.y, -dir.x)); //i: Count - 3

				//Add two verts at end
				dir *= -1;
				verts.Add(end + new Vector2(dir.y, -dir.x)); //i: Count - 2
				verts.Add(end + new Vector2(-dir.y, dir.x)); //i: Count - 1

				//Add indices
				inds.AddRange(new int[] { verts.Count - 2, verts.Count - 4, verts.Count - 1 });
				//inds.AddRange(new int[] { verts.Count - 4, verts.Count - 2, verts.Count - 1 });
				inds.AddRange(new int[] { verts.Count - 1, verts.Count - 4, verts.Count - 3 });
			}
		}

		m.vertices = verts.ToArray();
		m.triangles = inds.ToArray();
	}
}
