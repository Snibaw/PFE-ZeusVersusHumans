using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    public MoveController mover { get; set; }
    public Transform wolfGuide { get; set; }

    private WolfPack _wolfPack;

    [SerializeField] private float _rayonMovement = 10;

    [SerializeField] private LayerMask _layerMask;

    PointDistribution _pointDistribution;

    private void Start()
    {
        mover = GetComponent<MoveController>();

        Random.InitState(System.DateTime.Now.Second);

        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();

        StartCoroutine(FollowAroundWolfPack());

        _wolfPack = wolfGuide.GetComponent<WolfPack>();
    }

    private void Update()
    {
        WolfDetection();
    }

    private IEnumerator FollowAroundWolfPack()
    {
        yield return new WaitForSeconds(0.01f);

        while (true)
        {

            if(_wolfPack.humanToFollow == null)
            {
                mover.MoveTo(wolfGuide.position + Random.insideUnitSphere * 2f, false);
                yield return new WaitForSeconds(Random.Range(3, 8));
            }
            else
            {
                mover.MoveTo(_wolfPack.humanToFollow.position + Random.insideUnitSphere * 0.5f, false);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }
    }

    private void WolfDetection()
    {
        
        RaycastHit hit;
        Debug.Log("WolfDetection: " + Physics.SphereCast(transform.position, 1f, -transform.forward, out hit, 1f, _layerMask));
        if (Physics.SphereCast(transform.position, 1f, -transform.forward, out hit, 1f, _layerMask))
        {
            Debug.Log("Detected: "+ hit.collider.tag);
            Debug.Log("_wolfPack.humanToFollow: " + (_wolfPack.humanToFollow == null)); 
            if (_wolfPack.humanToFollow == null)
            {
                _wolfPack.humanToFollow = hit.collider.transform;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.forward * 1f, 1f);
    }


}
