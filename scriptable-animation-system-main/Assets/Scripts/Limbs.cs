using UnityEngine;



public class Limbs : MonoBehaviour
{
   
    public bool isHips; // add this function because if its hips and gets scaled the whole body disappers when dieing
    public PlayerHealth health;

    
    public void SendHitToServer(int attackerID, int damage)
    {
        health.SendHitToServer(attackerID, damage);   
    }


}