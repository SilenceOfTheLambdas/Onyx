using System;
using UnityEngine;

namespace Skills
{
    public class SkillProjectile : MonoBehaviour
    {
        [NonSerialized] public Skill Skill;
        public                 float projectileSpeed;
        private                float _timer;
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= 5f)
            {
                Destroy(gameObject);
            }
        }
    }
}