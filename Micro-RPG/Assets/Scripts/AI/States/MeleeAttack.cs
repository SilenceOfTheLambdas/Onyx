using Enemies;
using UnityEngine;

namespace AI.States
{
    public class MeleeAttack : IState
    {
        private          float _timer;
        private readonly Enemy _enemy;

        public MeleeAttack(Enemy enemy)
        {
            _enemy = enemy;
        }
        
        public void OnEnter()
        {
            _enemy.Attack();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _enemy.attackRate)
            {
                _enemy.Attack();
                _timer = 0;
            }
        }

        public void FixedTick()
        {
        }

        public void OnExit()
        {
        }
    }
}