using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimation : MonoBehaviour
{
    [SerializeField] private Animator _humanAnimator;
    [SerializeField] private NPCController _humanNPCController;
    private Vector3 _lastPosition;

    private void Start()
    {
        _lastPosition = transform.position;
    }
    void Update()
    {
        float currentSpeed = (Vector3.Distance(transform.position, _lastPosition)) / Time.deltaTime;

        _humanAnimator.SetFloat("speed", currentSpeed);

        _humanAnimator.SetBool("collecting", _humanNPCController.isExecuting);
    }

    private void LateUpdate()
    {
        _lastPosition = transform.position;
    }
}
