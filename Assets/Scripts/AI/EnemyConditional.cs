using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class EnemyConditional : Conditional
    {
        protected NavMeshAgent NavMeshAgent;
        protected Animator     Animator;

        public override void OnAwake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = gameObject.GetComponentInChildren<Animator>();
        }
    }
}