using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public Color fullHealth = Color.green;
    public Color lowHealth = Color.red;

    void Update()
    {
        if (GameManager.Instance == null) return;

        float healthPercent = (float)GameManager.Instance.Lives / GameManager.Instance.MaxLives;
        healthSlider.value = healthPercent;

        fillImage.color = Color.Lerp(lowHealth, fullHealth, healthPercent);
    }
}