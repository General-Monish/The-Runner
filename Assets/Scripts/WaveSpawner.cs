using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WavePawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Enemy[] enemies;
        public int count;
        public float timeBetweenSpawns;
    }
    public Wave[] waves;
    public Transform[] SpawnPoints;
    public float timebetweenwaves;
    private Wave currentWave;
    private int currentwaveIndex;
    private Transform player;

    private bool finishedSpawning;


    public GameObject boss;
    public Transform bossSpawnPoint;
    public GameObject healthBar;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(StartNextWave(currentwaveIndex));
    }

    IEnumerator StartNextWave(int index)
    {
        yield return new WaitForSeconds(timebetweenwaves);
        StartCoroutine(SpawnWave(index));
    }

    IEnumerator SpawnWave(int index)
    {
        currentWave = waves[index];
        for (int i = 0; i < currentWave.count; i++)
        {
            if (player == null)
            {
                yield break;
            }
            Enemy randomenemy = currentWave.enemies[Random.Range(0, currentWave.enemies.Length)];
            Transform randomSpot = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Instantiate(randomenemy, randomSpot.position, randomSpot.rotation);
            if (i == currentWave.count - 1)
            {
                finishedSpawning = true;
            }
            else
            {
                finishedSpawning = false;
            }
            yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
        }
    }

    private void Update()
    {
        if (finishedSpawning == true && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            finishedSpawning = false;
            if (currentwaveIndex + 1 < waves.Length)
            {
                currentwaveIndex++;
                StartCoroutine(StartNextWave(currentwaveIndex));
            }
            else
            {
                Instantiate(boss, bossSpawnPoint.position, bossSpawnPoint.rotation);
                healthBar.SetActive(true);
                Debug.Log("Game over");

            }
        }

    }
}
