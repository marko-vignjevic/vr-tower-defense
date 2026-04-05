using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
}

[System.Serializable]
public class Wave
{
     public List<EnemySpawnEntry> enemyGroups;
     public float timeBetweenGroups;
}

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public Wave[] waves;
    public Transform spawnPoint;
    public float timeBetweenWaves = 15.0f;
    private float waveTimer = 0.0f;

    private int currentWave = 0;
    private bool isSpawning = false;

    void Awake() => Instance = this;

    void Start()
    {
        waveTimer = timeBetweenWaves;
    }

    void Update()
    {
        // Automatically start the next wave when the timer
        // runs out and not currently spawning 
        if (isSpawning)
        {
            return;
        }
        if (currentWave >= waves.Length)
        {
            return;
        }

        waveTimer -= Time.deltaTime;
        
        StartNextWave();
    }
    public void StartNextWave()
    {
        // Start the next wave if not currently spawning and there are more waves to spawn
        if (!isSpawning && currentWave < waves.Length)
        {
            isSpawning = true;
            StartCoroutine(StartWaveCoroutine());
        }
    }

    IEnumerator StartWaveCoroutine()
    {
        // Wait for the time between waves before starting the next wave
        if (currentWave > 0)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
        }
        isSpawning = true;
        yield return StartCoroutine(SpawnWave(waves[currentWave]));
    }

    IEnumerator SpawnWave(Wave wave)
    {
        // Spawn all enemy groups in the wave with appropriate delays
        isSpawning = true;

        foreach (EnemySpawnEntry group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                Instantiate(group.enemyPrefab, spawnPoint.position, Quaternion.identity);
                yield return new WaitForSeconds(group.spawnInterval);
            }

            yield return new WaitForSeconds(wave.timeBetweenGroups);
        }

        // Update state to indicate wave is finished and start timer for next wave
        currentWave++;
        isSpawning = false;
        waveTimer = timeBetweenWaves;
    }

    public int GetCurrentWave() 
    {
        return currentWave + 1;
    }
    public bool IsSpawning() 
    {
        return isSpawning;
    }
    public float GetTimeBetweenWaves() 
    {
        return waveTimer;
    }
}
