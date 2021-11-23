using BehaviorDesigner.Runtime.Tasks;
using Enemies;
using UnityEngine;

namespace AI
{
    [TaskDescription("Is the player object within the enemy's player detection radius?")]
    public class PlayerInDetectionRadius : EnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Vector3.Distance(gameObject.transform.position, GameManager.Instance.player.transform.position)
                <= gameObject.GetComponent<Enemy>().playerDetectionRadius)
                return TaskStatus.Success;
            
            if (Vector3.Distance(gameObject.transform.position, GameManager.Instance.player.transform.position)
                > gameObject.GetComponent<Enemy>().playerDetectionRadius)
                return TaskStatus.Failure;

            return TaskStatus.Running;
        }
    }
}