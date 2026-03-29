using UnityEngine;

public class AlwaysSpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject SpawnObject;
    public float SpawnPeriod = 2f;
    private float Timer;

    void Start()
    {
        // Initialize the timer so it doesn't spawn on the very first frame
        Timer = SpawnPeriod;

        // Safety Check: If this script is accidentally attached to the prefab, 
        // disable it so it doesn't create an infinite loop.
        if (gameObject.name.Contains("(Clone)"))
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        // Don't run the timer if the game is over
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

        // Create the object at the designated spawn point
        Instantiate(SpawnObject, SpawnPoint.position, SpawnPoint.rotation);
    }
}