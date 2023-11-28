using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticCurve : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform Control;

    public Vector3 evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(A.position, Control.position, t);
        Vector3 cb = Vector3.Lerp(Control.position, B.position, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        if(A == null || B == null || Control == null) return;

        for(float t = 0; t < 1; t += 0.05f)
        {
            Gizmos.DrawSphere(evaluate(t), 0.1f);
        }
    }
}
