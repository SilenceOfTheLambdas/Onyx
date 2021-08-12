using Skills;
using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// Attack a target using the FlameShot ability
    /// </summary>
    public class FlameShotAttack : IState
    {
        private readonly Transform  _projectileSpawnPosition;
        private readonly Skill      _skill;
        private          GameObject _projectile;
        private readonly GameObject _target;
        private          Vector2    _direction;

        public FlameShotAttack(GameObject target, Transform projectileSpawnPosition, Skill skill)
        {
            _skill = skill;
            _target = target;
            _projectileSpawnPosition = projectileSpawnPosition;
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
            else if (_projectile != null)
            {
                var projectileSpeed = _projectile.GetComponent<SkillProjectile>().projectileSpeed;
                _skill.SkillTimer += Time.deltaTime;
                if (_skill.SkillTimer >= _skill.coolDownTime)
                {
                    _skill.InCoolDown = false;
                    _skill.SkillTimer = 0;
                }
                _projectile.GetComponent<Rigidbody2D>().AddForce(_direction * Time.deltaTime * projectileSpeed, ForceMode2D.Impulse);
            }
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
            _projectile = Object.Instantiate(_skill.skillEffect, _projectileSpawnPosition.position, Quaternion.Euler(new Vector2(0, 0)));
            _projectile.GetComponent<SkillProjectile>().Skill = _skill;
            _direction = -(_projectile.transform.position - new Vector3(_target.transform.position.x, _target.transform.position.y - 0.2f)).normalized;
        }
    }
}