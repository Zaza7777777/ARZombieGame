using UnityEngine;

public class ZombieBehaviour : MonoBehaviour
{
    public float MoveSpeed = 0.05f;
    public float CatchDistance = 0.05f;
    public float CatchCooldown = 2.0f;
    public float SpawnDelay = 3f;
    public float Lifetime = 30f;
    public int Health = 1;

    private Transform arCamera;
    private float cooldownTimer = 0f;
    private float spawnTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        arCamera = Camera.main.transform;
        Destroy(gameObject, Lifetime);
        spawnTimer = SpawnDelay;
    }

    void Update()
    {
        if (isDead) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) { Destroy(gameObject); return; }
        if (arCamera == null) { arCamera = Camera.main.transform; return; }

        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            return;
        }

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        Vector3 direction = arCamera.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            // Move toward player
            transform.position += direction.normalized * MoveSpeed * Time.deltaTime;

            // Face toward player, keeping upright
            Quaternion targetRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // Catch player if close enough
        if (Vector3.Distance(transform.position, arCamera.position) < CatchDistance && cooldownTimer <= 0)
        {
            CatchPlayer();
        }
    }

    public void TakeHit()
    {
        if (isDead) return;
        Health--;
        if (Health <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        GameManager.Instance.ZombieKilled();
        Debug.Log("Zombie killed!");
        Destroy(gameObject);
    }

    void CatchPlayer()
    {
        GameManager.Instance.LoseLife();
        Debug.LogWarning("Zombie caught you!");
        cooldownTimer = CatchCooldown;
       
    }
}