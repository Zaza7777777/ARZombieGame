using UnityEngine;

public class PelletBehaviour : MonoBehaviour
{
    // How close the phone needs to be to collect this pellet
    public float CollectDistance = 0.15f;

    // How many points this pellet is worth
    public int ScoreValue = 10;

    // How long before pellet disappears if not collected
    public float Lifetime = 10f;

    // Reference to the AR camera (your phone)
    private Transform arCamera;

    void Start()
    {
        // Find the AR camera so we can measure distance to it
        arCamera = Camera.main.transform;
        Destroy(gameObject, Lifetime);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (arCamera == null) return;

        // Check how far the phone is from this pellet
        float distance = Vector3.Distance(transform.position, arCamera.position);

        // If close enough, collect it
        if (distance < CollectDistance)
        {
            Collect();
        }
    }

    void Collect()
    {
        GameManager.Instance.AddScore(ScoreValue);
        Debug.Log("Pellet collected! +" + ScoreValue);
        Destroy(gameObject);
    }
}