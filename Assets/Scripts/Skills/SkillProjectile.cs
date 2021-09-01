using System;
using UnityEngine;

namespace Skills
{
    public class SkillProjectile : MonoBehaviour
    {
        [NonSerialized] public   Skill     Skill;
        public                   float     projectileSpeed;
        private                  float     _timer;
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= Skill.skillUseTime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (gameObject.CompareTag("Enemy"))
            {
                if (other.CompareTag("Enemy"))
                    return;
            }
            
            if (gameObject.CompareTag("Player"))
                if (other.CompareTag("Player"))
                    return;
            
            if (other.gameObject.layer == 6)
            {
                GetComponent<SphereCollider>().enabled = false;
                GetComponentInChildren<ParticleSystem>().Clear();
                GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }
}