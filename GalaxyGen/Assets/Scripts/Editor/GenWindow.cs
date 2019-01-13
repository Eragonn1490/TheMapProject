using UnityEngine;
using UnityEditor;

public class GenWindow : EditorWindow
{
	GalaxyGenData data;
	//Name file directory relative to assets folder
	string nameRelDir = "GalaxyGen/Names";
	string saveRelDir = "Resources/Galaxies/Galaxy01";
	Vector2 scrollPos;

	[MenuItem("Window/Galaxy Generation")]
	static void Init()
	{
		GenWindow window = (GenWindow)GetWindow(typeof(GenWindow));
		window.titleContent.text = "Galaxy Gen";
		window.minSize = new Vector2(350, 300);
		window.Show();
	}

	GenWindow()
	{
		data = new GalaxyGenData();
	}

	void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		//Styles
		GUIStyle titleStyle = new GUIStyle();
		titleStyle.fontSize = 25;
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.fontStyle = FontStyle.Bold;

		GUIStyle boldStyle = new GUIStyle();
		boldStyle.fontStyle = FontStyle.Bold;

		GUIStyle fadedStyle = new GUIStyle();
		fadedStyle.normal.textColor = new Color(
			fadedStyle.normal.textColor.r,
			fadedStyle.normal.textColor.g,
			fadedStyle.normal.textColor.b,
			0.5f
			);

		//Title
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Galaxy Generation", titleStyle);

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		//Galaxy options
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Galaxy Options", boldStyle);

		data.shape = (GalaxyShape)EditorGUILayout.EnumPopup("Shape", data.shape);
		data.scale = EditorGUILayout.FloatField("Galaxy Scale", data.scale);
		data.minStar = EditorGUILayout.IntField("Minimum Stars", data.minStar);
		data.maxStar = EditorGUILayout.IntField("Maximum Stars", data.maxStar);
		if (data.maxStar <= data.minStar) data.maxStar = data.minStar + 1;
		data.minDist = EditorGUILayout.FloatField("Min Star Distance", data.minDist);

		//Link Options
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Link Options", boldStyle);

		data.linkDist = EditorGUILayout.FloatField("Link Distance", data.linkDist);
		data.linkMaxOffset = EditorGUILayout.FloatField("Link Max Offset", data.linkMaxOffset);
		data.maxLinked = EditorGUILayout.IntField("Max Linked", data.maxLinked);
		data.requireOneLinked = EditorGUILayout.Toggle("Require 1 Link", data.requireOneLinked);
		data.linkClusters = EditorGUILayout.Toggle("Link All Clusters", data.linkClusters);
		data.clusterLinkDist = EditorGUILayout.FloatField("Clust Link Dist", data.clusterLinkDist);
		data.disconnectedClusters = EditorGUILayout.IntField("Disconnected Clusters", data.disconnectedClusters);

		//Solar system type weights
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Solar System Type Weights", boldStyle);

		int sum = data.fullWeight + data.blackHoleWeight + data.emptyWeight;

		EditorGUILayout.BeginHorizontal();
			data.fullWeight = EditorGUILayout.IntField("Full Probability", data.fullWeight);
			EditorGUILayout.LabelField("/" + sum, GUILayout.Width(100));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
			data.blackHoleWeight = EditorGUILayout.IntField("Black Hole Probability", data.blackHoleWeight);
			EditorGUILayout.LabelField("/" + sum, GUILayout.Width(100));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
			data.emptyWeight = EditorGUILayout.IntField("Empty Probability", data.emptyWeight);
			EditorGUILayout.LabelField("/" + sum, GUILayout.Width(100));
		EditorGUILayout.EndHorizontal();

		//Spiral specifics
		if (data.shape == GalaxyShape.Spiral)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Spiral Specifics", boldStyle);
			data.spiralArmCount = EditorGUILayout.IntSlider("Arm Count", data.spiralArmCount, 1, 10);
			data.spiralFactor = EditorGUILayout.Slider("Spiral Factor", data.spiralFactor, -60, 60);
			data.spiralMaxArmOffset = EditorGUILayout.Slider("Max Arm Offset", data.spiralMaxArmOffset, 0, 30);
			data.spiralArmOffsetPow = EditorGUILayout.Slider("Arm Offset Power", data.spiralArmOffsetPow, 0, 5);
		}

		//Irregular specifics
		if (data.shape == GalaxyShape.Irregular)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Irregular Specifics", boldStyle);
			data.irregMinDist = EditorGUILayout.FloatField("Min Distance", data.irregMinDist);
			data.irregMaxDist = EditorGUILayout.FloatField("Max Distance", data.irregMaxDist);
		}

		//Ring specifics
		if (data.shape == GalaxyShape.Ring)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Ring Specifics", boldStyle);
			data.ringSize = EditorGUILayout.Slider("Ring Size", data.ringSize, 0.99f, 0.001f);
		}

		//Files
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("File Paths", boldStyle);

		//Name file field
		EditorGUILayout.LabelField("Name File");

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Assets/", fadedStyle, GUILayout.Width(47));
			nameRelDir = EditorGUILayout.TextField(nameRelDir);
			EditorGUILayout.LabelField(".txt", fadedStyle, GUILayout.Width(35));
		EditorGUILayout.EndHorizontal();

		data.nameFileDir = "Assets/" + nameRelDir + ".txt";

		//Save file field
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Save Dir");

        

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Assets/", fadedStyle, GUILayout.Width(47));
			saveRelDir = EditorGUILayout.TextField(saveRelDir);
			EditorGUILayout.LabelField(".bytes", fadedStyle, GUILayout.Width(35));
		EditorGUILayout.EndHorizontal();

		data.saveFileDir = "Assets/" + saveRelDir + ".bytes";

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        data.randSeed = EditorGUILayout.IntField("RandomSeed", data.randSeed);
        EditorGUILayout.EndHorizontal();

        //Generate and save galaxy
        EditorGUILayout.Space();
		if (GUILayout.Button("Save as Galaxy", GUILayout.Height(40)))
			GalaxyGen.GenGalaxy(data);

		//Preview galaxy file
		EditorGUILayout.Space();
		if (GUILayout.Button("Preview", GUILayout.Height(40)))
			GalaxyGen.PreviewData(data);

		EditorGUILayout.EndScrollView();
	}
}
