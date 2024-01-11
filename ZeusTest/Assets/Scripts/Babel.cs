using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Babel : Building
{

    [SerializeField] private int maxLevel = 10;

   public override void levelUp()
   {
        level++;
        if (level == maxLevel) GameOver();
        else level2.transform.GetChild(level).gameObject.SetActive(true);
   }


   private void GameOver()
   {
        GameManager.instance.EndGame(true);
   }
}
