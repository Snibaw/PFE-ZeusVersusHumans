using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceToSpawn", menuName = "ResourceToSpawn")]
public class ResourceToSpawn : ScriptableObject
{
    public GameObject prefab;
    public int numberToSpawn = 5;
    public bool spawnOnGroup = false;
    public int numberOfGroupToSpawn = 0;
}
