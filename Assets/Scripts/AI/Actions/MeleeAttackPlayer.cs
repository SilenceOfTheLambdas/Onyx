using TheKiwiCoder;
using UnityEngine;

namespace AI.Actions
{
    public class MeleeAttackPlayer : ActionNode
    {
        private                 Animator _animator;
        private static readonly int      Attack = Animator.StringToHash("MeleeAttack");
        protected override void OnStart()
        {
            _animator = context.Agent.gameObject.GetComponentInChildren<Animator>();
            _animator.SetBool(Attack, true);
        }

        protected override void OnStop() 
        {
        }

        protected override State OnUpdate()
        {
            _animator.SetBool(Attack, true);
            return State.Success;
        }
    }
}
