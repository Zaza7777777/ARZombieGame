using UnityEngine;

public class AlwaysSpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject SpawnObject;
    public float SpawnPeriod = 2f;
    private float Timer;

    void Start()
    {
      
        Timer = SpawnPeriod;

        if (gameObject.name.Contains("(Clone)"))
        {
            this.enabled = false;
        }
    }

    void Update()
    {

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            Spawn();
            Timer = SpawnPeriod;
        }
    }

    void Spawn()
    {
        if (SpawnObject == null || SpawnPoint == null)
        {
            Debug.LogWarning("Spawner is missing a Prefab or SpawnPoint reference!");
            return;
        }

        Instantiate(SpawnObject, SpawnPoint.position, SpawnPoint.rotation);
    }
}