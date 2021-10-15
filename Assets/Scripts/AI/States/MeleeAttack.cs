using Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    public class MeleeAttack
    {
        private float _timer;
        private readonly Enemy _enemy;
        private readonly GameObject _target;
        private readonly Animator _animator;
        private static readonly int Attack = Animator.StringToHash("MeleeAttack");

        public MeleeAttack(Enemy enemy, GameObject target)
        {
            _enemy = enemy;
            _target = target;
            _animator = _enemy.GetComponentInChildren<Animator>();
        }

        public void OnEnter()
        {
            _animator.SetBool(Attack, true);
            _enemy.GetComponent<NavMeshAgent>().enabled = true;
            _enemy.transform.LookAt(_target.transform);
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _enemy.attackRate)
            {
                _animator.SetBool(Attack, true);
                _timer = 0;
            }
        }

        public void FixedTick()
        {
        }

        public void OnExit()
        {
            _animator.SetBool(Attack, false);
        }
    }
}