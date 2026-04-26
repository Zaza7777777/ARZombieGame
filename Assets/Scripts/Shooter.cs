using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    public float ShootRange = 20f;
    public float GazeTimeRequired = 0.8f;
    public Camera arCamera;
    public Image ReticleFill;
    public Image ReticleBase;       // add this — the background ring
    public GameObject revolver;

    [Header("Reticle Colors")]
    public Color idleColor = Color.white;
    public Color shootingColor = Color.red;
    public Color healColor = Color.green;

    private float gazeTimer = 0f;
    private ZombieBehaviour currentTarget = null;
    private HealthPickup currentMedKit = null;
    private WeaponRecoil weaponRecoil;

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
        if (revolver != null)
            weaponRecoil = revolver.GetComponent<WeaponRecoil>();
    }

    void Update()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;

        Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, ShootRange))
        {
            // Check for zombie first
            ZombieBehaviour zombie = hit.collider.gameObject
                .GetComponentInParent<ZombieBehaviour>();

            // Check for med kit
            HealthPickup medKit = hit.collider.gameObject
                .GetComponent<HealthPickup>();

            if (zombie != null)
            {
                currentMedKit = null;
                SetReticleColor(shootingColor);

                if (zombie == currentTarget)
                {
                    gazeTimer += Time.deltaTime;
                    if (ReticleFill != null)
                        ReticleFill.fillAmount = gazeTimer / GazeTimeRequired;

                    if (gazeTimer >= GazeTimeRequired)
                    {
                        zombie.TakeHit();
                        gazeTimer = 0f;
                        if (weaponRecoil != null)
                            weaponRecoil.PlayRecoil();
                    }
                }
                else
                {
                    currentTarget = zombie;
                    gazeTimer = 0f;
                }
            }
            else if (medKit != null)
            {
                // Looking at med kit
                currentTarget = null;
                currentMedKit = medKit;
                SetReticleColor(healColor);

                gazeTimer += Time.deltaTime;
                if (ReticleFill != null)
                    ReticleFill.fillAmount = gazeTimer / medKit.gazeTimeRequired;
            }
            else
            {
                ResetGaze();
            }
        }
        else
        {
            ResetGaze();
        }
    }

    void SetReticleColor(Color color)
    {
        if (ReticleFill != null) ReticleFill.color = color;
        if (ReticleBase != null) ReticleBase.color = color;
    }

    void ResetGaze()
    {
        currentTarget = null;
        currentMedKit = null;
        gazeTimer = 0f;
        if (ReticleFill != null)
        {
            ReticleFill.fillAmount = 0f;
            ReticleFill.color = idleColor;
        }
        if (ReticleBase != null)
            ReticleBase.color = idleColor;
    }
}