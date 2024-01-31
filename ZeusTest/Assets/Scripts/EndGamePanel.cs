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
        _bestScore.text = IntToTime(PlayerPrefs.GetInt("BestScore"));
        _currentScore.text = IntToTime(currentScore);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private string IntToTime(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        string minutesString = minutes.ToString();
        string secondsString = seconds.ToString();
        if (minutes < 10) minutesString = "0" + minutesString;
        if (seconds < 10) secondsString = "0" + secondsString;
        return minutesString + ":" + secondsString;
    }
}
