using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiBehavior : MonoBehaviour
{
    [Header("Outside Refrences")]
    public NavMeshAgent agent;
    public GameObject ballHolder;
    public Transform player;

    public LayerMask ground, throwable, enemyTeam;

    [Header("Looking For Ball")]
    public bool idle;
    Vector3 destination;
    bool destinationSet = false; //Checks if destination is set
    bool hasReachedDestination = false; //Checks to see if Ai has reached their destination
    public float ballSearchRange; //Range that Ai can search for balls
    bool ballInRange; //Checks to see if a ball is in range
    bool ballFound = false; //Checks to see if a ball is in range
    bool hasBall; //Cheks to see if Ai has a ball in hand
    float walkRange = 35; //Temporary
    public GameObject futureBall;
    public GameObject currentBall; //Ball that Ai is going to/holding
    Collider[] objectsInRange; //List of all objects ithin range of Ai
    public float pickUpTime = 2; //Time that Ai will take to pick up balls
    bool isResting = false;

    [Header("Heading To Enemy")]
    public float turnSpeed = 2; //Broken
    Vector3 direction; //Direction that Ai will face
    Quaternion rotation; //Rotates the Ai
    public GameObject currentEnemy; //Gameobject of current enemy being targeted
    public string enemyTagNameV1 = "";
    public string enemyTagNameV2 = "";

    [Header("Throwing Ball")]
    public float throwForce = 200; //Force that Ai will throw the ball
    public float enemySearchRange; //Range that Ai can search for enemies
    bool enemyInRange; //Checks to see if an enemyw is in range
    public Collider[] enemiesInRange; //List of enemeis in a certain radius
    bool lookAtEnemy = false; //Commands Ai to look at their target when throwing the ball
    bool throwing;
    public bool hitPlayerTeam;

    [Header("Catching Ball")]
    public bool canCatch = true;

    [Header("Functions")]
    public bool canA = true;
    public bool canB = true;
    public bool canC = true;
    public bool canD = true;
    public bool canE = true;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        ballHolder = gameObject.transform.GetChild(0).gameObject;
        hasBall = false;
    }

    private void Start()
    {
        if(!idle)
        SetDestination();

        NextStep();
    }

    void Update()
    {
        if (idle) { return; }
        ballInRange = Physics.CheckSphere(transform.position, ballSearchRange, throwable);
        enemyInRange = Physics.CheckSphere(transform.position, enemySearchRange, enemyTeam); //Checks if member of player's team is in range
        hasReachedDestination = agent.remainingDistance <= agent.stoppingDistance;

        if (currentBall != null)
        {
            canCatch = false;
            if (Check() == false)
            {
                StartCoroutine(ResetAi(0));
            }
        }

        NextStep();
    }

    void NextStep()
    {
        if (!isResting)
        {
            if (lookAtEnemy)
            {
                if (currentEnemy != null)
                {
                    Quaternion rotTarget = Quaternion.LookRotation(currentEnemy.transform.position - transform.position);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, turnSpeed * Time.deltaTime);
                }
            }

            if (hasBall == false)
            {
                if (ballInRange == false)
                {
                    LookFor();

                }
                else
                {
                    StartCoroutine(GoToBall());
                }
            }
            else
            {
                if (currentBall != null)
                {
                    currentBall.transform.position = ballHolder.transform.position;

                }

                if (enemyInRange == false)
                {

                    LookFor();
                }
                else
                {
                    GoToEnemy();
                }
            }
        }
        else
        {
            LookFor();
            StartCoroutine(Resting());
        }
    }

    void SetDestination()
    {
        float randomZ = Random.Range(-walkRange, walkRange);
        float randomX = Random.Range(-walkRange, walkRange);

        destination = new Vector3(randomX, transform.position.y, randomZ);

        destinationSet = true;
        hasReachedDestination = false;
    }

    void LookFor()
    {
        if (!destinationSet)
        {
            SetDestination();
        }
        else
        {
            agent.SetDestination(destination);
        }

        Vector3 distanceToDestination = transform.position - destination;

        if (hasReachedDestination)
        {
            destinationSet = false;
        }
    }

    IEnumerator GoToBall() //A
    {
        if (ballFound == false)
        {
            objectsInRange = Physics.OverlapSphere(transform.position, ballSearchRange);
            int i = 0;
            int g = -1;
            while (i < objectsInRange.Length) //Goes through every object within the radius
            {
                if (objectsInRange[i].tag == "Throwable" && objectsInRange[i].gameObject.layer == 8) //Checks to see if object is a ball
                {
                    g = i;
                }
                i++;
            }
            if (g != -1)
            {
                if (objectsInRange[g].GetComponent<BallStatus>().canPickUp)
                {
                    futureBall = objectsInRange[g].gameObject;
                    agent.SetDestination(futureBall.transform.position);
                    ballFound = true;
                }
                else
                {
                    futureBall = null;

                }
            }
            else
            {
                futureBall = null;
            }
        }
        else
        {
            if (futureBall != null)
            {
                if (futureBall.GetComponent<BallStatus>().canPickUp)
                {
                    if (Vector3.Distance(transform.position, futureBall.transform.position) <= ballSearchRange)
                    {//Makes sure that ball is still in range
                        agent.SetDestination(futureBall.transform.position);
                        yield return new WaitUntil(() => hasReachedDestination);
                        PickUpBall();
                    }
                    else
                    {
                        futureBall = null;
                        ballFound = false;
                    }
                }
                else
                {
                    futureBall = null;
                    ballFound = false;
                }

            }
        }
    }

    void PickUpBall() //B
    {

        if (futureBall != null && futureBall.GetComponent<BallStatus>().canPickUp && hasBall == false)
        {
            hasBall = true;
            currentBall = gameObject.GetComponent<AiBehavior>().futureBall;
            currentBall.GetComponent<BallStatus>().holder = gameObject;
            //Debug.Log(currentBall.name);
            currentBall.transform.position = ballHolder.transform.position;
            currentBall.GetComponent<Rigidbody>().useGravity = false;
            currentBall.GetComponent<BallStatus>().ChangeStateToFalse(gameObject);         
            futureBall = null;
            canB = false;
        }
        else
        {
            //StartCoroutine(ResetAi(0));
        }
    }


    void GoToEnemy() //C
    {
        enemiesInRange = Physics.OverlapSphere(transform.position, enemySearchRange);
        int i = 0;
        while (i < enemiesInRange.Length) //Goes through every object within the radius
        {
            if (enemiesInRange[i].tag == enemyTagNameV1 || (enemiesInRange[i].tag == enemyTagNameV2))
            {
                currentEnemy = enemiesInRange[i].gameObject;
                enemyInRange = true;
            }
            i++;
        }

        if (enemyInRange && throwing == false)
        {
            agent.isStopped = true;
            StartCoroutine(Throw());
        }
    }

    IEnumerator Throw() //D
    {
        throwing = true;
        if (Check())
        {
            lookAtEnemy = true;
            yield return new WaitForSeconds(1);
            if (currentBall != null)
            {
                currentBall.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * throwForce);
                currentBall.GetComponent<BallStatus>().hitPlayerTeam = hitPlayerTeam;
                currentBall.GetComponent<BallStatus>().canHit = true;
                currentBall.GetComponent<BallStatus>().CallChangeStateToTrue();
                currentBall.GetComponent<BallStatus>().holder = null;
                currentBall = null;
                StartCoroutine(ResetAi(1));
            }
        }
    }

    public void Catch(GameObject ball) //E
    {
        canCatch = false;
        currentBall = ball;
        currentBall.transform.position = ballHolder.transform.position;
        currentBall.GetComponent<Rigidbody>().useGravity = false;
        currentBall.GetComponent<BallStatus>().ChangeStateToFalse(gameObject);
        currentBall.GetComponent<BallStatus>().holder = gameObject;
        futureBall = null;
        hasBall = true;
    }

    bool Check() //Checks to make sure that the ball is assigned to this ai only
    {
        if(Object.ReferenceEquals(currentBall.GetComponent<BallStatus>().holder, gameObject))
        {
            return true;        
        }
        else
        {
            return false;
        }
    }

    IEnumerator ResetAi(float time)
    {

        futureBall = null;
        currentBall = null;
        yield return new WaitForSeconds(time);
        canCatch = true;
        currentEnemy = null;
        hasBall = false;
        ballFound = false;
        enemyInRange = false;
        lookAtEnemy = false;
        agent.isStopped = false;
        destinationSet = false;
        isResting = true;
        throwing = false;
        canB = true;
    }

    IEnumerator Resting()
    {
        yield return new WaitUntil(() => hasReachedDestination);
        isResting = false;
        NextStep();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, enemySearchRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, ballSearchRange);
    }
}