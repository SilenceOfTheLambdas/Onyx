using System;
using AI;
using AI.States;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    /// <summary>
    /// A mage uses magic to eliminate enemies.
    /// They have a medium health pool,
    /// Ranged Weapons
    /// </summary>
    public class Mage : MonoBehaviour
    {

        private StateMachine _stateMachine;

        private void Awake()
        {
            var animator = GetComponent<Animator>();
        }

        public Mage()
        {
            
        }
    }
}
