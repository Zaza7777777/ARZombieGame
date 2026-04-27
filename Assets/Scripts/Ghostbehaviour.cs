using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GhostBehaviour : MonoBehaviour
{
    public float MoveSpeed = 0.05f;
    public float CatchDistance = 0.05f;
    public float CatchCooldown = 2.0f;
    public float SpawnDelay = 3f;
    public float Lifetime = 15f;

    private Transform arCamera;
    private Rigidbody rb;
    private float cooldownTimer = 0f;
    private float spawnTimer = 0f;

    void Start()
    {
        arCamera = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; 
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY
                       | RigidbodyConstraints.FreezeRotationZ;
        Destroy(gameObject, Lifetime);
        spawnTimer = SpawnDelay;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver())
        {
            Destroy(gameObject);
            return;
        }
        if (arCamera == null || rb == null) return;

        if (spawnTimer > 0)
        {
            spawnTimer -= Time.fixedDeltaTime;
            return;
        }

        if (cooldownTimer > 0) cooldownTimer -= Time.fixedDeltaTime;

   
        Vector3 direction = arCamera.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Vector3 newPosition = rb.position + direction.normalized * MoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

        if (Vector3.Distance(transform.position, arCamera.position) < CatchDistance && cooldownTimer <= 0)
        {
            CatchPlayer();
        }
    }

    void CatchPlayer()
    {
        GameManager.Instance.LoseLife();
        Debug.LogWarning("Ghost caught you!");
        cooldownTimer = CatchCooldown;
        Destroy(gameObject);
    }
}