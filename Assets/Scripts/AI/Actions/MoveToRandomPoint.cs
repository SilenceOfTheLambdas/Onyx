using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Actions
{
    // ReSharper disable once UnusedType.Global
    public class MoveToRandomPoint : EnemyAction
    {
        public float RandomDistanceRadius = 2;
        public float StopDistance = 1;
        
        public override TaskStatus OnUpdate()
        {
            NavMeshAgent.SetDestination(Random.insideUnitSphere * RandomDistanceRadius);
            
            if (NavMeshAgent.pathPending)
                return TaskStatus.Running;
            
            if (NavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return TaskStatus.Failure;
            
            if (NavMeshAgent.remainingDistance <= StopDistance)
                return TaskStatus.Success;

            return TaskStatus.Success;
        }
    }
}