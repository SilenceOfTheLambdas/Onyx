using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    public bool hasALastKnownPosition;
    public Vector3 lastKnownTargetPosition;

    private void Start()
    {
        playerRef = GameManager.Instance.player.gameObject;
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);
        
        while (true)
        {
            yield return waitForSeconds;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        // OverlapSphere returns a list of object, we know that we only have 1 player
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            var target = rangeChecks[0].transform;
            var directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    // we can the player
                    canSeePlayer = true;
                    Quaternion rot = Quaternion.LookRotation(playerRef.transform.position - transform.position, transform.TransformDirection(Vector3.up));
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 3);
                }
                else
                {
                    lastKnownTargetPosition = target.position;
                    hasALastKnownPosition = true;
                    canSeePlayer = false;
                }
            }
            else
            {
                hasALastKnownPosition = false;
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            hasALastKnownPosition = false;
            canSeePlayer = false;
        }
    }
}
