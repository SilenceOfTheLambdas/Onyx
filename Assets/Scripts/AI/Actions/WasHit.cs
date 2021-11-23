using BehaviorDesigner.Runtime.Tasks;
using Enemies;

namespace AI.Actions
{
    public class WasHit : EnemyAction
    {
        private Enemy _enemy;
        
        public override void OnStart()
        {
            _enemy = NavMeshAgent.gameObject.GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_enemy.wasHit)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
