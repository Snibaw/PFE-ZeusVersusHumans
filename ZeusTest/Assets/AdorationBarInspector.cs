using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdorationBar))]
public class AdorationBarInsoector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AdorationBar adorationBar = (AdorationBar)target;

        
        if (GUILayout.Button("Adoration bar 75%"))
        {
            adorationBar.SetValue(75);
        }

        if (GUILayout.Button("Adoration bar 25%"))
        {
            adorationBar.SetValue(25);
        }
        

    }
}