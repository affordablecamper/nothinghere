using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ResetLastSeenLocation : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The shared vector representing the last seen location.")]
    public SharedBool canSeePlayer;
    
    public override TaskStatus OnUpdate()
    {
        if (canSeePlayer.Value == false)
        {
            // The lastSeenLocation is already true, no need to change it
            
            return TaskStatus.Success;

        }
        
        else
        {
            // The lastSeenLocation is not null, reset it to null
            
            return TaskStatus.Failure;
        }
    }

    public override void OnReset()
    {
        
    }
}