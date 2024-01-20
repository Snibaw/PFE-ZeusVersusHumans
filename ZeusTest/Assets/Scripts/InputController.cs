
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private GameObject canvasUI;

    private ThrowLightning throwLightning;
    private float x,y;
    
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
        // canvasUI = GameManager.instance.NBResources;
        throwLightning = GetComponent<ThrowLightning>();
    }
    private void ManageTownUI(bool isPhone = false)
    {
        Ray ray;
        if(isPhone) ray = mainCam.ScreenPointToRay(touch.position);
        else ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitVillage = false;

        if(canvasUI == null) return;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if( hit.collider.gameObject.CompareTag("Building"))
            {
                Building building = hit.collider.gameObject.GetComponent<Building>();
                if (building.BuildingType == BuildingType.village)
                {
                    hitVillage = true;
                    canvasUI.SetActive(true);
                    AdorationBar.instance.SetVisible(true,
                        building.gameObject.GetComponentInChildren<AdorationBarManager>());
                    canvasUI.GetComponent<ResourcesSlider>().SetVisible(true, building.gameObject.GetComponentInChildren<Storage>());
                }
            }

        }

        if (hitVillage == false)
        {
            canvasUI.SetActive(false);
            AdorationBar.instance.SetVisible(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //* This is for Android *//

        //Check if the player is touching the screen, then check if he is moving his finger (to rotate the planet) or not (to throw lightning)
        if(Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began: // If the player just touched the screen
                    timePressed = Time.time;
                    ManageTownUI(true);

                    if (touch.position.y < Screen.height * yBorderBtwRotationAndLightning)
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
                        Vector3 lightningDirection = new Vector3(x-xWhenPressed, y-yWhenPressed, 0);
                        throwLightning.FindFinalPointOnPlanet(lightningDirection, lightningDirection.magnitude, CalculateIntensity(Time.time-timePressed));
                    }
                    else
                    {
                        x = touch.deltaPosition.x;
                        y = touch.deltaPosition.y;
                        
                        if (Time.time-timePressed >= 0.03f) // If not, the planet will rotate too fast
                        {
                            cameraMovement.RotateAround(x,y);
                            if(!ReferenceEquals(_tutorial, null))
                                _tutorial.CompletePhase(1);
                        }
                    }

                    break;

                case TouchPhase.Stationary: // If the player didn't move his finger
                    if (isBelowYBorder)
                    {
                        x = touch.position.x;
                        y = touch.position.y;
                        Vector3 lightningDirection = new Vector3(x-xWhenPressed, y-yWhenPressed, 0);
                        throwLightning.FindFinalPointOnPlanet(lightningDirection, lightningDirection.magnitude, CalculateIntensity(Time.time-timePressed));
                    }
                        
                    break;

                case TouchPhase.Ended: // If the player stop touching the screen

                    if(isBelowYBorder && Time.time-timePressed > 0) // The player wants to throw lightning
                    {
                        if(xWhenPressed != x || yWhenPressed != y) // If the player move the mouse
                        {
                            if(!ReferenceEquals(_tutorial, null)) _tutorial.CompletePhase(2);
                            Vector3 lightningDirection = new Vector3(x-xWhenPressed, y-yWhenPressed, 0);
                            throwLightning.Throw(lightningDirection.normalized, lightningDirection.magnitude, CalculateIntensity(Time.time-timePressed));
                        }
                    }
                    if(!isBelowYBorder && Time.time - timePressed < timeBtwClickAndHold)
                    {
                        if(!ReferenceEquals(_tutorial, null)) _tutorial.CompletePhase(3);
                    }

                    timePressed = Time.time;
                    break;
            } 
        }
        else if (Input.touchCount == 2)
        {
            //Zoom in and out
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            
            float difference = currentMagnitude - prevMagnitude;
            Zoom(difference * 0.1f);
        }

        float CalculateIntensity(float time)
        {
            float preValue = time >= 1 ? 0 : Mathf.Pow(1 - time, 0.5f); // f(0) = 1 and f(x>=1) = 0
            return Mathf.Clamp(preValue, 0.25f, 1); // f(0) = 2 and f(x>=1) = 0.5f
        }

        void Zoom(float increment)
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment, 35, 80);
            if (Camera.main.fieldOfView <= 50)
            {
                GameManager.instance.ChangeShowHealthBars(true);
            }
            else
            {
                GameManager.instance.ChangeShowHealthBars(false);
            }
            Debug.Log("Camera.main.fieldOfView" + Camera.main.fieldOfView);
        }
    }
}
