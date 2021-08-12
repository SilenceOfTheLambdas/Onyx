using System.Collections.Generic;
using Enemies;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.States
{
    /// <summary>
    /// Patrol a random area.
    /// </summary>
    public class Patrol : IState
    {
        private readonly Enemy           _enemy;
        private          Animator        _animator;
        private          float           _patrolRadius;
        private          Rigidbody2D     _rigidbody2D;
        private readonly Seeker          _seeker;
        private readonly List<Transform> _patrolPoints;

        private Path    _path;
        private int     _currentWaypoint  = 0;
        private bool    _reachedEndOfPath = false;
        private Vector2 _currentTargetPosition;
        private float   timer;

        public Patrol(Enemy enemy, Animator animator, Seeker seeker, float patrolRadius, List<Transform> patrolPoints)
        {
            _enemy = enemy;
            _animator = animator;
            _seeker = seeker;
            _patrolRadius = patrolRadius;
            _patrolPoints = patrolPoints;
        }
        
        public void OnEnter()
        {
            _rigidbody2D = _enemy.GetComponent<Rigidbody2D>();

            _seeker.StartPath(_rigidbody2D.position, SelectRandomPositionFromList(), OnPathComplete);
        }

        private Vector2 SelectRandomPositionFromList()
        {
            _currentTargetPosition = _patrolPoints[Random.Range(0, _patrolPoints.Count - 1)].transform.position;
            return _currentTargetPosition;
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                _path = p;
                _currentWaypoint = 0;
            }
        }

        private void UpdatePath()
        {
            if (_seeker.IsDone() && Vector2.Distance(_rigidbody2D.position, _currentTargetPosition) < 0.3f)
            {
                _seeker.StartPath(_rigidbody2D.position, SelectRandomPositionFromList(), OnPathComplete);
            }
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
            timer += Time.deltaTime;
            if (timer >= .5f)
            {
                UpdatePath();
                timer -= .5f;
            }
            
            if (_path == null)
                return;

            if (_currentWaypoint >= _path.vectorPath.Count)
            {
                _reachedEndOfPath = true;
                return;
            }

            _reachedEndOfPath = false;

            var direction = ((Vector2) _path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
            var force     = direction * _enemy.moveSpeed * Time.deltaTime;

            _rigidbody2D.AddForce(force);

            var distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);

            if (distance < 0.3)
                _currentWaypoint++;
        }

        public void OnExit()
        {
        }
    }
}