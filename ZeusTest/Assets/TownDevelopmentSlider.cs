using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownDevelopmentSlider : MonoBehaviour
{
    public static TownDevelopmentSlider instance;
    [SerializeField] private Image[] slidersImages;
    [SerializeField] private Image[] housesImages;
    [SerializeField] private Slider leftSlider;
    [SerializeField] private Slider middleSlider;
    [SerializeField] private GameObject[] crownImages; // 0 is  left, 1 middle, 2 right
    int totalScore = 3;
    List<int> scores = new List<int>() { 1, 1, 1 };
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    private IEnumerator Start()
    {
        //Wait for town to be initialized
        yield return new WaitForSeconds(0.1f);
        for(int i = 0; i < GameManager.instance.Towns.Count; i++)
        {
            Color color = GameManager.instance.Towns[i].GetComponent<TownBehaviour>().townColor;
            slidersImages[i].color = color;
            housesImages[i].color = color;
            crownImages[i].SetActive(false);
        }
    }
    public void UpdateSliders(int townIndex, int score)
    {
        scores[townIndex] = score+1;
        totalScore = scores[0] + scores[1] + scores[2];

        float previousValue = (float)scores[0] / totalScore;
        leftSlider.value = previousValue;
        middleSlider.value = previousValue + (float)scores[1] / totalScore;

        UpdateCrown();
    }

    private void UpdateCrown()
    {
        if (scores[0] >= scores[1] && scores[0] >= scores[2])
        {
            crownImages[0].SetActive(true);
            crownImages[1].SetActive(false);
            crownImages[2].SetActive(false);
        }
        
        else if (scores[1] >= scores[0] && scores[1] >= scores[2])
        {
            crownImages[0].SetActive(false);
            crownImages[1].SetActive(true);
            crownImages[2].SetActive(false);
        }
        else if (scores[2] >= scores[0] && scores[2] >= scores[1])
        {
            crownImages[0].SetActive(false);
            crownImages[1].SetActive(false);
            crownImages[2].SetActive(true);
        }
    }

    public void MoveCameraToTown(int townIndex)
    {
        Camera.main.transform.parent.GetComponent<CameraMovement>().MoveToObject(GameManager.instance.Towns[townIndex]);
    }
}
