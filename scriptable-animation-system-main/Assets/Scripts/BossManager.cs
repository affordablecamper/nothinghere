using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public bool bossEventStarted;
    public GameObject[] enemyTypes;
    public GameObject[] bossType;
    public float cooldownTime = 0f;
    public int maxEnemyCount = 5;
    public Transform[] spawnPoints;

    private int currentEnemyCount = 0;

    // Update is called once per frame
    void Update()
    {
        if (bossEventStarted)
        {
            StartBossEvent();
            SpawnEnemyTypes();
        }
    }

    private void StartBossEvent()
    {
        foreach (var boss in bossType)
        {
            boss.SetActive(true);
        }
    }

    private void SpawnEnemyTypes()
    {
        if (currentEnemyCount >= maxEnemyCount)
        {
            return;
        }

        cooldownTime -= Time.deltaTime;
        if (cooldownTime <= 0f)
        {
            InstantiateRandomEnemyType();
            currentEnemyCount++;
            ResetCooldownTime();
        }
    }

    private void InstantiateRandomEnemyType()
    {
        if (enemyTypes.Length == 0)
        {
            Debug.LogError("No enemy types defined. Add at least one enemy type to the enemyTypes array.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, enemyTypes.Length);
        GameObject randomEnemyType = enemyTypes[randomIndex];

        // Choose a random spawn point
        int randomSpawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform randomSpawnPoint = spawnPoints[randomSpawnPointIndex];

        // Instantiate the random enemy type at the chosen spawn point
        Instantiate(randomEnemyType, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }

    private void ResetCooldownTime()
    {
        // Generate a new cooldown time for spawning the next enemy
        cooldownTime = UnityEngine.Random.Range(1f, 5f);
    }
}