using UnityEngine;
using TMPro;

public class NukeBehaviour : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 90f;

    [Header("Gaze Settings")]
    public float gazeTimeRequired = 3f;

    [Header("Nuke Settings")]
    public float timeBeforeExpire = 15f;

    [Header("Feedback")]
    public GameObject explosionEffectPrefab;
    public AudioClip activateSound;
    public TMP_Text countdownLabel; // optional: world-space TMP on the nuke object

    private float gazeTimer = 0f;
    private float expireTimer = 0f;
    private bool activated = false;

    void Update()
    {
        if (activated) return;

        // Rotate like medkit
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Countdown to expiry
        expireTimer += Time.deltaTime;

        // Update the countdown label if assigned
        if (countdownLabel != null)
        {
            float remaining = Mathf.Max(0f, timeBeforeExpire - expireTimer);
            countdownLabel.text = Mathf.CeilToInt(remaining) + "s";
        }

        // Nuke expires and disappears
        if (expireTimer >= timeBeforeExpire)
        {
            Debug.Log("Nuke expired!");
            Destroy(gameObject);
            return;
        }

        // Gaze detection — same as HealthPickup
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                gazeTimer += Time.deltaTime;
                if (gazeTimer >= gazeTimeRequired)
                    Activate();
            }
            else
            {
                gazeTimer = 0f;
            }
        }
        else
        {
            gazeTimer = 0f;
        }
    }

    void Activate()
    {
        activated = true;

        // Explosion effect
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Sound
        if (activateSound != null)
            AudioSource.PlayClipAtPoint(activateSound, transform.position);

        // Kill all zombies + cost 1 life
        GameManager.Instance.KillAllZombies();
        GameManager.Instance.LoseLife();

        Debug.Log("NUKE ACTIVATED — all zombies killed, 1 life lost");
        Destroy(gameObject);
    }
}