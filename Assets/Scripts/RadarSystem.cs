using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadarSystem : MonoBehaviour
{
    public float RadarRange = 2f;        // real world distance radar covers
    public float RadarSize = 75f;        // half the radar UI size in pixels
    public Transform PlayerTransform;    // drag your ARCamera here
    public GameObject ZombieDotPrefab;   // drag your ZombieDot prefab here
    public RectTransform RadarRect;      // drag the Radar image here

    private Dictionary<ZombieBehaviour, RectTransform> dots = new Dictionary<ZombieBehaviour, RectTransform>();

    void Update()
    {
        if (PlayerTransform == null) return;

        // Find all zombies in the scene
        ZombieBehaviour[] zombies = FindObjectsOfType<ZombieBehaviour>();

        // Remove dots for zombies that no longer exist
        List<ZombieBehaviour> toRemove = new List<ZombieBehaviour>();
        foreach (var entry in dots)
        {
            if (entry.Key == null)
            {
                Destroy(entry.Value.gameObject);
                toRemove.Add(entry.Key);
            }
        }
        foreach (var key in toRemove) dots.Remove(key);

        // Add dots for new zombies
        foreach (ZombieBehaviour zombie in zombies)
        {
            if (!dots.ContainsKey(zombie))
            {
                GameObject dot = Instantiate(ZombieDotPrefab, RadarRect);
                dots[zombie] = dot.GetComponent<RectTransform>();
            }
        }

        // Update dot positions
        foreach (var entry in dots)
        {
            if (entry.Key == null) continue;

            // Get direction from player to zombie
            Vector3 diff = entry.Key.transform.position - PlayerTransform.position;

            // Rotate relative to player's facing direction (so radar rotates with you)
            float angle = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            angle -= PlayerTransform.eulerAngles.y;
            float radian = angle * Mathf.Deg2Rad;

            // Calculate distance, clamp to radar edge if too far
            float distance = Mathf.Min(diff.magnitude / RadarRange, 1f);

            // Convert to radar UI position
            float x = Mathf.Sin(radian) * distance * RadarSize;
            float y = Mathf.Cos(radian) * distance * RadarSize;

            entry.Value.anchoredPosition = new Vector2(x, y);
        }
    }
}