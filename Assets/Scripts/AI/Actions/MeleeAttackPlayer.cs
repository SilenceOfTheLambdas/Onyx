using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace AI.Actions
{
    // ReSharper disable once UnusedType.Global
    public class MeleeAttackPlayer : EnemyAction
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        public override void OnStart()
        {
            Animator.SetTrigger(Attack);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
        }
    }
}
