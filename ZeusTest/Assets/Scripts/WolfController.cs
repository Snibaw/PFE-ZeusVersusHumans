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
    

    [SerializeField] private LayerMask _layerMask;

    PointDistribution _pointDistribution;

    private float _timeLastAttack;

    [SerializeField] private float _cooldownAttack;

    [SerializeField] private float _rangeAttack;

    public ObjectToDestroy objectToDestroy;

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
        
        Collider[] colliders = Physics.OverlapSphere(transform.position - transform.forward * 0.5f, 2.5f, _layerMask);
        foreach (Collider collider in colliders)
        {
            //Debug.Log("Detected: "+ hit.collider.tag);
            //Debug.Log("_wolfPack.humanToFollow: " + (_wolfPack.humanToFollow == null)); 
            if (_wolfPack.humanToFollow == null)
            {
                if (collider.transform.parent.parent.gameObject.GetComponent<NPCController>().canBeAttacked()) _wolfPack.HumanToFollow = collider.transform;
            }
            else
            {
                if(Vector3.Distance(_wolfPack.humanToFollow.position, transform.position) + 0.1f > 
                    Vector3.Distance(collider.transform.position, transform.position))
                {
                    if (collider.transform.parent.parent.GetComponent<NPCController>().canBeAttacked()) _closestHumanToFollow = collider.transform;
                }
                else
                {
                    _closestHumanToFollow = null;
                }
            }
        }
        if(colliders.Length == 0) _closestHumanToFollow = null;
        if(colliders.Length >= 3 && _wolfPack._wolfs.Count <= 1)
        {
            _wolfPack.humanToFollow = null;
            _closestHumanToFollow = null;
            _wolfPack.transform.position = transform.position + Random.onUnitSphere*10;
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
        
        StartCoroutine(StopMovingTime(2));
        _closestHumanToFollow.parent.parent.GetComponent<NPCController>().objectToDestroy.TakeDamage(50);
        _wolfAnimator.SetTrigger("Bite Attack");
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

    IEnumerator StopMovingTime(float timeSecond)
    {
        float timeBegin = Time.time;
        while (true)
        {
            if (Time.time - timeBegin > timeSecond)
            {
                break;
            }
            else
            {
                mover.StopMoving();
                yield return new WaitForEndOfFrame();
            }
        }
    }


}
