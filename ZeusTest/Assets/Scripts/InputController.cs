using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private CameraMovement cameraMovement;
    private GameObject canvasUI;
    private ThrowLightning throwLightning;
    private float x,y;
    private bool fingerHasMoved = false;

    [SerializeField] private float timeBtwClickAndHold = 0.5f;
    [SerializeField] private float yBorderBtwRotationAndLightning = 0.25f;
    private bool isBelowYBorder = false;
    private float timePressed = 0;
    
    private Camera mainCam;
    private float xWhenPressed, yWhenPressed;
    private Touch touch;
    
    void Start()
    {
        mainCam = Camera.main;
        canvasUI = GameManager.instance.NBResources;
        throwLightning = GetComponent<ThrowLightning>();
    }
    private void ActivateUI(bool isPhone = false)
    {
        Ray ray;
        if(isPhone) ray = mainCam.ScreenPointToRay(touch.position);
        else ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(canvasUI == null) return;
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

    // Update is called once per frame
    void Update()
    {

        //* This is for Android *//

        //Check if the player is touching the screen, then check if he is moving his finger (to rotate the planet) or not (to throw lightning)
        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began: // If the player just touched the screen

                    timePressed = Time.time;
                    if (canvasUI != null && canvasUI.activeInHierarchy)
                    {
                        canvasUI.SetActive(false); //Deactivate UI when click elsewhere
                    }
                    else
                    {
                        ActivateUI(true);
                    }
                    if( touch.position.y < Screen.height * yBorderBtwRotationAndLightning)
                        isBelowYBorder = true;
                    else
                        isBelowYBorder = false;
                    
                    xWhenPressed = touch.position.x;
                    yWhenPressed = touch.position.y;
                    break;

                case TouchPhase.Moved: // If the player move his finger
                    if (isBelowYBorder)
                    {
                        x = touch.position.x;
                        y = touch.position.y;
                    }
                    else
                    {
                        x = touch.deltaPosition.x;
                        y = touch.deltaPosition.y;
                        
                        if (Time.time-timePressed >= 0.03f) // If not, the planet will rotate too fast
                        {
                            cameraMovement.RotateAround(x,y);
                        }
                    }

                    break;

                case TouchPhase.Stationary: // If the player didn't move his finger
                    
                        
                    break;

                case TouchPhase.Ended: // If the player stop touching the screen

                    if(isBelowYBorder && Time.time-timePressed > 0) // The player wants to throw lightning
                    {
                        if(xWhenPressed != x || yWhenPressed != y) // If the player move the mouse
                        {
                            Vector3 lightningDirection = new Vector3(x-xWhenPressed, y-yWhenPressed, 0);
                            Debug.Log("TimePressed : " + timePressed);
                            throwLightning.Throw(lightningDirection.normalized, lightningDirection.magnitude, Time.time-timePressed);
                        }
                    }

                    timePressed = Time.time;
                    break;
            } 
        }

        //* This is for PC *//
        // #if UNITY_EDITOR
        //     // At the first input we check if its below or above the yborder between rotation and lightning
        //     if(Input.GetMouseButtonDown(0)) // First left click input
        //     {
        //         if (canvasUI != null && canvasUI.activeInHierarchy)
        //         {
        //             canvasUI.SetActive(false); //Deactivate UI when click elsewhere
        //         }
        //         else
        //         {
        //             ActivateUI();
        //         }
        //         if(Input.mousePosition.y < Screen.height * yBorderBtwRotationAndLightning)
        //             isBelowYBorder = true;
        //         else
        //             isBelowYBorder = false;
        //         
        //         xWhenPressed = Input.mousePosition.x;
        //         yWhenPressed = Input.mousePosition.y;
        //     }
        //     if(Input.GetMouseButton(0)) // If player hold left click
        //     {
        //         if(isBelowYBorder) 
        //         {
        //             x = Input.mousePosition.x;
        //             y = Input.mousePosition.y;
        //         }
        //         else //Rotate the planet
        //         {
        //             x = Input.GetAxis("Mouse X");
        //             y = Input.GetAxis("Mouse Y");
        //             
        //             if (timePressed >= 0.03f) // If not, the planet will rotate too fast
        //             {
        //                 cameraMovement.RotateAround(x,y);
        //             }
        //             
        //         }
        //         timePressed += Time.deltaTime;
        //     }
        //     else 
        //     {
        //         if(isBelowYBorder && timePressed > 0) // The player wants to throw lightning
        //         {
        //             if(xWhenPressed != x || yWhenPressed != y) // If the player move the mouse
        //             {
        //                 Vector3 lightningDirection = new Vector3(x-xWhenPressed, y-yWhenPressed, 0);
        //                 throwLightning.Throw(lightningDirection.normalized, lightningDirection.magnitude, timePressed);
        //             }
        //         }
        //         timePressed = 0;
        //     }
        // #endif
    }
}
