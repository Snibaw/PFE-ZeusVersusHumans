    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
    using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] public GameObject planet;
    private float startTime;

    public Context context;
    public GameObject NBResources;

    public List<Vector3> AllPointsOnSphere = new List<Vector3>();
    
    [Header("End Of Level")]
    [SerializeField] private GameObject endOfLevelPanel;
    [SerializeField] private string loseMessageWhenAdorationBarIsZero = "0";
    [SerializeField] private string loseMessageWhenAdorationBarIsFull = "1";

    private void Awake() {
        Application.targetFrameRate = 60;
        
        if(SceneManager.GetActiveScene().name == "MainScene")
        {
            if (!PlayerPrefs.HasKey("Tutorial"))
            {
                SceneManager.LoadScene("Tuto");
                return;
            }
        }
        Time.timeScale = gameSpeed;

        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
        
        startTime = Time.time;
        endOfLevelPanel.SetActive(false);
    }
    
    
    
    //Delete when not needed anymore
    public void SetDamageText(float damage, float life)
    {
        damageText.text = Mathf.Ceil(damage).ToString() + " / " + Mathf.Ceil(life).ToString();
        StartCoroutine(ResetDamageText(0.5f));
    }
    IEnumerator ResetDamageText(float time)
    {
        yield return new WaitForSeconds(time);
        damageText.text = "";
    }

    public void EndGame(bool isBabelTowerBuilt)
    {
        int score = (int)((Time.time - startTime)*1000);
        if (score > PlayerPrefs.GetInt("BestScore",0))
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
        endOfLevelPanel.SetActive(true);
        endOfLevelPanel.GetComponent<EndGamePanel>().ShowEndGamePanel(isBabelTowerBuilt ? loseMessageWhenAdorationBarIsZero : loseMessageWhenAdorationBarIsFull, PlayerPrefs.GetInt("BestScore"), score);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
