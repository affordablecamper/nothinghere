using UnityEngine;
using IngameDebugConsole;

public class ServerSpawnBotCMD : MonoBehaviour
{
    [ConsoleMethod("server.spawnbot", "Spawns a bot at a map SpawnPoint")]
    public static void SpawnBot()
    {
        BotSpawner.FindBotSpawner.SpawnBot();
    }
}