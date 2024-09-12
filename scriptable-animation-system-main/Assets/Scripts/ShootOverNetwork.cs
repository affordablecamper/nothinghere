using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

//This is made by Bobsi Unity - Youtube
public class ShootOverNetwork : NetworkBehaviour
{
    public static ShootOverNetwork instance;
    
    private void Awake()
    {
        instance = this;
    }

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    public void _DamagePlayer(int playerID, int damage, int attackerID)
    {
        if (!base.IsServer)
            return;
        


        players[playerID].health -= damage;
        print("Player " + playerID.ToString() + " health is " + players[playerID].health);

        if (players[playerID].health <= 0)
        {
            PlayerKilled(playerID, attackerID);
        }

        
    }

    void PlayerKilled(int playerID, int attackerID)
    {
        print("Player " + playerID.ToString() + " was killed by " + attackerID.ToString());
        players[playerID].deaths++;
        players[playerID].health = 100;

        if (!players[playerID].isBot)
        {
            players[attackerID].kills++;
            RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, spawnPoints.Count));
        }
        else {
            //players[playerID].switching.RemoveAllWeapons();
            RespawnBotPlayer(players[playerID].playerObject, Random.Range(0, spawnPoints.Count));
        }
            


    }








    public float getCurrentHealth(int PlayerID)
    {
        return players[PlayerID].health;
        
    }

    IEnumerator EnablePlayerBotControls(GameObject player)
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        
        player.GetComponent<NavMeshAgent>().enabled = true;
        player.GetComponent<BehaviorTree>().enabled = true;
        



    }



    IEnumerator EnablePlayerControls(GameObject player)
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
       
        player.GetComponent<CharacterController>().enabled = true;
       
           
           
    }

    [ServerRpc(RequireOwnership = false)]
    void RespawnBotPlayer(GameObject player, int spawn)
    {
        player.GetComponent<NavMeshAgent>().enabled = false;
        player.GetComponent<BehaviorTree>().enabled = false;

        player.transform.position = spawnPoints[spawn].position;
        Debug.Log("Respawned Bot and changed location");
        StartCoroutine(EnablePlayerBotControls(player));
    }


    [TargetRpc]
    void RespawnPlayer(NetworkConnection conn, GameObject player, int spawn)
    {
        // Disable network synchronization
        player.GetComponent<CharacterController>().enabled = false;

        // Set the player's position and rotation
        player.transform.position = spawnPoints[spawn].position;
            
        // Re-enable network synchronization after a small delay
        StartCoroutine(EnablePlayerControls(player));
        
        
    }
    public class Player
    {
        public int health = 100;
        public GameObject playerObject;
        public NetworkConnection connection;
        public int kills = 0;
        public int deaths = 0;
        public bool isBot;
        
    }
}