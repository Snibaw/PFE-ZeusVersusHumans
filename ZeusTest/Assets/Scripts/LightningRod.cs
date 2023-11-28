using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<LightningBehaviour>(out var lightning))
        {
            lightning.curve.A = lightning.transform;
            lightning.curve.B = transform;
            lightning.curve.Control = transform;
            lightning.speed *= 2;
        }
    }
}
