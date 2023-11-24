using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointDistribution))]
public class PointDistributionInterface : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PointDistribution pointDistribution = (PointDistribution)target;

        if (GUILayout.Button("Make Spheres Points"))
        {
            pointDistribution.Start();
        }

        if (GUILayout.Button("Make Path"))
        {
            pointDistribution.FindPath();
        }

        if (GUILayout.Button("Clear Spheres Folder"))
        {
            pointDistribution.ClearSphereFolder();
        }
    }
}