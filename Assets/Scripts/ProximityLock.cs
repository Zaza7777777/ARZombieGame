using System;
using UnityEngine;
using Vuforia;

public class ProximityLock : MonoBehaviour
{
    public bool Updating = false;
    public static GameObject Target;
    public String TargetTag = "lockon";

    public float LockonDistance = 0.1f;
    public GameObject LockonDistanceIndicator;
    public GameObject TargettedIndicator;
    public float DistanceToTarget;

    private GameObject LastTarget;

    
    void Update()
    {
        
        if(LockonDistanceIndicator != null)
        {
            LockonDistanceIndicator.transform.localScale = 
                new Vector3(LockonDistance * 2, 0.001f, LockonDistance * 2);
        }

        if (!Updating) return;

        if(Target == null )
        {
            
            GameObject[] lockOnObjects = GameObject.FindGameObjectsWithTag(TargetTag);
            Target = null;
            float minDistance = float.PositiveInfinity;

           
            foreach (GameObject obj in lockOnObjects)
            {
                ObserverBehaviour ob = obj.GetComponent<ObserverBehaviour>();
                if (ob == null) continue;
                if (ob.TargetStatus.Status == Status.NO_POSE) continue;

                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < minDistance )
                {
                    minDistance = distance;
                    Target = obj;
                }
            }

            
            if (minDistance > LockonDistance)
            {
                Target = null;
            }
            else
            {
                Debug.LogWarning("Found target = " + Target.name);
            }
        }

        
        if (Target != null)
        {
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            DistanceToTarget = distance; 
            if (distance > LockonDistance)
            {
                Debug.LogWarning("Target no longer in range = " + Target.name);
                Target = null;
            }
        }

       
        if ((Target != null) && (LastTarget != Target))
        {
            Debug.LogWarning("Started targetting " + Target.name);
           
            TargettedIndicator.transform.parent = Target.transform;
            TargettedIndicator.transform.localPosition = Vector3.zero;
        }

        
        if (Target != null)
        {

        }

      
        if ((LastTarget != null) && (Target != LastTarget))
        {
            Debug.LogWarning("Stopped targetting " + LastTarget.name);
           
            TargettedIndicator.transform.parent = transform;
            TargettedIndicator.transform.localPosition = Vector3.zero;
        }

     
        LastTarget = Target;
    }

  
    public void StartScanning()
    {
        Debug.LogWarning("Found. Start updating, and scanning for nearby objects.");
        Updating = true;
        Target = null;
    }

  
    public void StopScanning()
    {
        Debug.LogWarning("Lost. Stop updating and scanning. Lose target (if have one).");
        Updating = false;
        Target = null;
    }
}
