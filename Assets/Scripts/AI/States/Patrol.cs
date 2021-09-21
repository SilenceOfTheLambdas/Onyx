using System.Collections.Generic;
using Enemies;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI.States
{
    /// <summary>
    /// Patrol a random area.
    /// </summary>
    public class Patrol
    {
        private readonly Enemy           _enemy;
        private          Rigidbody       _rigidbody;
        private readonly Seeker          _seeker;
        private readonly List<Transform> _patrolPoints;
        private readonly NavMeshAgent    _navMeshAgent;

        private Path    _path;
        private Vector3 _currentTargetPosition;
        private float   _timer;

        public Patrol(Enemy enemy, List<Transform> patrolPoints)
        {
            _enemy = enemy;
            _patrolPoints = patrolPoints;
            _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        }
        
        public void OnEnter()
        {
            _rigidbody = _enemy.GetComponent<Rigidbody>();
            _navMeshAgent.SetDestination(SelectRandomPositionFromList());
        }

        public void Tick()
        {
            
        }

        private Vector3 SelectRandomPositionFromList()
        {
            _currentTargetPosition = _patrolPoints[Random.Range(0, _patrolPoints.Count - 1)].transform.position;
            return _currentTargetPosition;
        }

        private void UpdatePath()
        {
            if (Vector3.Distance(_rigidbody.position, _currentTargetPosition) <= 1.5f)
            {
                _navMeshAgent.SetDestination(SelectRandomPositionFromList());
            }
        }

        public void FixedTick()
        {
            _timer += Time.deltaTime;
            if (_timer >= .5f)
            {
                UpdatePath();
                _timer -= .5f;
            }
        }

        public void OnExit()
        {
        }
    }
}