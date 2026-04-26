using UnityEngine;
using TMPro;

public class HealPopup : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeDuration = 1f;
    private TextMeshPro tmp;
    private Color startColor;
    private float timer = 0f;

    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.text = "+25 HP";
        startColor = tmp.color;
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        transform.forward = Camera.main.transform.forward;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= fadeDuration)
            Destroy(gameObject);
    }
}