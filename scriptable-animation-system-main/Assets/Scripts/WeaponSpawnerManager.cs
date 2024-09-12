using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class WeaponSpawnerManager : NetworkBehaviour
{
    private GameManager gameManager;
    private float interval = 120f; // Time interval in seconds (2 minutes)
    public Transform[] weaponSpawnPoints;
    public GameObject[] weaponsToSpawn;

    private List<int> shuffledSpawnPointIndices;
    private int currentSpawnIndex = 0;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        StartCoroutine(SpawnWeaponsRoutine());
    }

    private IEnumerator SpawnWeaponsRoutine()
    {
        // Initial weapon spawn
        yield return new WaitForSeconds(20f);
        FirstWeaponSpawn();

        // Repeat weapon spawn every 2 minutes
        while (true)
        {
            yield return new WaitForSeconds(interval);
            TwoMinutesPassed();
        }
    }

    public void FirstWeaponSpawn()
    {
        ShuffleSpawnPointIndices();
        SpawnWeapon();
        SpawnWeapon();
        SpawnWeapon();
    }

    private void TwoMinutesPassed()
    {
        float elapsedMinutes = gameManager.GetElapsedMinutes();
        if (elapsedMinutes >= 2)
        {
            ShuffleSpawnPointIndices();
            SpawnWeapon();
            SpawnWeapon();
            SpawnWeapon();
        }
    }

    private void SpawnWeapon()
    {
        int randomSpawnPointIndex = GetNextUnusedSpawnPointIndex();
        int randomWeaponIndex = Random.Range(0, weaponsToSpawn.Length);

        GameObject weaponObject = Instantiate(weaponsToSpawn[randomWeaponIndex], weaponSpawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(weaponObject, null);
    }

    private void ShuffleSpawnPointIndices()
    {
        shuffledSpawnPointIndices = new List<int>();
        for (int i = 0; i < weaponSpawnPoints.Length; i++)
        {
            shuffledSpawnPointIndices.Add(i);
        }

        // Fisher-Yates shuffle
        for (int i = shuffledSpawnPointIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = shuffledSpawnPointIndices[i];
            shuffledSpawnPointIndices[i] = shuffledSpawnPointIndices[randomIndex];
            shuffledSpawnPointIndices[randomIndex] = temp;
        }

        currentSpawnIndex = 0;
    }

    private int GetNextUnusedSpawnPointIndex()
    {
        int spawnPointIndex = shuffledSpawnPointIndices[currentSpawnIndex];
        currentSpawnIndex = (currentSpawnIndex + 1) % shuffledSpawnPointIndices.Count;

        return spawnPointIndex;
    }
}