using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

public class ChasePlayer : ActionNode
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxTime;
    [SerializeField] private float stopDistance;
    private float _timer;
    private GameObject _targetObjectToChase;
    private NavMeshAgent _navMeshAgent;
    protected override void OnStart()
    {
        _targetObjectToChase = FindObjectOfType<Player.Player>().gameObject;
        _navMeshAgent = context.agent;
        _navMeshAgent.stoppingDistance = stopDistance;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0.0f)
        {
            var sqDistance = (_targetObjectToChase.transform.position - _navMeshAgent.destination).sqrMagnitude;
            if (sqDistance > maxDistance * maxDistance)
            {
                _navMeshAgent.SetDestination(_targetObjectToChase.transform.position);
            }
            _timer = maxTime;
        }

        if (_navMeshAgent.pathPending)
            return State.Running;

        if (_navMeshAgent.remainingDistance <= stopDistance)
            return State.Success;

        if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            return State.Failure;

        return State.Running;
    }
}
