using Skills;
using UnityEngine;

namespace Player
{
    public class PlayerHitDetection : MonoBehaviour
    {
        private Player _player;
        private AbilitiesSystem _playerAbilitySystem;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();
            _playerAbilitySystem = _player.GetComponent<AbilitiesSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // If we are hit by an enemy projectile skill
            if (other.CompareTag("Enemy") && other.GetComponent<SkillProjectile>())
            {
                var damageModifier = (_playerAbilitySystem.intelligenceElementalDamageIncreaseAmount * _playerAbilitySystem.intelligence);
                _player.TakeDamage(other.GetComponent<SkillProjectile>().Skill.amountOfDamage - damageModifier);
            }
        }
    }
}
