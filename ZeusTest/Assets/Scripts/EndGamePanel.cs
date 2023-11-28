using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _losingMessage;
    [SerializeField] private TMP_Text _bestScore;
    [SerializeField] private TMP_Text _currentScore;
    
    public void ShowEndGamePanel(string losingMessage, int bestScore, int currentScore)
    {
        _losingMessage.text = losingMessage;
        _bestScore.text = "Best Score: " + bestScore;
        _currentScore.text = "Current Score: " + currentScore;
    }
}
