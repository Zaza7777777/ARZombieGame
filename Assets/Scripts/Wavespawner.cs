using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject ZombieObject;
    public float SpawnInterval = 2f;
    public float SpawnDistance = 0.5f; // how far in front of camera zombies spawn

    private float timer = 0f;
    private int spawnedThisWave = 0;
    private int lastWave = 0;
    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (!GameManager.Instance.WaveInProgress) return;

        // Reset spawner for new wave
        if (GameManager.Instance.CurrentWave != lastWave)
        {
            spawnedThisWave = 0;
            lastWave = GameManager.Instance.CurrentWave;
        }

        int totalThisWave = GameManager.Instance.ZombiesPerWave + (GameManager.Instance.CurrentWave - 1) * 2;
        if (spawnedThisWave >= totalThisWave) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        // Spawn at random angle around the player at a fixed distance
        float randomAngle = Random.Range(0f, 360f);
        Vector3 offset = new Vector3(
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * SpawnDistance,
            0,
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * SpawnDistance
        );

        // Spawn at camera height so zombie is visible
        Vector3 spawnPosition = arCamera.transform.position + offset;
        spawnPosition.y = arCamera.transform.position.y - 0.1f;

        GameObject zombie = Instantiate(ZombieObject, spawnPosition, Quaternion.identity);

        // Scale speed with wave number
        ZombieBehaviour zb = zombie.GetComponent<ZombieBehaviour>();
        if (zb != null)
        {
            zb.MoveSpeed = 0.03f + (GameManager.Instance.CurrentWave - 1) * 0.01f;
        }

        spawnedThisWave++;
        timer = SpawnInterval;
        Debug.Log("Spawned zombie " + spawnedThisWave + " at " + spawnPosition);
    }
}