using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Orbitable))]
public class OrbitabalEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Orbitable t = target as Orbitable;

		//Draw if has not been done yet
		if (t.trajectory == null)
		{
			t.UpdateTrajectory();
			t.MoveToStart();
		}

		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();
		if (EditorGUI.EndChangeCheck())
		{
			t.UpdateTrajectory();
			t.MoveToStart();
		}
	}
}
