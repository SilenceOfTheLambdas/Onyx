using BehaviorDesigner.Runtime.Tasks;
using Enemies;
using UnityEngine;

namespace AI.Actions
{
    [TaskDescription("Is the player object within the radius of the enemy's melee range?")]
    public class IsPlayerInMeleeRange : EnemyConditional
    {
        private GameObject _player;
        private Enemy      _enemy;
        private Vector3    _enemyPosition;
        public override void OnStart()
        {
            _player = GameManager.Instance.player.gameObject;
            _enemy = gameObject.GetComponent<Enemy>();
        }
        public override TaskStatus OnUpdate()
        {
            if (_player)
            {
                _enemyPosition = _enemy.gameObject.transform.position;
                return Vector3.Distance(_player.transform.position, _enemyPosition) <= _enemy.meleeAttackRange ? TaskStatus.Success : TaskStatus.Failure;
            }

            if (!_player) return TaskStatus.Failure;

            return TaskStatus.Running;
        }
    }
}
