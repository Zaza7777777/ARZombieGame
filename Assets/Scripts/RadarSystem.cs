using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadarSystem : MonoBehaviour
{
    public float RadarRange = 2f;        
    public float RadarSize = 75f;        
    public Transform PlayerTransform;    
    public GameObject ZombieDotPrefab;   
    public RectTransform RadarRect;      

    private Dictionary<ZombieBehaviour, RectTransform> dots = new Dictionary<ZombieBehaviour, RectTransform>();

    void Update()
    {
        if (PlayerTransform == null) return;

        
        ZombieBehaviour[] zombies = FindObjectsOfType<ZombieBehaviour>();

        
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

       
        foreach (ZombieBehaviour zombie in zombies)
        {
            if (!dots.ContainsKey(zombie))
            {
                GameObject dot = Instantiate(ZombieDotPrefab, RadarRect);
                dots[zombie] = dot.GetComponent<RectTransform>();
            }
        }

        
        foreach (var entry in dots)
        {
            if (entry.Key == null) continue;

            
            Vector3 diff = entry.Key.transform.position - PlayerTransform.position;

            
            float angle = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            angle -= PlayerTransform.eulerAngles.y;
            float radian = angle * Mathf.Deg2Rad;

            
            float distance = Mathf.Min(diff.magnitude / RadarRange, 1f);

            
            float x = Mathf.Sin(radian) * distance * RadarSize;
            float y = Mathf.Cos(radian) * distance * RadarSize;

            entry.Value.anchoredPosition = new Vector2(x, y);
        }
    }
}