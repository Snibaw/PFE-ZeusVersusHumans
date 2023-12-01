using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private CameraMovement cameraMovement;
    private ThrowLightning throwLightning;
    private float x,y;
    private bool fingerHasMoved = false;

    [SerializeField] private SerialHandler serialHandler;

    [SerializeField] private float timeBtwClickAndHold = 0.5f;
    [SerializeField] private float yBorderBtwRotationAndLightning = 0.25f;
    private bool isBelowYBorder = false;
    private float timePressed = 0;

    //* This is for PC *//
    #if UNITY_EDITOR
        private float xWhenPressed, yWhenPressed;
    #endif
    
    void Start()
    {
        throwLightning = GetComponent<ThrowLightning>();
    }

    // Update is called once per frame
    void Update()
    {

        //* This is for Android *//

        //Check if the player is touching the screen, then check if he is moving his finger (to rotate the planet) or not (to throw lightning)

        /*
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began: // If the player just touched the screen

                    fingerHasMoved = false;
                    timePressed = 0;
                    break;

                case TouchPhase.Moved: // If the player move his finger

                    fingerHasMoved = true;
                    //Rotate the planet
                    x = touch.deltaPosition.x;
                    y = touch.deltaPosition.y;
                    Debug.Log("x" + x + "y" + y);
                    cameraMovement.RotateAround(x,y);

                    break;

                case TouchPhase.Stationary: // If the player didn't move his finger

                    timePressed += Time.deltaTime;
                        
                    break;

                case TouchPhase.Ended: // If the player stop touching the screen

                    if(!fingerHasMoved && timePressed < timeBtwClickAndHold) // If the player didn't hold the touch and didn't move his finger, throw lightning
                    {
                        // throwLightning.Throw();
                    }
                    break;
            } 
            

        }

        */

        //* This is for PC *//


#if UNITY_EDITOR
        // At the first input we check if its below or above the yborder between rotation and lightning
        /*
        if(Input.GetMouseButtonDown(0)) // First left click input
        {
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
            else //Rotate the planet
            {
                x = Input.GetAxis("Mouse X");
                y = Input.GetAxis("Mouse Y");

                if (timePressed >= 0.03f) // If not, the planet will rotate too fast
                {
                    cameraMovement.RotateAround(x,y);
                }

            }
            timePressed += Time.deltaTime;
        }
        else 
        {
            if(isBelowYBorder && timePressed > 0) // The player wants to throw lightning
            {
                if(xWhenPressed != x || yWhenPressed != y) // If the player move the mouse
                {
                    Vector3 lightningDirection = new Vector3(x-xWhenPressed, y-yWhenPressed, 0);
                    throwLightning.Throw(lightningDirection.normalized, lightningDirection.magnitude, timePressed);
                }
            }
            timePressed = 0;
        }
        */

        //Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Debug.Log("serialHandler.xValue " + serialHandler.xValue + " serialHandler.yValue " + serialHandler.yValue);
        float x = serialHandler.xValue;
        x = Mathf.Abs(x - 506) < 50 ? 0 : (x - 506);
        float y = serialHandler.yValue;
        y = Mathf.Abs(y - 509) < 50 ? 0 : (y - 509);

        //direction += new Vector3(x, -y, 0);
        Debug.Log(x + " | " + -y);

        if(x != 0 || y != 0)
        {
            Vector2 direction = new Vector2(x,-y).normalized;
            cameraMovement.RotateAround(direction.y, -direction.x);
        }
#endif
    }
}
