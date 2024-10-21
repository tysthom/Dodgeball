using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathNotification : MonoBehaviour
{
    public void ShowNotification(GameObject ball, GameObject receiver)
    {
        if (ball != null && receiver != null)
        {
            if (ball.GetComponent<BallStatus>().pastHolder != null)
            {
                Debug.Log(ball.GetComponent<BallStatus>().pastHolder.name + " hit " + receiver.name);
            }
        }
    }
}
