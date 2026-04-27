using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject ZombieObject;
    public GameObject GolemObject;

    [Header("Spawn Settings")]
    public float SpawnInterval = 2f;
    public float SpawnDistance = 3f;         
    public float SpawnYOffset = -0.1f;       
    [Range(0f, 1f)]
    public float GolemSpawnChance = 0.25f;

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

        if (GameManager.Instance.CurrentWave != lastWave)
        {
            spawnedThisWave = 0;
            lastWave = GameManager.Instance.CurrentWave;
        }

        int totalThisWave = GameManager.Instance.ZombiesPerWave
                          + (GameManager.Instance.CurrentWave - 1) * 2;
        if (spawnedThisWave >= totalThisWave) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        SpawnEnemy();
        spawnedThisWave++;
        timer = SpawnInterval;
    }

    void SpawnEnemy()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 offset = new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad) * SpawnDistance,
            0,
            Mathf.Cos(angle * Mathf.Deg2Rad) * SpawnDistance
        );

        Vector3 pos = arCamera.transform.position + offset;
        pos.y = arCamera.transform.position.y + SpawnYOffset;  

        bool spawnGolem = GolemObject != null && Random.value < GolemSpawnChance;
        GameObject prefab = spawnGolem ? GolemObject : ZombieObject;

        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);

        float speedBoost = (GameManager.Instance.CurrentWave - 1) * 0.01f;

        if (spawnGolem)
        {
            GolemBehaviour gb = enemy.GetComponent<GolemBehaviour>();
            if (gb != null) gb.MoveSpeed = 0.02f + speedBoost;
        }
        else
        {
            ZombieBehaviour zb = enemy.GetComponent<ZombieBehaviour>();
            if (zb != null) zb.MoveSpeed = 0.03f + speedBoost;
        }

        Debug.Log($"Spawned {(spawnGolem ? "Golem" : "Zombie")} #{spawnedThisWave} at {pos}");
    }
}