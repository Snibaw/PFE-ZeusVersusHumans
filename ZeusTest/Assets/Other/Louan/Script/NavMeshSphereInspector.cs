using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMeshSphere))]
public class NavMeshSphereInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavMeshSphere navMeshSphere = (NavMeshSphere)target;

        if (GUILayout.Button("Update Sub Div"))
        {
            navMeshSphere.UpdateSubDiv();
        }

        if (GUILayout.Button("Clear NavMesh Surface On Planet"))
        {
            navMeshSphere.ClearNavMeshSurfaceOnPlanet();
        }

    }
}
