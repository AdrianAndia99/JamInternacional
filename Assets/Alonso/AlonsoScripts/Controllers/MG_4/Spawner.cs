using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // caramelos y podridos
    public float xRange = 8f;
    public float spawnInterval = 1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        float randomX = Random.Range(-xRange, xRange);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, 0);
        int index = Random.Range(0, objectsToSpawn.Length);
        Instantiate(objectsToSpawn[index], spawnPos, Quaternion.identity);
    }
}

