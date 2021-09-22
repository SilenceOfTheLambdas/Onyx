using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private LayerMask    cameraLayerMask;
        [SerializeField] private GameObject   pfMoveToEffect;
        private                  GameObject   _moveToEffectWorld;
        private                  bool         _rewindTime;
        private                  Animator     _animator;
        public                   NavMeshAgent navMeshAgent;

        private static readonly int         VelocityZ = Animator.StringToHash("VelocityZ");
        private static readonly int         VelocityX = Animator.StringToHash("VelocityX");
        private                 InputAction _click;
        [NonSerialized] public  bool        UsingBeamSkill;
        private static readonly int         Speed = Animator.StringToHash("Speed");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            _click = new InputAction(binding: "<Mouse>/leftButton");
            _click.performed += ctx =>
            {
                var mRay = Camera.main.ScreenPointToRay(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
                if (Physics.Raycast(mRay, out var mRaycastHit, Mathf.Infinity, cameraLayerMask))
                {
                    if (!navMeshAgent.Raycast(mRaycastHit.point, out var hit))
                    {
                        // If there is already a move to effect, destroy it
                        var moveToEffectSpawnPoint = new Vector3(mRaycastHit.point.x, mRaycastHit.point.y + 0.01f, mRaycastHit.point.z);
                        if (_moveToEffectWorld)
                        {
                            Destroy(_moveToEffectWorld);
                        } else
                            _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);
                        
                        navMeshAgent.SetDestination(mRaycastHit.point);
                    }
                }
            };
            _click.Enable();
        }
        
        private void Update()
        {
            // checks to see if we are clicking on terrain or an enemy, and acts accordingly
            if (Mouse.current.leftButton.isPressed && !(GetComponent<Player>().inventoryOpen || GetComponent<Player>().skillTreeOpen) && !UsingBeamSkill)
            {
                var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(mRay, out var hit, Mathf.Infinity, cameraLayerMask))
                {
                    // if (!navMeshAgent.Raycast(hit.point, out var hitOutside) &&
                    //     GetComponent<Player>().state == Player.State.Normal)
                    // {
                        Debug.Log("Hey");
                        // If there is already a move to effect, destroy it
                        var moveToEffectSpawnPoint = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
                        if (_moveToEffectWorld)
                        {
                            Destroy(_moveToEffectWorld);
                            _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);
                        } else
                            _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);
                        
                        navMeshAgent.SetDestination(hit.point);
                    // }
                }
                
                // If we click on an enemy
                if (Physics.Raycast(mRay, out hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
                {
                    var positionToMoveTo = hit.point;
                    // If the player has a weapon equipped
                    if (GameManager.Instance.player.GetComponent<PlayerEquipmentManager>().weaponItem != null)
                    {
                        var weaponRange = GameManager.Instance.player.GetComponent<PlayerEquipmentManager>().weaponItem
                            .weaponRange / 2;
                        positionToMoveTo -= new Vector3(weaponRange - 0.2f, 0f, weaponRange - 0.2f);
                    }
                    if (GameManager.Instance.player.GetComponent<PlayerEquipmentManager>().weaponItem == null)
                    {
                        positionToMoveTo -= new Vector3(0.5f, 0, 0.5f);
                    }
                    
                    // Only move when the player is NOT attacking
                    if (GetComponent<Player>().state == Player.State.Normal)
                        navMeshAgent.SetDestination(positionToMoveTo);

                    // Destroy Move To Effect when we have reached it
                    if (_moveToEffectWorld)
                    {
                        if (Vector3.Distance(transform.position, _moveToEffectWorld.transform.position) <= 1f)
                            Destroy(_moveToEffectWorld);
                    }

                }
            }

            // Animating
            var velocityZ = Vector3.Dot(navMeshAgent.velocity.normalized, transform.forward);
            var velocityX = Vector3.Dot(navMeshAgent.velocity.normalized, transform.right);
            
            _animator.SetFloat(VelocityZ, velocityZ, 0.1f, Time.deltaTime);
            _animator.SetFloat(VelocityX, velocityX, 0.1f, Time.deltaTime);
            
            _animator.SetFloat(Speed, navMeshAgent.velocity.magnitude);
        }
    }
}
