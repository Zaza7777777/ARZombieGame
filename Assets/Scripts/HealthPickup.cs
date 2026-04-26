using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 90f;

    [Header("Gaze Settings")]
    public float gazeTimeRequired = 1.5f;

    [Header("Feedback")]
    public GameObject healPopupPrefab;
    public AudioClip pickupSound;

    private float gazeTimer = 0f;
    private bool collected = false;

    void Update()
    {
        if (collected) return;

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                gazeTimer += Time.deltaTime;
                if (gazeTimer >= gazeTimeRequired)
                    Collect();
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

    void Collect()
    {
        collected = true;

        // Popup text
        if (healPopupPrefab != null)
        {
            Vector3 popupPos = transform.position + Vector3.up * 0.3f;
            Instantiate(healPopupPrefab, popupPos, Quaternion.identity);
        }

        // Sound
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        GameManager.Instance.GainLife();
        Destroy(gameObject);
    }
}