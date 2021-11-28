using System;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Skills
{
    [RequireComponent(typeof(CapsuleCollider), typeof(ParticleSystem))]
    public class BeamManager : MonoBehaviour
    {
        [NonSerialized]  public  Skill      Skill;
        [NonSerialized]  public  Transform  BeamSpawnPoint;
        [SerializeField] private float      rotationSpeed;
        private                  float      _timer;
        private                  Quaternion _originalRotation;

        private void Update()
        {
            Vector3 pos  = Mouse.current.delta.ReadValue();
            var     rotX = pos.x * rotationSpeed * Mathf.Deg2Rad;
            GameManager.Instance.player.GetComponent<PlayerMovement>().UsingBeamSkill = true;
            
            if (BeamSpawnPoint.transform.localEulerAngles.y + rotX < 91f || BeamSpawnPoint.transform.localEulerAngles.y + rotX > 271)
                BeamSpawnPoint.transform.Rotate(Vector3.up, rotX);

            _timer += Time.deltaTime;
            if (_timer >= Skill.skillUseTime)
            {
                BeamSpawnPoint.rotation = _originalRotation;
                GameManager.Instance.player.GetComponent<PlayerMovement>().UsingBeamSkill = false;
                Skill.InCoolDown = true;
                Destroy(gameObject);
            }
        }

        public void SetOriginalRotation(Quaternion originalRotation)
        {
            _originalRotation = originalRotation;
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
            
            if (other.gameObject.layer == LayerMask.GetMask("Terrain"))
            {
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponentInChildren<ParticleSystem>().Clear();
                GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }
}
