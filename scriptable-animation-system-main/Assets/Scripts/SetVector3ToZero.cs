using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SetVector3ToZero : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The SharedVector3 variable to set to zero.")]
    public SharedVector3 vector3Variable;

    public override TaskStatus OnUpdate()
    {
        vector3Variable.Value = Vector3.zero;
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        vector3Variable = null;
    }
}