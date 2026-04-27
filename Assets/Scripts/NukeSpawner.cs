using UnityEngine;

public class NukeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject nukePrefab;
    public float spawnDistance = 2f;
    public float spawnRadius = 1f;

    [Header("Timing")]
    public float minSpawnDelay = 30f;   
    public float maxSpawnDelay = 60f; 

    private GameObject activeNuke;
    private bool waitingToSpawn = false;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (activeNuke == null && !waitingToSpawn)
        {
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        waitingToSpawn = true;
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
        Invoke("SpawnNuke", delay);
    }

    void SpawnNuke()
    {
        if (nukePrefab == null) return;

        Camera cam = Camera.main;
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = cam.transform.position
                         + cam.transform.forward * spawnDistance
                         + cam.transform.right * randomOffset.x
                         + Vector3.up * randomOffset.y;

        activeNuke = Instantiate(nukePrefab, spawnPos, Quaternion.identity);
        waitingToSpawn = false;
    }
}