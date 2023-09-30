using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnResources : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private MeshCollider planetCollider;
    private float planetRadius;
    private Vector3 planetCenter;
    [SerializeField] private float earthThickness = 0.3f; // Need to adapt this value if there are hills on the planet
    [SerializeField] private int maxIterationToFindAPosition = 100;

    [Header("ResourcesToSpawn")]
    [SerializeField] private GameObject folderParentOfResources;

    [SerializeField] private ResourceToSpawn[] ResourcesToSpawn;
    

    private void Start() 
    {
        planetRadius = planetCollider.bounds.extents.x;
        planetCenter = planetCollider.bounds.center;
        InitSpawnResourcesOnPlanet();
    }
    private void InitSpawnResourcesOnPlanet()
    {
        foreach (ResourceToSpawn Resource in ResourcesToSpawn)
        {
            if(Resource.spawnOnGroup)
            {
                for(int i = 0; i < Resource.numberOfGroupToSpawn; i++)
                {
                    // Spawn one Resource then spawn other Resources around it, good idea ??
                    Vector3 spawnPointOfFirstResource = SpawnSingleResource(Resource.prefab);
                    SpawnMultipleResources(Resource, spawnPointOfFirstResource);
                }
            }
            else
            {
                //Spawn the Resource independently
                for(int i = 0; i< Resource.numberToSpawn; i++)
                {
                    SpawnSingleResource(Resource.prefab);
                }
            }
        }
    }
    private Vector3 SpawnSingleResource(GameObject prefab)
    {
        Vector3 spawnPoint;
        spawnPoint = SearchAPositionToSpawn(prefab, "Random");
        spawnPoint += spawnPoint.normalized * earthThickness; // Spawn the Resource a little bit above so it's on the surface of the planet
        InstantiateResource(prefab, spawnPoint);
        return spawnPoint;
    }
    private void SpawnMultipleResources(ResourceToSpawn Resource, Vector3 spawnPointOfFirstResource)
    {
        Vector3 spawnPoint = spawnPointOfFirstResource;
        for(int i = 0; i < Resource.numberToSpawn - 1; i++) // First Resource was already spawned
        {
            spawnPoint = SearchAPositionToSpawn(Resource.prefab, "NearPoint", spawnPoint);
            spawnPoint += spawnPoint.normalized * earthThickness; // Spawn the Resource a little bit above so it's on the surface of the planet
            InstantiateResource(Resource.prefab, spawnPoint);
        }
    }
    private Vector3 SearchAPositionToSpawn(GameObject prefab, string typeOfPositionToSearch, Vector3 nearPoint = new Vector3())
    {
        RaycastHit hit;

        for(int i = 0; i < maxIterationToFindAPosition ; i++)
        {
            // typeOfPositionToSearch can be random or near a specific point
            Vector3 raycastPosition = ChooseRaycastPosition(typeOfPositionToSearch, nearPoint);
            Vector3 raycastToCenterOfPlanet = (planetCenter - raycastPosition).normalized;
            if(Physics.Raycast(raycastPosition, raycastToCenterOfPlanet, out hit, planetRadius*1.5f)) // If the raycast hit something (Be careful if earthThickness >> planetRadius, it will never hit the planet)
            {
                if(hit.collider.gameObject.tag == "Ground")
                {
                    return hit.point;
                }
            }
        } 
        Debug.Log("No position found to spawn " + prefab.name);
        return Vector3.zero;  
    }
    private Vector3 ChooseRaycastPosition(string typeOfPositionToSearch, Vector3 positionCloseTo)
    {
        switch(typeOfPositionToSearch)
        {
            case "Random":
                return planetCenter + Random.onUnitSphere * (planetRadius*2 + earthThickness); // Multiply by 2 si it's sure to be outside the planet
            case "NearPoint":
                //We want to find a point on the circle of radius 1 around positionCloseTo. We will then project this point on the plane perpendicular to the vector planetCenter-positionCloseTo
                Vector3 planeNormal = (positionCloseTo - planetCenter).normalized;
                float randomDistance = Random.Range(0.1f, 0.2f); // For now it's now in the ScriptableObject, but it should be I think
                float randomAngle = Random.Range(0.0f, 2.0f * Mathf.PI);
                // Calculate a point on the circle (r=1) using trigonometry
                Vector3 pointOnCircle = positionCloseTo + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
                // Project the point onto the plane perpendicular to the planeNormal
                Vector3 projectedPoint = pointOnCircle - Vector3.Dot(pointOnCircle - positionCloseTo, planeNormal) * planeNormal;
                // Calculate the new point near positionCloseTo
                Vector3 newPoint = projectedPoint + Random.onUnitSphere * randomDistance;
                return newPoint;  
            default:
                return Vector3.zero;
        }
    }
    private void InstantiateResource(GameObject prefab, Vector3 spawnPoint)
    {
        GameObject objSpawned = Instantiate(prefab, spawnPoint, Quaternion.identity);
        objSpawned.transform.rotation = Quaternion.FromToRotation(Vector3.up, objSpawned.transform.position - planetCenter); // Set the rotation
        objSpawned.transform.parent = folderParentOfResources.transform; // Set the parent
    }

}
