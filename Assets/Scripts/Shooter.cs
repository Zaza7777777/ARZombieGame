using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    public float ShootRange = 20f;
    public float GazeTimeRequired = 0.8f;
    public Camera arCamera;
    public Image ReticleFill;
    public Image ReticleBase;
    public GameObject revolver;

    [Header("Reticle Colors")]
    public Color idleColor = Color.white;
    public Color shootingColor = Color.red;
    public Color healColor = Color.green;
    public Color nukeColor = new Color(1f, 0.5f, 0f);
    public Color golemColor = new Color(0.5f, 0f, 1f);  // purple for golems

    private float gazeTimer = 0f;
    private ZombieBehaviour currentTarget = null;
    private GolemBehaviour currentGolem = null;          // NEW
    private HealthPickup currentMedKit = null;
    private NukeBehaviour currentNuke = null;
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
            ZombieBehaviour zombie = hit.collider.GetComponentInParent<ZombieBehaviour>();
            GolemBehaviour golem = hit.collider.GetComponentInParent<GolemBehaviour>(); // NEW
            HealthPickup medKit = hit.collider.GetComponentInParent<HealthPickup>();
            NukeBehaviour nuke = hit.collider.GetComponentInParent<NukeBehaviour>();

            if (zombie != null)
            {
                currentGolem = null;
                currentMedKit = null;
                currentNuke = null;
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
                        if (weaponRecoil != null) weaponRecoil.PlayRecoil();
                    }
                }
                else
                {
                    currentTarget = zombie;
                    gazeTimer = 0f;
                }
            }
            else if (golem != null)                      // NEW BLOCK
            {
                currentTarget = null;
                currentMedKit = null;
                currentNuke = null;
                SetReticleColor(golemColor);

                if (golem == currentGolem)
                {
                    gazeTimer += Time.deltaTime;
                    if (ReticleFill != null)
                        ReticleFill.fillAmount = gazeTimer / GazeTimeRequired;

                    if (gazeTimer >= GazeTimeRequired)
                    {
                        golem.TakeHit();
                        gazeTimer = 0f;
                        if (weaponRecoil != null) weaponRecoil.PlayRecoil();
                    }
                }
                else
                {
                    currentGolem = golem;
                    gazeTimer = 0f;
                }
            }
            else if (medKit != null)
            {
                currentTarget = null;
                currentGolem = null;
                currentNuke = null;
                currentMedKit = medKit;
                SetReticleColor(healColor);
                gazeTimer += Time.deltaTime;
                if (ReticleFill != null)
                    ReticleFill.fillAmount = gazeTimer / medKit.gazeTimeRequired;
            }
            else if (nuke != null)
            {
                currentTarget = null;
                currentGolem = null;
                currentMedKit = null;
                currentNuke = nuke;
                SetReticleColor(nukeColor);
                gazeTimer += Time.deltaTime;
                if (ReticleFill != null)
                    ReticleFill.fillAmount = gazeTimer / nuke.gazeTimeRequired;
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
        currentGolem = null;
        currentMedKit = null;
        currentNuke = null;
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