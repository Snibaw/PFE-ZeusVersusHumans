using System.Collections;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    public MoveController mover { get; set; }
    public Transform wolfGuide { get; set; }

    private Animator _wolfAnimator;

    [SerializeField] private GameObject[] _skinWolf;

    public WolfPack _wolfPack;

    private Transform _closestHumanToFollow;

    [SerializeField] private float _rayonMovement = 10;

    [SerializeField] private LayerMask _layerMask;

    PointDistribution _pointDistribution;

    private float _timeLastAttack;

    [SerializeField] private float _cooldownAttack;

    [SerializeField] private float _rangeAttack;

    private void Start()
    {
        _closestHumanToFollow = null;

        mover = GetComponent<MoveController>();

        Random.InitState(System.DateTime.Now.Second);

        _pointDistribution = GameObject.FindWithTag("Planet").GetComponent<PointDistribution>();

        StartCoroutine(FollowAroundWolfPack());
        StartCoroutine(AnimationMovement());

        _wolfPack = wolfGuide.GetComponent<WolfPack>();

    }

    public void ChooseWolfColor(int choice)
    {
        for (int i = 0; i < _skinWolf.Length; i++)
        {
            if ((choice % _skinWolf.Length) == i) 
            {
                _skinWolf[i].SetActive(true);
                _wolfAnimator = _skinWolf[i].GetComponent<Animator>();
            }
            else Destroy(_skinWolf[i]);
        }
    }

    private void Update()
    {
        WolfDetection();
        Attack();
    }

    private IEnumerator FollowAroundWolfPack()
    {
        yield return new WaitForSeconds(0.01f);

        while (true)
        {
            if(_closestHumanToFollow != null)
            {
                mover.MoveTo(_closestHumanToFollow.position + Random.insideUnitSphere * 0.5f, false);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
            else if(_wolfPack.humanToFollow != null)
            {
                mover.MoveTo(_wolfPack.humanToFollow.position + Random.insideUnitSphere * 0.5f, false);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
            else
            {
                
                mover.MoveTo(wolfGuide.position + Random.insideUnitSphere * 2f, false);
                yield return new WaitForSeconds(Random.Range(3, 8));
            }
        }
    }

    private void WolfDetection()
    {
        
        RaycastHit hit;
        //Debug.Log("WolfDetection: " + Physics.SphereCast(transform.position, 1f, -transform.forward, out hit, 1f, _layerMask));
        if (Physics.SphereCast(transform.position, 1f, -transform.forward, out hit, 1f, _layerMask))
        {
            //Debug.Log("Detected: "+ hit.collider.tag);
            //Debug.Log("_wolfPack.humanToFollow: " + (_wolfPack.humanToFollow == null)); 
            if (_wolfPack.humanToFollow == null)
            {
                if (hit.collider.transform.parent.parent.gameObject.GetComponent<NPCController>().canBeAttacked()) _wolfPack.humanToFollow = hit.collider.transform;
            }
            else
            {
                if(Vector3.Distance(_wolfPack.humanToFollow.position, transform.position) + 0.1f > 
                    Vector3.Distance(hit.collider.transform.position, transform.position))
                {
                    if (hit.collider.transform.parent.parent.GetComponent<NPCController>().canBeAttacked()) _closestHumanToFollow = hit.collider.transform;
                }
                else
                {
                    _closestHumanToFollow = null;
                }
            }
        }
        else
        {
            _closestHumanToFollow = null;
        }
    }


    private IEnumerator AnimationMovement()
    {
        Vector3 lastPosition = transform.position;
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (lastPosition == transform.position) _wolfAnimator.SetBool("Run Forward", false);
            else _wolfAnimator.SetBool("Run Forward", true);

            lastPosition = transform.position;
        }
    }

    void Attack()
    {
        //Debug.Log("Wolf Attack: _closestHumanToFollow= "+ _closestHumanToFollow + " | Time.time - _timeLastAttack < _cooldownAttack= "+ (Time.time - _timeLastAttack < _cooldownAttack)+ " | Vector3.Distance(_closestHumanToFollow.position, transform.position) > _rangeAttack= "+(Vector3.Distance(_closestHumanToFollow.position, transform.position) > _rangeAttack));
        if (_closestHumanToFollow == null) return;
        if (Time.time - _timeLastAttack < _cooldownAttack) return;
        if (Vector3.Distance(_closestHumanToFollow.position, transform.position) > _rangeAttack) return;

        //Debug.Log("Wolf: Attack Human");
        Camera.main.transform.parent.GetComponent<CameraMovement>().MoveToObject(_closestHumanToFollow.gameObject);

        _closestHumanToFollow.parent.parent.GetComponent<ObjectToDestroy>().TakeDamage(295);
        _timeLastAttack = Time.time;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position - transform.forward * 1f, 1f);
    }

    private void OnDestroy()
    {
        Debug.Log("Detect: Je suis un Loup et je suis Mort");
    }


}
