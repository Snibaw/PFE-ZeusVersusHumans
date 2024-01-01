using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WolfPack : MonoBehaviour
{
    private List<GameObject> _wolfs;
    [SerializeField] private GameObject _prefabWolf;
    [SerializeField] private Transform _parentWolf;
    PointDistribution _pointDistribution;

    public Transform humanToFollow;

    [SerializeField] private float _rayonMovement = 30;
    private int _numberOfWolf;

    private void Awake()
    {
        humanToFollow = null;
    }

    void Start()
    {
        Random.InitState(System.DateTime.Now.Second);
        _wolfs = new List<GameObject>();
        SetNumberOfWolf(1);
        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();

        StartCoroutine(WolfPackMovement());

        
    }

    public void SetNumberOfWolf(int numberOfWolf)
    {
        //Debug.Log("Nb Wolf Before:"+ _wolfs.Count);
        _numberOfWolf = numberOfWolf;
        int diff = _wolfs.Count - _numberOfWolf;
        if (diff > 0) 
        {
            for (int i = 0; i < diff; i++)
            {
                _wolfs.RemoveAt(0);
            }
        }
        else if(diff < 0)
        {
            for(int i = 0;i < Mathf.Abs(diff); i++)
            {
                GameObject wolf = Instantiate(_prefabWolf, transform.position, Quaternion.identity, _parentWolf);
                wolf.GetComponent<WolfController>().wolfGuide = transform;
                _wolfs.Add(wolf);
            }
        }
        //Debug.Log("Nb Wolf After:" + _wolfs.Count);
    }

    private IEnumerator WolfPackMovement()
    {
        yield return new WaitForSeconds(1);
        while (true) 
        {
            if(humanToFollow == null)
            {
                transform.position = _pointDistribution.FindTheClosestGraphNode(transform.position + Random.insideUnitSphere * _rayonMovement).Position;
                //Debug.Log("Guide: "+ transform.position);
                yield return new WaitForSeconds(Random.Range(15, 45));
            }
            else
            {
                transform.position = humanToFollow.position;
                yield return new WaitForEndOfFrame();
            }
            
        }
    }


}





[CustomEditor(typeof(WolfPack))]
public class WolfPackInterface : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WolfPack wolfPack = (WolfPack)target;
        if (GUILayout.Button("Set Number of Wolf: 0"))
        {
            wolfPack.SetNumberOfWolf(0);
        }

        if (GUILayout.Button("Set Number of Wolf: 1"))
        {
            wolfPack.SetNumberOfWolf(1);
        }

        if (GUILayout.Button("Set Number of Wolf: 3"))
        {
            wolfPack.SetNumberOfWolf(3);
        }

        if (GUILayout.Button("Set Number of Wolf: 10"))
        {
            wolfPack.SetNumberOfWolf(10);
        }
    }
}


