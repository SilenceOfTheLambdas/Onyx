using BehaviorDesigner.Runtime.Tasks;
using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Actions
{
    // ReSharper disable once UnusedType.Global
    public class ChasePlayer : EnemyAction
    {
        public float maxDistance;
        public float maxTime;
        private float _stopDistance;

        private float      _timer;
        private GameObject _targetObjectToChase;
        private float      _originalStoppingDistance;

        public override void OnStart()
        {
            _targetObjectToChase = GameManager.Instance.player.gameObject;
            _stopDistance = gameObject.GetComponent<Enemy>().meleeAttackRange;
            _originalStoppingDistance = NavMeshAgent.stoppingDistance;
            NavMeshAgent.stoppingDistance = _stopDistance;
        }

        public override TaskStatus OnUpdate()
        {
            _timer -= Time.deltaTime;

            if (_timer < 0.0f)
            {
                var sqDistance = (_targetObjectToChase.transform.position - NavMeshAgent.destination).sqrMagnitude;
                if (sqDistance > maxDistance * maxDistance)
                {
                    NavMeshAgent.SetDestination(_targetObjectToChase.transform.position);
                }
                _timer = maxTime;
            }

            if (NavMeshAgent.pathPending)
                return TaskStatus.Running;

            if (NavMeshAgent.remainingDistance <= _stopDistance)
                return TaskStatus.Success;

            if (NavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return TaskStatus.Failure;

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            NavMeshAgent.stoppingDistance = _originalStoppingDistance;
        }
    }
}
