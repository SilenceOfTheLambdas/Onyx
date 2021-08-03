using Enemies;
using Pathfinding;
using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// Chase a target game object.
    /// </summary>
    public class Chase : IState
    {
        private readonly GameObject  _targetObjectToChase;
        private readonly Enemy       _enemy;
        private          Rigidbody2D _rigidbody2D;
        private readonly Seeker      _seeker;
        private          Path        _path;
        private          int         _currentWaypoint;
        private          float       _timer;

        public Chase(GameObject target, Enemy enemy, Seeker seeker)
        {
            _targetObjectToChase = target;
            _enemy = enemy;
            _seeker = seeker;
        }
        
        public void OnEnter()
        {
            _rigidbody2D = _enemy.GetComponent<Rigidbody2D>();

            _seeker.StartPath(_rigidbody2D.position, _targetObjectToChase.transform.position, OnPathComplete);
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
            if (_seeker.IsDone() && Vector2.Distance(_rigidbody2D.position, _targetObjectToChase.transform.position) <= _enemy.playerDetectionRadius)
            {
                _seeker.StartPath(_rigidbody2D.position, _targetObjectToChase.transform.position, OnPathComplete);
            }
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
            _timer += Time.deltaTime;
            if (_timer >= .5f)
            {
                UpdatePath();
                _timer -= .5f;
            }
            
            if (_path == null)
                return;

            if (_currentWaypoint >= _path.vectorPath.Count)
            {
                return;
            }

            var direction = ((Vector2) _path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
            var force     = direction * _enemy.moveSpeed * Time.deltaTime;

            _rigidbody2D.AddForce(force);

            var distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);
            
            if (distance < _enemy.enemyStoppingDistance)
                _currentWaypoint++;
        }

        public void OnExit()
        {
        }
    }
}