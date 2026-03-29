using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public float ShootRange = 20f;
    public Camera arCamera;

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    void Update()
    {
        if (arCamera == null) arCamera = Camera.main;

        // New Input System - detect screen tap on phone
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                Shoot();
            }
        }

        // New Input System - mouse click for Editor testing
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;

        Debug.Log("Shoot!");

        Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, ShootRange))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            ZombieBehaviour zombie = hit.collider.gameObject.GetComponentInParent<ZombieBehaviour>();
            if (zombie != null)
            {
                zombie.TakeHit();
                Debug.LogWarning("Zombie hit!");
            }
        }
    }
}