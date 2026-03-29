using UnityEngine;

public class LockedOnSpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject SpawnObject;
    public float SpawnPeriod = 2f;

    private float Timer = 0;

    void Start()
    {
        Timer = SpawnPeriod;
    }

    void Update()
    {
        // 1. Safety Check: If this is a clone, it should NOT be a spawner
        if (gameObject.name.Contains("(Clone)")) return;

        // 2. Proximity Check
        if (ProximityLock.Target != gameObject) return;

        // 3. Game State Check
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        Timer -= Time.deltaTime;
        if (Timer > 0) return;

        // 4. Spawn Logic
        if (SpawnObject != null && SpawnPoint != null)
        {
            Instantiate(SpawnObject, SpawnPoint.position, SpawnPoint.rotation);
        }

        Timer = SpawnPeriod;
    }
}