using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckLastSeenLocation : Conditional
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The shared vector representing the last seen location.")]
    public SharedVector3 lastSeenLocation;
    public SharedBool canSee;

    public override TaskStatus OnUpdate()
    {
        if (lastSeenLocation.Value != Vector3.zero)
        {
            // lastSeenLocation is equal to Vector3.zero, indicating it is null, return success
            return TaskStatus.Success;
        }
        else
        {
            // lastSeenLocation is not equal to Vector3.zero or can see the player, return failure
            return TaskStatus.Failure;
        }
    }
}