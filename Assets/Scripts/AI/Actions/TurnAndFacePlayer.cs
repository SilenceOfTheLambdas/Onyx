using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace AI.Actions
{
    public class TurnAndFacePlayer : EnemyAction
    {
        private GameObject _targetObjectToChase;
        public override void OnStart()
        {
            _targetObjectToChase = GameManager.Instance.player.gameObject;
        }

        public override TaskStatus OnUpdate() {
            gameObject.transform.LookAt(_targetObjectToChase.transform);
            return TaskStatus.Success;
        }
    }
}
