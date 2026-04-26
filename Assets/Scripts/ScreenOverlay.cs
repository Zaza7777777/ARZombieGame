using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ScreenOverlay : MonoBehaviour
{
    public static ScreenOverlay Instance;

    [Header("UI References")]
    public Image vignetteImage;
    public TMP_Text shakeText;
    public Image shakeBarFill;

    [Header("Shake Settings")]
    public float shakeThreshold = 2.5f;
    public float shakeRequired = 3f;

    private float shakeProgress = 0f;
    private bool isActive = false;
    private bool shakeComplete = false;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        Vector3 accel = Input.acceleration;
        float shake = accel.sqrMagnitude;

        if (shake > shakeThreshold)
        {
            shakeProgress += Time.deltaTime * 1.5f;
            if (shakeBarFill != null)
                shakeBarFill.fillAmount = shakeProgress / shakeRequired;
        }

        if (vignetteImage != null)
        {
            float pulse = Mathf.Sin(Time.time * 8f) * 0.15f + 0.6f;
            Color c = vignetteImage.color;
            c.a = pulse;
            vignetteImage.color = c;
        }

        if (shakeText != null)
        {
            float offsetX = Random.Range(-3f, 3f);
            float offsetY = Random.Range(-3f, 3f);
            shakeText.rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
        }

        if (shakeProgress >= shakeRequired)
            shakeComplete = true;
    }

    public void ShowOverlay()
    {
        shakeProgress = 0f;
        shakeComplete = false;
        gameObject.SetActive(true);
        isActive = true;
    }

    public void HideOverlay()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public bool IsShakeComplete() => shakeComplete;
}