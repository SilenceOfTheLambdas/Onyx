using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

public class ChasePlayer : ActionNode
{
    private                  float        _timer;
    private                  GameObject   _targetObjectToChase;
    private                  NavMeshAgent _navMeshAgent;
    [SerializeField] private float        maxDistance;
    [SerializeField] private float        maxTime;
    protected override void OnStart()
    {
        _targetObjectToChase = FindObjectOfType<Player.Player>().gameObject;
        _navMeshAgent = context.agent;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
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
        
        if (_navMeshAgent.remainingDistance <= 1f)
            return State.Success;
        if (_navMeshAgent.remainingDistance > 1f)
            return State.Running;
        
        return State.Failure;
    }
}
