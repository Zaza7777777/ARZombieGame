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
     
        if (gameObject.name.Contains("(Clone)")) return;

        if (ProximityLock.Target != gameObject) return;

 
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        Timer -= Time.deltaTime;
        if (Timer > 0) return;

        
        if (SpawnObject != null && SpawnPoint != null)
        {
            Instantiate(SpawnObject, SpawnPoint.position, SpawnPoint.rotation);
        }

        Timer = SpawnPeriod;
    }
}