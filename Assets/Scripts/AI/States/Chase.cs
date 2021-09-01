using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    /// <summary>
    /// Chase a target game object.
    /// </summary>
    public class Chase : IState
    {
        private readonly        GameObject   _targetObjectToChase;
        private readonly        Enemy        _enemy;
        private                 Animator     _animator;
        private                 int          _currentWaypoint;
        private                 float        _timer;
        private readonly        NavMeshAgent _navMeshAgent;
        private static readonly int          Speed = Animator.StringToHash("Speed");

        public Chase(GameObject target, Enemy enemy)
        {
            _targetObjectToChase = target;
            _enemy = enemy;
            _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        }
        
        public void OnEnter()
        {
            _animator = _enemy.GetComponentInChildren<Animator>();
            _navMeshAgent.enabled = true;
            _animator.SetFloat(Speed, 1f);
            _navMeshAgent.SetDestination(_targetObjectToChase.transform.position);
        }

        public void Tick()
        {
            _navMeshAgent.SetDestination(_targetObjectToChase.transform.position);
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