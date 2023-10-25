using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Billboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI bestActionText;
    [SerializeField] private TextMeshProUGUI inventoryText;
    private Transform mainCameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
    }

    public void UpdateStatsText(int energy, int hunger, int resource)
    {
        statsText.text = $"Energy: {energy}\nHunger: {hunger}\nResource: {resource}";
    }

    public void UpdateBestActionText(string bestAction)
    {
        bestActionText.text = bestAction;
    }

    public void UpdateInventoryText(int wood, int stone, int metal)
    {
        inventoryText.text = $"Wood: {wood}\nStone: {stone}\nMetal: {metal}";
    }
}