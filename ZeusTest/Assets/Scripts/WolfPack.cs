using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPack : MonoBehaviour
{
    private List<GameObject> _wolfs;
    [SerializeField] private GameObject _prefabWolf;
    [SerializeField] private Transform _parentWolf;
    PointDistribution _pointDistribution;

    public Transform humanToFollow;

    [SerializeField] private float _rayonMovement = 30;
    [SerializeField] private float _rayonDistanceFromOtherWolfPack = 100;
    private int _numberOfWolf;

    public int wolfsSkin;

    private void Awake()
    {
        humanToFollow = null;
    }

    void Start()
    {
        Random.InitState(System.DateTime.Now.Second);
        _wolfs = new List<GameObject>();
        SetNumberOfWolf(Random.Range(2,4));
        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();

        StartCoroutine(WolfPackMovement());

        
    }

    public void SetNumberOfWolf(int numberOfWolf)
    {
        _numberOfWolf = numberOfWolf;
        int diff = _wolfs.Count - _numberOfWolf;
        if (diff > 0) 
        {
            for (int i = 0; i < diff; i++)
            {
                Destroy(_wolfs[0]);
                _wolfs.RemoveAt(0);
            }
        }
        else if(diff < 0)
        {
            for(int i = 0;i < Mathf.Abs(diff); i++)
            {
                GameObject wolf = Instantiate(_prefabWolf, transform.position, Quaternion.identity, _parentWolf);
                wolf.GetComponent<WolfController>().wolfGuide = transform;
                wolf.GetComponent<WolfController>().ChooseWolfColor(wolfsSkin);
                _wolfs.Add(wolf);
            }
        }
    }

    private IEnumerator WolfPackMovement()
    {
        float delay = 0;
        yield return new WaitForSeconds(1);
        while (true) 
        {
            //Debug.Log("Human To Follow:" + humanToFollow);
            delay -= Time.deltaTime;
            if(humanToFollow == null)
            {
                if(delay <= 0)
                {
                    Vector3 wantedPosition = _pointDistribution.FindTheClosestGraphNode(transform.position + Random.insideUnitSphere * _rayonMovement).Position;
                    if (DistanceFromOtherWolfPacks(wantedPosition) > _rayonDistanceFromOtherWolfPack)
                    {
                        transform.position = wantedPosition;
                        delay = Random.Range(15, 45);
                    }
                        
                    
                }
                yield return new WaitForEndOfFrame();
            }
            else
            {
                transform.position = humanToFollow.transform.position;
                yield return new WaitForEndOfFrame();
            }
            
        }
    }

    private float DistanceFromOtherWolfPacks(Vector3 wantedPosition)
    {
        float distanceMin = Mathf.Infinity;

        foreach (WolfPack wolfPack in GameManager.instance.WolfPacks)
        {
            float distance = Vector3.Distance(wolfPack.transform.position, wantedPosition);
            if(wolfPack != this && distanceMin > distance)
            {
                distanceMin = distance;
            }
        }

        return distanceMin;


    }

    private void LateUpdate()
    {
        if (_wolfs.Count == 0) Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        GameManager.instance.WolfPacks.Remove(this);
    }


}




//
// [CustomEditor(typeof(WolfPack))]
// public class WolfPackInterface : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//         WolfPack wolfPack = (WolfPack)target;
//         if (GUILayout.Button("Set Number of Wolf: 0"))
//         {
//             wolfPack.SetNumberOfWolf(0);
//         }
//
//         if (GUILayout.Button("Set Number of Wolf: 1"))
//         {
//             wolfPack.SetNumberOfWolf(1);
//         }
//
//         if (GUILayout.Button("Set Number of Wolf: 3"))
//         {
//             wolfPack.SetNumberOfWolf(3);
//         }
//
//         if (GUILayout.Button("Set Number of Wolf: 10"))
//         {
//             wolfPack.SetNumberOfWolf(10);
//         }
//
//         if (GUILayout.Button("Set Number of Wolf: 200"))
//         {
//             wolfPack.SetNumberOfWolf(200);
//         }
//     }
// }


