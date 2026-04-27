using UnityEngine;

public class MinigunMode : MonoBehaviour
{
    [Header("Unlock Settings")]
    public int killsRequired = 1;
    public float minigunDuration = 10f;

    [Header("Minigun Settings")]
    public float shootInterval = 0.1f;
    public float shootRange = 20f;
    public int spreadCount = 5;
    public float spreadAngle = 15f;

    [Header("References")]
    public GameObject revolver;
    public GameObject minigun;
    public Camera arCamera;

    private bool minigunActive = false;
    private float shootTimer = 0f;
    private float minigunTimer = 0f;

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;

        if (minigun != null)
            minigun.SetActive(false);
    }

    void Update()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;

       
        if (!minigunActive &&
            GameManager.Instance.GetKillCount() >= killsRequired)
        {
            ActivateMinigun();
        }

        if (minigunActive)
        {
          
            minigunTimer -= Time.deltaTime;
            if (minigunTimer <= 0f)
            {
                DeactivateMinigun();
                return;
            }

        
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                shootTimer = 0f;
                FireSpread();
            }
        }
    }

    void FireSpread()
    {
        for (int i = 0; i < spreadCount; i++)
        {
            float angle = -spreadAngle / 2f +
                         (spreadAngle / (spreadCount - 1)) * i;

            Quaternion rotation = Quaternion.AngleAxis(
                angle, arCamera.transform.up);

            Vector3 direction = rotation * arCamera.transform.forward;
            Ray ray = new Ray(arCamera.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, shootRange))
            {
                ZombieBehaviour zombie = hit.collider.gameObject
                    .GetComponentInParent<ZombieBehaviour>();

                if (zombie != null)
                {
                    zombie.TakeHit();
                    Debug.Log("Minigun hit zombie!");
                }
            }
        }
    }

    void ActivateMinigun()
    {
        minigunActive = true;
        minigunTimer = minigunDuration;

        if (revolver != null) revolver.SetActive(false);
        if (minigun != null) minigun.SetActive(true);

        Debug.Log("Minigun ACTIVATED! " + minigunDuration + "s remaining");
    }

    void DeactivateMinigun()
    {
        minigunActive = false;

        if (revolver != null) revolver.SetActive(true);
        if (minigun != null) minigun.SetActive(false);

        
        GameManager.Instance.ResetKillCount();

        Debug.Log("Minigun DEACTIVATED! Kill 10 more to earn it again");
    }
}