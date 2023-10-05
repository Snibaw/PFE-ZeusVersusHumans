using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IADecisionnel : MonoBehaviour
{
    [SerializeField] private int _nbDecisionTook;
    private List<Decision> _decisionTook;
    [SerializeField] private DecisionRate[] _decisionRates;



    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        _decisionTook = new List<Decision>();
    }

    public void TakingAllDecision()
    {
        _decisionTook.Clear();
        for (int i = 0; i < _nbDecisionTook; i++)
        {
            Decision decision = TakingOneDecision();
            _decisionTook.Add(decision);
            Debug.Log("Decision " + i + " = " + decision);
        }
    }

    private Decision TakingOneDecision()
    {
        int totalRate = 0;

        foreach (var decisionRate in _decisionRates)
        {
            totalRate += decisionRate.GetPonderation();
        }

        int rateChoose = Random.Range(0,totalRate);
        int searchDecision = 0;

        foreach (var decisionRate in _decisionRates)
        {
            searchDecision += decisionRate.GetPonderation();
            if (rateChoose <= searchDecision - 1) return decisionRate.GetDecision();
        }

        return Decision.None;
        
    }



}


[System.Serializable]
struct DecisionRate
{
    [SerializeField] private Decision decision;
    [SerializeField] private int ponderation;

    public DecisionRate(Decision decision, int ponderation)
    {

        this.decision = decision;
        if (ponderation >= 0) this.ponderation = ponderation;
        else this.ponderation = 0;
    }

    public Decision GetDecision()
    {
        return decision;
    }

    public int GetPonderation()
    {
        return ponderation;
    }

    public void SetDecision(Decision decision)
    {
        this.decision = decision;
    }

    public void SetPonderation(int ponderation)
    {
        if (ponderation >= 0) this.ponderation = ponderation;
        else this.ponderation = 0;
    }

    public void SetDecisionRate(Decision decision, int ponderation)
    {
        SetDecision(decision);
        SetPonderation(ponderation);
    }
}

[System.Serializable]
enum Decision
{
    None,
    Proteger,
    Attaquer,
    Explorer,
}
