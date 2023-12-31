using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    public string Name;
    public float timeToExecute;
    private float _score;

    public float score
    {
        get { return _score; }
        set
        {
            this._score = Mathf.Clamp01(value);
        } 
    }
    public Consideration[] considerations;

    public Transform RequiredDestination { get; protected set; }
    public virtual void Awake()
    {
        score = 0;
    }

    public abstract void Execute(NPCController _npcController);

    public abstract void SetRequiredDestination(NPCController _npcController);
}
