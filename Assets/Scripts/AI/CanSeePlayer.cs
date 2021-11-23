using BehaviorDesigner.Runtime.Tasks;

namespace AI
{
    // ReSharper disable once UnusedType.Global
    public class CanSeePlayer : EnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            return gameObject.GetComponent<FieldOfView>().canSeePlayer ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}