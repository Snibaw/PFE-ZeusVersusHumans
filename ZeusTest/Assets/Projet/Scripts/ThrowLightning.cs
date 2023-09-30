using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowLightning : MonoBehaviour
{
    private Camera cam;

    [Header("Energie")]
    [SerializeField] private Slider sliderEnergie;
    [SerializeField] private float maxEnergie = 100f;
    [SerializeField] private float regenEnergie = 0.01f;
    [SerializeField] private float minEnergieToThrowLightning = 5f;
    public float energie;
    void Start()
    {
        cam = Camera.main;
        energie = maxEnergie;

        sliderEnergie.maxValue = maxEnergie;
    }

    public void Throw()
    {
        if(energie > minEnergieToThrowLightning)
        {
            //Create a raycast from the camera to the mouse position but only hit the layer "CanBeDestroyed"
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 5f);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100f))
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("CanBeDestroyed"))
                {
                    Debug.Log(hit.collider.gameObject.name + " was hit with your lightning");
                    hit.collider.gameObject.GetComponent<ObjectToDestroy>().TakeDamage(energie);
                    SetEnergie(0);
                }
                
            }            
        }
    }
    private void FixedUpdate() {
        //RegenEnergie
        SetEnergie(Mathf.Min(energie + regenEnergie, maxEnergie));
    }
    void SetEnergie(float _energie)
    {
        energie = _energie;
        sliderEnergie.value = energie;
    }
}
