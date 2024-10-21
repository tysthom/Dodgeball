using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catch : MonoBehaviour
{
    public bool hitPlayerTeam;
    public float time = 3;

    public void CatchBall(GameObject ball)
    {
            if (ball.GetComponent<BallStatus>().canHit && gameObject.GetComponent<AiBehavior>().canCatch)
            {
            if (hitPlayerTeam != ball.GetComponent<BallStatus>().hitPlayerTeam)
            {   
                int i = Random.Range(1, 5);
                if (i == 1)
                {
                    gameObject.GetComponent<AiBehavior>().Catch(ball);
                }
                else
                {
                    gameObject.GetComponent<AiBehavior>().canCatch = false;
                    StartCoroutine(CoolDown(time));
                }
            }
           }
         }

    IEnumerator CoolDown(float time)
    {
        gameObject.GetComponent<FieldOfView>().catchBall = null;
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<AiBehavior>().canCatch = true;
    }
}
