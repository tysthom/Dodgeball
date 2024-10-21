using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    public int friendlyCount = 1;
    public int enemyCount = 0;

    public TMP_Text friendlyCountText, enemyCountText;
    public TMP_Text resultsText;

    public void SetText(int fCount, int eCount)
    {
        friendlyCount = Mathf.Max(friendlyCount, 0);
        enemyCount = Mathf.Max(enemyCount, 0);

        friendlyCountText.text = "Friendlies Left: " + friendlyCount;
        enemyCountText.text = "Enemies Left: " + enemyCount;

        StartCoroutine(CheckForWinner());
    }

    IEnumerator CheckForWinner()
    {
        yield return new WaitForSeconds(.5f);

        if (friendlyCount == 0)
        {
            resultsText.gameObject.SetActive(true);
            resultsText.text = "Your team Lost!";
        }
        else if (enemyCount == 0)
        {
            resultsText.gameObject.SetActive(true);
            resultsText.text = "Your team Won!";
        }
    }
}
