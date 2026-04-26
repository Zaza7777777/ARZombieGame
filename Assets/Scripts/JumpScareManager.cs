using UnityEngine;
using UnityEngine.UI;

public class JumpScareManager : MonoBehaviour
{
    public static JumpScareManager Instance;

    [Header("UI References")]
    public GameObject zombieFaceImage;
    public Image shakeBar;

    [Header("Jumpscare Timing")]
    public float minTime = 5f;
    public float maxTime = 60f;
    public float jumpScareDuration = 1f;

    [Header("Skip Chance")]
    [Range(0f, 1f)]
    public float skipChance = 0.4f;

    [Header("Health Scaling")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Shake Dismiss (Optional)")]
    public bool requireShakeToDismiss = false;
    public int shakesRequired = 2;
    public float shakeThreshold = 0.8f;

    [Header("Audio")]
    public AudioSource jumpScareAudio;

    private int shakeCount = 0;
    private bool isActive = false;
    private Vector3 lastAccel;
    private float shakeCooldown = 0f;
    private float activeTimer = 0f;

    void Awake() { Instance = this; }
    void Start() { ScheduleNextJumpScare(); }

    // ─── Call from your health system whenever damage is taken ───
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
    }

    void ScheduleNextJumpScare()
    {
        float urgency = 1f - (currentHealth / maxHealth);
        float randomTime = Random.Range(minTime, maxTime) * (1f - urgency * 0.5f);
        Invoke("TriggerJumpScare", randomTime);
    }

    void TriggerJumpScare()
    {
        if (GameManager.Instance.IsGameOver()) return;

        // Random chance to skip and silently reschedule
        if (Random.value < skipChance)
        {
            ScheduleNextJumpScare();
            return;
        }

        shakeCount = 0;
        isActive = true;
        activeTimer = 0f;

        zombieFaceImage.SetActive(true);

        // Play audio — will be cut off on dismiss
        if (jumpScareAudio != null)
        {
            jumpScareAudio.Stop();
            jumpScareAudio.Play();
        }

        if (requireShakeToDismiss)
        {
            shakeBar.gameObject.SetActive(true);
            shakeBar.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (!isActive) return;

        // ── Auto-dismiss after split second ──
        if (!requireShakeToDismiss)
        {
            activeTimer += Time.deltaTime;
            if (activeTimer >= jumpScareDuration)
                DismissJumpScare();
            return;
        }

        // ── Shake to dismiss ──
        shakeCooldown -= Time.deltaTime;
        Vector3 accel = Input.acceleration;
        Vector3 delta = accel - lastAccel;
        float spike = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);

        if (spike > shakeThreshold && shakeCooldown <= 0f)
        {
            shakeCount++;
            shakeCooldown = 0.3f;
            shakeBar.fillAmount = (float)shakeCount / shakesRequired;
        }

        lastAccel = accel;

        if (shakeCount >= shakesRequired)
            DismissJumpScare();
    }

    void DismissJumpScare()
    {
        isActive = false;

        // Cut audio immediately when face disappears
        if (jumpScareAudio != null)
            jumpScareAudio.Stop();

        zombieFaceImage.SetActive(false);

        if (requireShakeToDismiss)
        {
            shakeBar.fillAmount = 0f;
            shakeBar.gameObject.SetActive(false);
        }

        ScheduleNextJumpScare();
    }

    // ─── Call on game over ───
    public void StopAllScares()
    {
        CancelInvoke("TriggerJumpScare");

        if (jumpScareAudio != null)
            jumpScareAudio.Stop();

        if (isActive) DismissJumpScare();
    }
}