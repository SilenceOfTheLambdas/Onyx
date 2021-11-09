using Enemies;
using UnityEngine;

namespace TheKiwiCoder {
    public class Wait : ActionNode {
        public float duration           = 1;
        public bool  useMeleeAttackRate = false;
        float        startTime;

        protected override void OnStart() {
            startTime = Time.time;
            if (useMeleeAttackRate)
                duration = context.Agent.GetComponent<Enemy>().attackRate;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (Time.time - startTime > duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}
