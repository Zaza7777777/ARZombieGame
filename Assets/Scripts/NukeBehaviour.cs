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
    public TMP_Text countdownLabel; 

    private float gazeTimer = 0f;
    private float expireTimer = 0f;
    private bool activated = false;

    void Update()
    {
        if (activated) return;

        
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        
        expireTimer += Time.deltaTime;

        
        if (countdownLabel != null)
        {
            float remaining = Mathf.Max(0f, timeBeforeExpire - expireTimer);
            countdownLabel.text = Mathf.CeilToInt(remaining) + "s";
        }

       
        if (expireTimer >= timeBeforeExpire)
        {
            Debug.Log("Nuke expired!");
            Destroy(gameObject);
            return;
        }

        
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

        
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        
        if (activateSound != null)
            AudioSource.PlayClipAtPoint(activateSound, transform.position);

        
        GameManager.Instance.KillAllZombies();
        GameManager.Instance.LoseLife();

        Debug.Log("NUKE ACTIVATED — all zombies killed, 1 life lost");
        Destroy(gameObject);
    }
}