using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NavmeshManager : MonoBehaviour
{

    public static NavMeshData _navMeshData;
    public static NavMeshBuildSettings _navMeshBuildSettings;
    public static List<NavMeshBuildSource> _navMeshBuildSources;
    public static Bounds _bounds;
    public static Transform _navTrans;

    //[SerializeField] private NavMeshBuildSettings _navMeshBuildSettingsT;
    //[SerializeField] private NavMeshBuildSource[] _navMeshBuildSourcesT;
    //[SerializeField] private Bounds _boundsT;
    //[SerializeField] private Transform _navTransT;

    private void Awake()
    {
        _navMeshBuildSettings = NavMesh.CreateSettings();
        _navMeshBuildSources = new List<NavMeshBuildSource>();

        foreach (var source in _navMeshBuildSources)
        {
            _navMeshBuildSources.Add(source);
        }

        _navMeshData = NavMeshBuilder.BuildNavMeshData(_navMeshBuildSettings, _navMeshBuildSources, new Bounds(Vector3.zero, new Vector3(10, 10, 10)), new Vector3(0, 0, 0), Quaternion.identity);
        NavMesh.AddNavMeshData(_navMeshData);

        /*
        _navMeshBuildSettings = NavMesh.CreateSettings();
        _navMeshBuildSources = NavMesh.
        _bounds = _boundsT;
        _navTrans = _navTransT;
        _navMeshData = NavMeshBuilder.BuildNavMeshData(_navMeshBuildSettings, _navMeshBuildSources, _bounds, _navTrans.position, _navTrans.rotation);
        */

    }

    public static void UpdateNavMesh()
    {
        //NavMeshBuilder.UpdateNavMeshDataAsync(_navMeshData, _navMeshBuildSettings, _navMeshBuildSources, _bounds);
        _navMeshData = NavMeshBuilder.BuildNavMeshData(_navMeshBuildSettings, _navMeshBuildSources, new Bounds(Vector3.zero, new Vector3(10, 10, 10)), new Vector3(0, 0, 0), Quaternion.identity);
        NavMesh.AddNavMeshData(_navMeshData);
    }
}
