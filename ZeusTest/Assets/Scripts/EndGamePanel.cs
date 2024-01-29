using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _bestScore;
    [SerializeField] private TMP_Text _currentScore;
    
    public void ShowEndGamePanel(int currentScore)
    {
        _bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
        _currentScore.text = currentScore.ToString();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
