using Enemies;
using Skills;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    /// <summary>
    /// Attack a target using the FlameShot ability
    /// </summary>
    public class FlameShotAttack
    {
        private readonly        Transform    _projectileSpawnPosition;
        private readonly        Skill        _skill;
        private                 GameObject   _projectile;
        private readonly        GameObject   _target;
        private                 Vector3      _direction;
        private                 Enemy        _enemy;
        private                 Animator     _animator;
        private readonly        NavMeshAgent _navMeshAgent;
        private static readonly int          Speed = Animator.StringToHash("Speed");

        public FlameShotAttack(Enemy enemy, GameObject target, Transform projectileSpawnPosition, Skill skill)
        {
            _skill = skill;
            _enemy = enemy;
            _target = target;
            _projectileSpawnPosition = projectileSpawnPosition;
            _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        }
        
        public void OnEnter()
        {
        }

        public void Tick()
        {
            if (!_skill.InCoolDown)
            {
                SpawnProjectile();
            }
            
            if (_projectile != null)
            {
                var projectileSpeed = _projectile.GetComponent<SkillProjectile>().projectileSpeed;
                _skill.SkillTimer += Time.deltaTime;
                if (_skill.SkillTimer >= _skill.coolDownTime)
                {
                    _skill.InCoolDown = false;
                    _skill.SkillTimer = 0;
                }
                _projectile.GetComponent<Rigidbody>().AddForce(_direction * Time.deltaTime * projectileSpeed, ForceMode.Impulse);
            }

            var targetRotation = Quaternion.LookRotation(_target.transform.position - _enemy.transform.position);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }

        public void FixedTick()
        {
        }

        public void OnExit()
        {
        }

        private void SpawnProjectile()
        {
            _skill.InCoolDown = true;
            _skill.HasBeenUsed = true;
            _projectile = Object.Instantiate(_skill.skillEffect, _projectileSpawnPosition);
            _projectile.GetComponent<SkillProjectile>().Skill = _skill;
            _direction = (_projectile.transform.position - new Vector3(_target.transform.position.x, 0, _target.transform.position.z)).normalized;
            
            _projectile.GetComponent<Rigidbody>().AddForce(_enemy.transform.forward * Speed, ForceMode.Impulse);
        }
    }
}