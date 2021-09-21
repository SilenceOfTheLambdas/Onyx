using TheKiwiCoder;
public class PlayerInSight : ActionNode
{
    private FieldOfView _fieldOfView;
    protected override void OnStart()
    {
        _fieldOfView = context.gameObject.GetComponent<FieldOfView>();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (_fieldOfView.canSeePlayer)
            return State.Success;
        if (!_fieldOfView.canSeePlayer)
            return State.Failure;
        
        return State.Failure;
    }
}
