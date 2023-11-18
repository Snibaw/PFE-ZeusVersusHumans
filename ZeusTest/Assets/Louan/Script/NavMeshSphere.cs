using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class NavMeshSphere : MonoBehaviour
{
    [SerializeField] private Transform _planet;

    [SerializeField] private GameObject _prefabNavMeshSurface;

    [SerializeField] int _subDivHorizontal;
    [SerializeField] int _subDivVertical;

    private float _rayonSphere;

    public void UpdateSubDiv()
    {
        ClearNavMeshSurfaceOnPlanet();

        if (_subDivHorizontal < 0) _subDivHorizontal = 0;
        if (_subDivVertical < 0) _subDivVertical = 0;

        _rayonSphere = _planet.lossyScale.x;

        Quaternion angle = Quaternion.identity;

        for (int i = 0; i < _subDivVertical+1; i++)
        {
            for (int j = 0; j < _subDivHorizontal + 1; j++)
            {

                InstantiateNavMeshSurfaceOnPlanet(Quaternion.Euler(j * 360f / (_subDivHorizontal + 1f), 0f, i * 360f / (_subDivVertical + 1f)));

                Debug.Log("X = "+ (j * 360f / (_subDivHorizontal + 1f))+" | Z = "+(i * 360f / (_subDivVertical + 1f)));
            }
        }
    }

    private void InstantiateNavMeshSurfaceOnPlanet(Quaternion angle)
    {
        Vector3 position = new Vector3(0, _rayonSphere, 0);
        Vector3.RotateTowards(position, new Vector3(0, 0, 0), angle.x, angle.z);
        GameObject navMeshSurface = Instantiate(_prefabNavMeshSurface, position, Quaternion.LookRotation(new Vector3(0, 0, 0)),transform);
    }

    public void ClearNavMeshSurfaceOnPlanet()
    {
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
