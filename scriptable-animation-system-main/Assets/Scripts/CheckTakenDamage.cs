using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckTakenDamage : Conditional
{
    public PlayerHealth enemyHealth;
    public SharedGameObject player;
    public override TaskStatus OnUpdate()
    {
        
        if (enemyHealth != null && enemyHealth.currentHealth < enemyHealth.maxHealth)
        {
            player.Value = enemyHealth._attackerGameOBJ;
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

    public override void OnReset()
    {
        enemyHealth = null;
        player = null;
    }
}