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
            if (_timer >= 5f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
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
                GetComponent<SpriteRenderer>().sprite = null;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponentInChildren<ParticleSystem>().Clear();
                GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }
}