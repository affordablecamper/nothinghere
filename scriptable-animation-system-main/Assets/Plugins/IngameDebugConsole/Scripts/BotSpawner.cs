using UnityEngine;
using FishNet;


public class BotSpawner : MonoBehaviour
{
    public GameObject botPrefab;
    public static BotSpawner FindBotSpawner;
    public Transform[] spawns;
    private void Awake()
    {
        if (FindBotSpawner != null)
        {
            Debug.LogError("More than one BotSpawner instance found!");
            return;
        }

        FindBotSpawner = this;
    }

    public void SpawnBot()
    {
        GameObject bot = Instantiate(botPrefab);
        InstanceFinder.ServerManager.Spawn(bot, null);
        bot.transform.position = spawns[Random.Range(0, spawns.Length -1)].position; 
        
        
    }
}