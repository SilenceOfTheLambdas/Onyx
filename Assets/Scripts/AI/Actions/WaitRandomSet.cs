using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

namespace AI.Actions
{
    public class WaitRandomSet : ActionNode
    {
        [SerializeField] private List<float> randomDurations;
        private float _duration;
        private float _startTime;
        protected override void OnStart()
        {
            _startTime = Time.time;
            _duration = randomDurations[Random.Range(0, randomDurations.Count)];
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (Time.time - _startTime > _duration)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}
