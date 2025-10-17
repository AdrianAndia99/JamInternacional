using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleEntry
{
    public DynamicPooling obstacle;
    [Range(0f, 1f)] public float probability;
}

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private bool spawnPointRotation;
    [SerializeField] private float spawnRate;
    [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();
    [SerializeField] private List<ObstacleEntry> obstacleEntries = new List<ObstacleEntry>();

    private Coroutine spawnObstaclesCoroutine;

    private void Awake()
    {
        for (int i = 0; i < obstacleEntries.Count; i++)
        {
            obstacleEntries[i].obstacle.SetUp();
        }
    }
    public void StartSpawning()
    {
        if (spawnObstaclesCoroutine == null)
        {
            spawnObstaclesCoroutine = StartCoroutine(SpawnObstaclesCycle());
        }
    }

    public void StopSpawning()
    {
        if (spawnObstaclesCoroutine != null)
        {
            StopCoroutine(spawnObstaclesCoroutine);
            spawnObstaclesCoroutine = null;
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        int n = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[n];
    }

    private ObstacleEntry GetRandomObstacle()
    {
        float totalProb = 0f;

        for (int i = 0; i < obstacleEntries.Count; i++)
        {
            totalProb += obstacleEntries[i].probability;
        }

        float randomValue = Random.Range(0f, totalProb);
        float cumulative = 0f;

        for (int i = 0; i < obstacleEntries.Count; i++)
        {
            cumulative += obstacleEntries[i].probability;
            if (randomValue <= cumulative)
            {
                return obstacleEntries[i];
            }
        }

        return obstacleEntries[Random.Range(0, obstacleEntries.Count)];
    }

    private IEnumerator SpawnObstaclesCycle()
    {
        while (true)
        {
            ObstacleEntry selected = GetRandomObstacle();
            Transform selectedPoint = GetRandomSpawnPoint();
            if (spawnPointRotation == false)
            {
                selected.obstacle.GetObject(selectedPoint.position, Quaternion.identity);
            }
            else
            {
                selected.obstacle.GetObject(selectedPoint.position, selectedPoint.rotation);
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }
}