using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] public GameObject planet;

    public List<Vector3> AllPointsOnSphere = new List<Vector3>();

    private void Awake() {
        Application.targetFrameRate = 60;
        Time.timeScale = gameSpeed;

        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
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


}
