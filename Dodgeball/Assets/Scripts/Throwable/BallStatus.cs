using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStatus : MonoBehaviour
{
    public bool canPickUp = true;
    public bool canHit;
    public bool hitPlayerTeam;
    public GameObject holder;
    public GameObject pastHolder;
    public GameObject gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager");
    }

    public void ChangeStateToFalse(GameObject h)
    {
        canPickUp = false;
        holder = h;
        pastHolder = holder;
        gameObject.layer = LayerMask.NameToLayer("Not Throwable");
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void CallChangeStateToTrue()
    {
        GetComponent<Rigidbody>().useGravity = true;
        StartCoroutine(ChangeStateToTrue(2));
    }

    public void DeathChangeStateToTrue()
    {
        StartCoroutine(ChangeStateToTrue(0));
    }

    public IEnumerator ChangeStateToTrue(float time)
    {
        yield return new WaitForSeconds(time);
        canHit = false;
        canPickUp = true;
        gameObject.layer = LayerMask.NameToLayer("Throwable");
        GetComponent<Rigidbody>().useGravity = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Good" && hitPlayerTeam && canHit)
        {
            if (canPickUp == false)
            {
                if (collision.gameObject.GetComponent<AiBehavior>().currentBall != null)
                {
                    collision.gameObject.GetComponent<AiBehavior>().currentBall.GetComponent<BallStatus>().DeathChangeStateToTrue();
                }
                gameManager.GetComponent<DeathNotification>().ShowNotification(gameObject, collision.gameObject);
                gameManager.GetComponent<GameStatus>().SetText(--(gameManager.GetComponent<GameStatus>().friendlyCount),
                    gameManager.GetComponent<GameStatus>().enemyCount);
                Destroy(collision.gameObject);
                canHit = false;
            }
        } else if (collision.gameObject.tag == "Player" && hitPlayerTeam && canHit)
            {
           if( collision.gameObject.GetComponent<PlayerMovement>().currentBall != null)
            {
                collision.gameObject.GetComponent<PlayerMovement>().currentBall.GetComponent<BallStatus>().DeathChangeStateToTrue();
            }
            collision.gameObject.GetComponent<PlayerDeath>().Death();
            collision.gameObject.GetComponent<Transform>().position = new Vector3(0, 200, 0);
            gameManager.GetComponent<GameStatus>().SetText(--(gameManager.GetComponent<GameStatus>().friendlyCount),
                    gameManager.GetComponent<GameStatus>().enemyCount);
            canHit = false;
        }
            else if (collision.gameObject.tag == "Bad" && !hitPlayerTeam && canHit)
        {
            if (canPickUp == false)
            {
                if (collision.gameObject.GetComponent<AiBehavior>().currentBall != null)
                {
                    collision.gameObject.GetComponent<AiBehavior>().currentBall.GetComponent<BallStatus>().DeathChangeStateToTrue();
                }
                gameManager.GetComponent<DeathNotification>().ShowNotification(gameObject, collision.gameObject);
                gameManager.GetComponent<GameStatus>().SetText(gameManager.GetComponent<GameStatus>().friendlyCount,
                    --(gameManager.GetComponent<GameStatus>().enemyCount));
                Destroy(collision.gameObject);
                canHit = false;
            }
        } else
        {
            canHit = false;  
        }
        pastHolder = null;
    }
}
