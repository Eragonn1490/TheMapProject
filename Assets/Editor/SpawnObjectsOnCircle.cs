using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnObjectsOnCircle))]
public class SpawnObjectsOnCircleEditor : Editor
{
    private SpawnObjectsOnCircle _spawnObjectsOnCircle = null;
    private SpawnObjectsOnCircle SpawnObjectsOnCircle
    {
        get { return _spawnObjectsOnCircle ?? (_spawnObjectsOnCircle = (SpawnObjectsOnCircle)target); }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Spawn Objects")) SpawnObjectsOnCircle.SpawnObjects();
    }
}
