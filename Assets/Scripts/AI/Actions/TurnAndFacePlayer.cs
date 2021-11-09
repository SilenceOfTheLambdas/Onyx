using Enemies;
using TheKiwiCoder;
using UnityEngine;

namespace AI.Actions
{
    public class TurnAndFacePlayer : ActionNode
    {
        private GameObject _targetObjectToChase;
        protected override void OnStart() {
            _targetObjectToChase = FindObjectOfType<Player.Player>().gameObject;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            context.GameObject.transform.LookAt(_targetObjectToChase.transform);
            return State.Success;
        }
    }
}
