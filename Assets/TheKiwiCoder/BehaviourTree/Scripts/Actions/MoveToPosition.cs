using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToPosition : ActionNode
{
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart() {
        context.Agent.stoppingDistance = stoppingDistance;
        context.Agent.speed = speed;
        context.Agent.destination = blackboard.moveToPosition;
        context.Agent.updateRotation = updateRotation;
        context.Agent.acceleration = acceleration;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.Agent.pathPending) {
            return State.Running;
        }

        if (context.Agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (context.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return State.Failure;
        }

        return State.Running;
    }
}
