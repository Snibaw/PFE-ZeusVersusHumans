using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private PlanetMovement planetMovement;
    [SerializeField] private float timeBtwClickAndHold = 0.5f;
    private float timePressed = 0;
    private ThrowLightning throwLightning;
    private float x,y;
    private float xWhenPressed, yWhenPressed;
    // Start is called before the first frame update
    void Start()
    {
        throwLightning = GetComponent<ThrowLightning>();
        x = 0;
        y = 0;
        timePressed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)) // If player left click
        {
            //Rotate the planet
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");
            planetMovement.rotate(x,y);
            if(timePressed == 0) // If the player just clicked
            {
                xWhenPressed = x;
                yWhenPressed = y;
            }

            timePressed += Time.deltaTime;
        }
        else 
        {
            if(timePressed < timeBtwClickAndHold && timePressed > 0) // Check if the player hold the click or not
            {
                if(xWhenPressed == x && yWhenPressed == y) // If the player didn't move the mouse
                    throwLightning.Throw();
            }
            timePressed = 0;
        }

    }
}
