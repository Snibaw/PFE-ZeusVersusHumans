using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTownUI : MonoBehaviour
{
    [SerializeField] private float yBorderBtwRotationAndLightning = 0.25f;
    [SerializeField] private GameObject canvasUI;
    private ThrowLightning throwLightning;
    private float x,y;
    private bool fingerHasMoved = false;
    private bool isBelowYBorder = false;
    private float timePressed = 0;

    private Camera mainCam;

#if UNITY_EDITOR
        private float xWhenPressed, yWhenPressed;
    #endif

    private void Start()
    {
        mainCam = Camera.main;
    }


    private void ActivateUI()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 5f);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if( hit.collider.gameObject.CompareTag("Building"))
            {
                if (hit.collider.gameObject.GetComponent<Building>().BuildingType == BuildingType.village)
                {
                    canvasUI.SetActive(true);
                    canvasUI.GetComponent<UI_Town>().SetResourcesNb();
                }
            }
        }
    }

    private void DeActivateUI()
    {
        canvasUI.SetActive(false);
    }


    void Update()
    {
        //* This is for PC *//
        #if UNITY_EDITOR
            // At the first input we check if its below or above the yborder between rotation and lightning
            if(Input.GetMouseButtonDown(0)) // First left click input
            {
                if (canvasUI.activeInHierarchy)
                {
                    DeActivateUI(); //Deactivate UI when click elsewhere
                }
                
                if(Input.mousePosition.y < Screen.height * yBorderBtwRotationAndLightning)
                    isBelowYBorder = true;
                else
                    isBelowYBorder = false;
                
                xWhenPressed = Input.mousePosition.x;
                yWhenPressed = Input.mousePosition.y;
            }
            if(Input.GetMouseButton(0)) // If player hold left click
            {
                if(isBelowYBorder) 
                {
                    x = Input.mousePosition.x;
                    y = Input.mousePosition.y;
                }
                timePressed += Time.deltaTime;
            }
            if(Input.GetMouseButtonDown(0)) //Player stops holding
            {
                ActivateUI();
                // if(xWhenPressed == x && yWhenPressed == y) // If the player move the mouse
                // {
                //    ActivateUI();
                // }
                timePressed = 0;
            }
        #endif
    }
}
