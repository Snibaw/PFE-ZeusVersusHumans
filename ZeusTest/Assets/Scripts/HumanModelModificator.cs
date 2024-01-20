using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanModelModificator : MonoBehaviour
{
    [SerializeField] private GameObject hat;
    
    public void ChangeHatColor(Color color)
    {
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.color = color;
        hat.GetComponent<Renderer>().material = newMat;
    }
}
