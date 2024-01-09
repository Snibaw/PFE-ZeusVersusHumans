using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : MonoBehaviour
{
    private MoveController mover;
    private NPCController npcController;
    private int _energy;
    public int energy
    {
        get { return _energy; }
        set
        {
            if(_energy <= 0 && value > 0)
            {
                mover.StopMoving();
                StartCoroutine(npcController.ExecuteAction("Sleep", 10f));
            }
            
            _energy = Mathf.Clamp(value, 0, 100);
            mover.AdaptSpeedToEnergy(_energy);
            OnStatValueChanged?.Invoke();
        }
    }

    private int _hunger;
    public int hunger
    {
        get { return _hunger; }
        set
        {
            _hunger = Mathf.Clamp(value, 0, 100);
            OnStatValueChanged?.Invoke();
        }
    }

    private int _resource;
    public int resource
    {
        get { return _resource; }
        set
        {
            _resource = value;
            OnStatValueChanged?.Invoke();
        }
    }

    [SerializeField] private float timeToDecreaseHunger = 5f;
    [SerializeField] private float timeToDecreaseEnergy = 5f;
    private float timeLeftEnergy;
    private float timeLeftHunger;

    [SerializeField] private Billboard billboard;

    public delegate void StatValueChangedHandler();
    public event StatValueChangedHandler OnStatValueChanged;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MoveController>();
        npcController = GetComponent<NPCController>();
        // Test case: NPC will likely work
        hunger = 0;
        energy = 100;
        resource = 0;
    }

    private void OnEnable()
    {
        OnStatValueChanged += UpdateDisplayText;
    }

    private void OnDisable()
    {
        OnStatValueChanged -= UpdateDisplayText;
    }

    public void UpdateHunger()
    {
        if (timeLeftHunger > 0)
        {
            timeLeftHunger -= Time.deltaTime;
            return;
        }

        timeLeftHunger = timeToDecreaseHunger;
        hunger += 1;
    }

    public void UpdateEnergy(bool shouldNotUpdateEnergy)
    {
        if (shouldNotUpdateEnergy)
        {
            return;
        }

        if (timeLeftEnergy > 0)
        {
            timeLeftEnergy -= Time.deltaTime;
            return;
        }

        timeLeftEnergy = timeToDecreaseEnergy;
        energy -= 1;
    }

    void UpdateDisplayText()
    {
        billboard.UpdateStatsText(energy, hunger, resource);
    }
    
}
