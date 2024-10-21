using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float catchRange;
    [Range(0,360)]
    public float viewAngel;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public GameObject catchBall;

    bool canCatch;

    private void Start()
    {
        StartCoroutine(callFunction());
    }

    IEnumerator callFunction()
    {
        yield return new WaitForSeconds(0);
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, catchRange, targetMask);
        for(int i=0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngel / 2)
            {
                float disToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, target.position, disToTarget, obstacleMask)) //No obstacles in the way
                {
                    canCatch = true;
                    catchBall = target.gameObject;
                }
            }
        }
        if (canCatch)
        {
            gameObject.GetComponent<Catch>().CatchBall(catchBall);
            canCatch = false;
        }
        StartCoroutine(callFunction());
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
