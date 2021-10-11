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
        [SerializeField] private float        moveToEffectDestroyDistance;
        public                   NavMeshAgent navMeshAgent;
        
        
        [NonSerialized] public bool        UsingBeamSkill;
        private                GameObject  _moveToEffectWorld;
        private                bool        _rewindTime;
        private                Animator    _animator;
        private                InputAction _click;

        private static readonly int Speed = Animator.StringToHash("Speed");

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
            if (Mouse.current.leftButton.isPressed && (!(GetComponent<Player>().inventoryOpen || GetComponent<Player>().skillTreeOpen)) && !UsingBeamSkill)
            {
                #region Move to mouse position
                var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(mRay, out var hit, Mathf.Infinity, cameraLayerMask))
                {

                    // If there is already a move to effect, destroy it
                    var moveToEffectSpawnPoint = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
                    if (_moveToEffectWorld)
                    {
                        Destroy(_moveToEffectWorld);
                        _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);
                    } else
                        _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);
                        
                    navMeshAgent.SetDestination(hit.point);
                }
                #endregion

                #region Move towards enemy, and stop within weapon range
                // If we click on an enemy
                if (SuperuserUtils.SuperuserUtils.Instance.IsTheMouseHoveringOverGameObject(LayerMask.GetMask("Enemy"), out var enemy))
                {
                    if (enemy != null)
                        if (Vector3.Distance(transform.position, enemy.transform.position) <= GameManager.Instance.player.GetComponent<PlayerEquipmentManager>()?.weaponItem.weaponRange)
                            return;
                    
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
                }
                #endregion
            }

            // Animating
            _animator.SetFloat(Speed, navMeshAgent.velocity.magnitude);
            
            UpdateMoveToEffect();
        }

        #region Helper

        private void UpdateMoveToEffect()
        {
            // Destroy Move To Effect when we have reached it
            if (_moveToEffectWorld != null)
            {
                if (Vector3.Distance(transform.position, _moveToEffectWorld.transform.position) <= moveToEffectDestroyDistance)
                    Destroy(_moveToEffectWorld);
            }
        }

        #endregion
    }
}