using UnityEngine;

public class MedKitSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject medKitPrefab;
    public float spawnDistance = 2f;
    public float spawnDelay = 15f;
    public float spawnRadius = 1f;

    private GameObject activeMedKit;
    private bool waitingToSpawn = false;

    void Start()
    {
        SpawnMedKit();
    }

    void Update()
    {
        if (activeMedKit == null && !waitingToSpawn)
        {
            waitingToSpawn = true;
            Invoke("SpawnMedKit", spawnDelay);
        }
    }

    void SpawnMedKit()
    {
        if (medKitPrefab == null) return;

        Camera cam = Camera.main;

        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = cam.transform.position
                         + cam.transform.forward * spawnDistance
                         + cam.transform.right * randomOffset.x
                         + Vector3.up * randomOffset.y;

        activeMedKit = Instantiate(medKitPrefab, spawnPos, Quaternion.identity);
        waitingToSpawn = false;
    }
}