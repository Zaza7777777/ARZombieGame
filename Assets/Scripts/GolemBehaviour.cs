using UnityEngine;
using System.Collections;

public class GolemBehaviour : MonoBehaviour
{
    public float MoveSpeed = 0.025f;       
    public float CatchDistance = 0.05f;
    public float CatchCooldown = 3.0f;
    public float SpawnDelay = 3f;
    public float Lifetime = 45f;
    public int Health = 15;              
    public int LivesDamage = 2;          
    public bool isDead = false;

    [Header("Audio")]
    public AudioClip growlClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    private AudioSource audioSource;
    private Transform arCamera;
    private float cooldownTimer = 0f;
    private float spawnTimer = 0f;

    void Start()
    {
        arCamera = Camera.main.transform;
        Destroy(gameObject, Lifetime);
        spawnTimer = SpawnDelay;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 10f;
        audioSource.playOnAwake = false;

        if (growlClip != null)
            audioSource.PlayOneShot(growlClip);

        StartCoroutine(GrowlRoutine());
    }

    IEnumerator GrowlRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(Random.Range(10f, 15f));
            if (!isDead && growlClip != null)
                audioSource.PlayOneShot(growlClip);
        }
    }

    void Update()
    {
        if (isDead) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) { Destroy(gameObject); return; }
        if (arCamera == null) { arCamera = Camera.main.transform; return; }

        if (spawnTimer > 0) { spawnTimer -= Time.deltaTime; return; }
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        Vector3 direction = arCamera.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.position += direction.normalized * MoveSpeed * Time.deltaTime;

            Quaternion targetRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);
        }

        if (Vector3.Distance(transform.position, arCamera.position) < CatchDistance && cooldownTimer <= 0)
            CatchPlayer();
    }

    public void TakeHit()
    {
        if (isDead) return;
        Health--;
        if (hitClip != null) audioSource.PlayOneShot(hitClip);
        StartCoroutine(FlashRed());
        if (Health <= 0) Die();
    }

    public void KillInstantly()
    {
        if (isDead) return;
        Health = 0;
        Die();
    }

    IEnumerator FlashRed()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;

        foreach (Renderer r in renderers)
            r.material.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        if (!isDead)
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.color = originalColors[i];
    }

    void Die()
    {
        isDead = true;
        audioSource.Stop();
        if (deathClip != null)
            AudioSource.PlayClipAtPoint(deathClip, transform.position);

   
        GameManager.Instance.GolemKilled();
        Debug.Log("Golem killed!");
        Destroy(gameObject);
    }

    void CatchPlayer()
    {
  
        for (int i = 0; i < LivesDamage; i++)
            GameManager.Instance.LoseLife();

        Debug.LogWarning("Golem caught you! (-" + LivesDamage + " lives)");
        cooldownTimer = CatchCooldown;
    }
}