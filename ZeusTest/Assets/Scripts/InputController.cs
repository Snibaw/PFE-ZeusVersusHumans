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
            timePressed += Time.deltaTime;
            //Rotate the planet
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");
            planetMovement.rotate(x,y);
        }
        else // the player stop clicking
        {
            if(timePressed < timeBtwClickAndHold && timePressed > 0)
            {
                throwLightning.Throw();
            }
            timePressed = 0;
        }

    }
}
