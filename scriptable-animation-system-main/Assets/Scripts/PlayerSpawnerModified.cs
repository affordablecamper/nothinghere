using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

public class PlayerSpawnerModified : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private string spawnPointTag = "SpawnPoint";

    private NetworkManager _networkManager;
    private Transform[] _spawnPoints;

    private void Start()
    {
        _networkManager = InstanceFinder.NetworkManager;
        _spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag).Select(obj => obj.transform).ToArray();
    }

    public void SpawnPlayer()
    {
        if (_networkManager.IsServer)
        {
            // Instantiate the player prefab
            GameObject playerObject = Instantiate(playerPrefab.gameObject);

            // Set the position and rotation of the player object
            Transform spawnPoint = GetRandomSpawnPoint();
            playerObject.transform.position = spawnPoint.position;
            playerObject.transform.rotation = spawnPoint.rotation;

            // Get the NetworkObject component from the player object
            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                // Call the Spawn method on the server manager to spawn the player
                _networkManager.ServerManager.Spawn(networkObject);
            }
            else
            {
                Debug.LogError("Player prefab does not have a NetworkObject component.");
                Destroy(playerObject);
            }
        }
        else if (_networkManager.IsClient)
        {
            // Instantiate the player prefab
            GameObject playerObject = Instantiate(playerPrefab.gameObject);

            // Set the position and rotation of the player object
            Transform spawnPoint = GetRandomSpawnPoint();
            playerObject.transform.position = spawnPoint.position;
            playerObject.transform.rotation = spawnPoint.rotation;

            // Get the NetworkObject component from the player object
            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                // Call the Spawn method on the client manager to spawn the player
                _networkManager.ServerManager.Spawn(networkObject);
            }
            else
            {
                Debug.LogError("Player prefab does not have a NetworkObject component.");
                Destroy(playerObject);
            }
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (_spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found with the specified tag.");
            return transform;
        }

        int randomIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[randomIndex];
    }
}