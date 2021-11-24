using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(PlayerEquipmentManager))]
    [RequireComponent(typeof(AbilitiesSystem))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private LayerMask cameraLayerMask;
        [SerializeField] private GameObject pfMoveToEffect;
        [SerializeField] private float moveToEffectDestroyDistance;
        public NavMeshAgent navMeshAgent;


        [NonSerialized] public bool                   UsingBeamSkill;
        private                GameObject             _moveToEffectWorld;
        private                bool                   _rewindTime;
        private                Animator               _animator;
        private                PlayerEquipmentManager _playerEquipmentManager;
        private                AbilitiesSystem        _playerAbilitiesSystem;

        [SerializeField]
        [Tooltip("How quickly the player turns when the attack key is pressed")]
        private float turnDamping;

        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            _playerAbilitiesSystem = GetComponent<AbilitiesSystem>();
        }

        private void Update()
        {
            if (!(GetComponent<Player>().inventoryOpen || GetComponent<Player>().skillTreeOpen) && !UsingBeamSkill)
            {
                // checks to see if we are clicking on terrain or an enemy, and acts accordingly
                if (Mouse.current.leftButton.isPressed && GetComponent<Player>().state != Player.State.Attacking)
                {
                    #region Move to mouse position
                    var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(mRay, out var hit, Mathf.Infinity, cameraLayerMask))
                    {
                        if (_moveToEffectWorld != null)
                            Destroy(_moveToEffectWorld);
                        navMeshAgent.SetDestination(hit.point);
                    }
                    #endregion

                    #region Move towards enemy, and stop within weapon range
                    // If we click on an enemy
                    if (Physics.Raycast(mRay, out var mRaycastHit))
                    {
                        if (!mRaycastHit.collider.CompareTag("Enemy")) return;
                        
                        var enemy = mRaycastHit.collider.gameObject;
                        
                        if (enemy == null) return;

                        var positionToMoveTo = hit.point;
                        if (GameManager.Instance.player.GetComponent<PlayerEquipmentManager>().hasWeaponEquipped)
                        {
                            var weaponRange = GameManager.Instance.player.GetComponent<PlayerEquipmentManager>().weaponItem.weaponRange / 2;
                            positionToMoveTo = enemy.transform.position + new Vector3(weaponRange, 0f, weaponRange);
                        } else {
                            positionToMoveTo = enemy.transform.position + new Vector3(1f, 0f, 1f);
                        }

                        // Only move when the player is NOT attacking
                        if (GetComponent<Player>().state == Player.State.Normal)
                            navMeshAgent.SetDestination(positionToMoveTo);
                    }
                    #endregion
                }
                // If we clicked once, then move AND spawn the move-to effect
                if (Mouse.current.leftButton.wasReleasedThisFrame && GetComponent<Player>().state != Player.State.Attacking)
                {
                    var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(mRay, out var hit, Mathf.Infinity, cameraLayerMask))
                    {

                        // If there is already a move to effect, destroy it
                        var moveToEffectSpawnPoint = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
                        if (_moveToEffectWorld)
                        {
                            Destroy(_moveToEffectWorld);
                            _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);
                        }
                        else
                            _moveToEffectWorld = Instantiate(pfMoveToEffect, moveToEffectSpawnPoint, Quaternion.identity);

                        navMeshAgent.SetDestination(hit.point);
                    }
                }

                // When the player hits the attack button
                if (Mouse.current.rightButton.isPressed && _playerEquipmentManager.hasWeaponEquipped)
                {
                    #region Rotate and face the cursor (in World Space) and stop moving
                    var mRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(mRay, out var hit, Mathf.Infinity))
                    {
                        var lookPosition = hit.point - transform.position;
                        lookPosition.y = 0;
                        var rotation = Quaternion.LookRotation(lookPosition);
                        navMeshAgent.SetDestination(transform.position); // Stop the player from moving
                        _animator.SetFloat("Rotation", Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnDamping).normalized.y);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnDamping);
                    }
                    #endregion
                }
            }

            UpdateMoveToEffect();
        }

        private void LateUpdate()
        {
            // Animating
            _animator.SetFloat(Speed, navMeshAgent.velocity.magnitude);
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
