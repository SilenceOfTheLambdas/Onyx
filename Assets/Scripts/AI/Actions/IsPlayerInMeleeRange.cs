using Enemies;
using TheKiwiCoder;
using UnityEngine;

namespace AI.Actions
{
    public class IsPlayerInMeleeRange : ActionNode
    {
        private GameObject _player;
        private Enemy      _enemy;
        private Vector3    _enemyPosition;
        protected override void OnStart()
        {
            _player = FindObjectOfType<Player.Player>().gameObject;
            _enemy = context.Agent.GetComponent<Enemy>();
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate()
        {
            if (_player)
            {
                _enemyPosition = context.Agent.gameObject.transform.position;
                return Vector3.Distance(_player.transform.position, _enemyPosition) <= _enemy.meleeAttackRange ? State.Success : State.Failure;
            }

            if (!_player) return State.Failure;

            return State.Running;
        }
    }
}
