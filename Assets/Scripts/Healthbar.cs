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

        // Update slider value based on lives
        float healthPercent = (float)GameManager.Instance.Lives / GameManager.Instance.MaxLives;
        healthSlider.value = healthPercent;

        // Change color based on health
        fillImage.color = Color.Lerp(lowHealth, fullHealth, healthPercent);
    }
}