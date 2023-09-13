using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private PlanetMovement planetMovement;
    private ThrowLightning throwLightning;
    private float x,y;
    private bool fingerHasMoved = false;

    [SerializeField] private float timeBtwClickAndHold = 0.5f;
    private float timePressed = 0;

    //* This is for PC *//

    // private float xWhenPressed, yWhenPressed;
    
    // Start is called before the first frame update
    void Start()
    {
        throwLightning = GetComponent<ThrowLightning>();
        // timePressed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //* This is for Android *//

        //Check if the player is touching the screen, then check if he is moving his finger (to rotate the planet) or not (to throw lightning)
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
                    planetMovement.rotate(x,y);
                    break;

                case TouchPhase.Stationary: // If the player didn't move his finger

                    timePressed += Time.deltaTime;
                        
                    break;

                case TouchPhase.Ended: // If the player stop touching the screen

                    if(!fingerHasMoved && timePressed < timeBtwClickAndHold) // If the player didn't hold the touch and didn't move his finger, throw lightning
                    {
                        throwLightning.Throw();
                    }
                    break;
            } 
        }

        //* This is for PC *//

        // if(Input.GetMouseButton(0)) // If player left click
        // {
        //     //Rotate the planet
        //     x = Input.GetAxis("Mouse X");
        //     y = Input.GetAxis("Mouse Y");
        //     planetMovement.rotate(x,y);
        //     if(timePressed == 0) // If the player just clicked
        //     {
        //         xWhenPressed = x;
        //         yWhenPressed = y;
        //     }

        //     timePressed += Time.deltaTime;
        // }
        // else 
        // {
        //     if(timePressed < timeBtwClickAndHold && timePressed > 0) // Check if the player hold the click or not
        //     {
        //         if(xWhenPressed == x && yWhenPressed == y) // If the player didn't move the mouse
        //             throwLightning.Throw();
        //     }
        //     timePressed = 0;
        // }
    }
}
