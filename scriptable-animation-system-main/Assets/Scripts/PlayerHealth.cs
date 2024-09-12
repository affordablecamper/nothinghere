using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    
    public float currentHealth;
    public float maxHealth = 100; // change this later
    public GameObject _attackerGameOBJ;
   
    int playerID;
    [ServerRpc(RequireOwnership = false)]
    public void SendHitToServer(int attackerID, int damage)
    {
        PlayerSetup playerSetup = this.GetComponent<PlayerSetup>();
        if (playerSetup.isBot)
        {
            Debug.Log("The Hit was a bot navigating bot pathway");

            
            playerID = this.gameObject.GetInstanceID();
            ShootOverNetwork.instance._DamagePlayer(playerID, damage, attackerID);
        }
        else
        {
            playerID = this.NetworkObject.OwnerId;
            ShootOverNetwork.instance._DamagePlayer(playerID, damage, attackerID);
        }
        _attackerGameOBJ = ShootOverNetwork.instance.players[attackerID].playerObject;
        
    }

    public void Update()
    {
        if (!GetComponent<PlayerSetup>().isBot)
        {
            currentHealth = ShootOverNetwork.instance.getCurrentHealth(this.NetworkObject.OwnerId);
        }
        else
        {
            currentHealth = ShootOverNetwork.instance.getCurrentHealth(gameObject.GetInstanceID());
        }
        

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            int playerID = networkObject.OwnerId;
            int attackerID = this.NetworkObject.OwnerId;
            ShootOverNetwork.instance._DamagePlayer(playerID, 999, attackerID);
        }
    }
}