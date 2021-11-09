using System;
using Enemies;
using TheKiwiCoder;

namespace AI.Actions
{
    public class WasHit : ActionNode
    {
        private Enemy _enemy;
        
        protected override void OnStart()
        {
            _enemy = context.Agent.GetComponent<Enemy>();
        }

        protected override void OnStop() 
        {
        }

        protected override State OnUpdate()
        {
            if (_enemy.wasHit)
            {
                return State.Success;
            }
            return State.Failure;
        }
    }
}
