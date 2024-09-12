using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class UpdateStateTask : Action
{
    public EnemyController.StateEnum newState; // Update the data type of the newState field

    private EnemyController enemyController;

    public override void OnAwake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    public override TaskStatus OnUpdate()
    {
        enemyController.ChangeState(newState); // Pass the newState to the ChangeState method of the EnemyController
        return TaskStatus.Success;
    }
}