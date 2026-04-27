using UnityEngine;

public class PelletBehaviour : MonoBehaviour
{
    
    public float CollectDistance = 0.15f;

    
    public int ScoreValue = 10;

    
    public float Lifetime = 10f;

    
    private Transform arCamera;

    void Start()
    {
        
        arCamera = Camera.main.transform;
        Destroy(gameObject, Lifetime);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (arCamera == null) return;

        
        float distance = Vector3.Distance(transform.position, arCamera.position);

        
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