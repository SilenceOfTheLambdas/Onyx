using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    public class WanderState
    {
        private readonly        Enemy           _enemy;
        private                 Animator        _animator;
        private                 NavMeshAgent    _navMeshAgent;
        private                 List<Transform> _patrolPoints;
        private                 int             _index;
        private                 float           _timer;
        private static readonly int             Speed = Animator.StringToHash("Speed");

        public WanderState(Enemy enemy)
        {
            _enemy = enemy;
        }
        
        public void OnEnter()
        {
            _patrolPoints = _enemy.PatrolPoints;
            _animator = _enemy.GetComponentInChildren<Animator>();
            _navMeshAgent = _enemy.gameObject.GetComponent<NavMeshAgent>();
            _navMeshAgent.enabled = true;
            _animator.SetFloat(Speed, 1f);
        }

        public void Tick()
        {
            if (_navMeshAgent.remainingDistance < 1f)
            {
                var nextDestination = GetNextDestination();
                _navMeshAgent.SetDestination(nextDestination);
            }
        }

        private Vector3 GetNextDestination()
        {
            _index++;
            if (_index >= _patrolPoints.Count)
                _index = 0;

            return _patrolPoints[_index].position;
        }

        public void FixedTick()
        {
        }

        public void OnExit()
        {
            _navMeshAgent.enabled = false;
            _animator.SetFloat(Speed, 0f);
        }
    }
}