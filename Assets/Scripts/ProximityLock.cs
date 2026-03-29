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

    // Update is called once per frame
    void Update()
    {
        // Resize distance indicator
        if(LockonDistanceIndicator != null)
        {
            LockonDistanceIndicator.transform.localScale = 
                new Vector3(LockonDistance * 2, 0.001f, LockonDistance * 2);
        }

        if (!Updating) return;

        if(Target == null )
        {
            // Look for target within proxity, then target it

            // Find all GameObjects with the tag "lockon"
            GameObject[] lockOnObjects = GameObject.FindGameObjectsWithTag(TargetTag);
            Target = null;
            float minDistance = float.PositiveInfinity;

            // Iterate through each and find the closest
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

            // Is it within the lock on distance? If not, lose it.
            if (minDistance > LockonDistance)
            {
                Target = null;
            }
            else
            {
                Debug.LogWarning("Found target = " + Target.name);
            }
        }

        // Still within distance?
        if (Target != null)
        {
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            DistanceToTarget = distance; // so can view distance in the object inspector
            if (distance > LockonDistance)
            {
                Debug.LogWarning("Target no longer in range = " + Target.name);
                Target = null;
            }
        }

        // First time locking onto this target - once
        if ((Target != null) && (LastTarget != Target))
        {
            Debug.LogWarning("Started targetting " + Target.name);
            //for (int i = 0; i < Target.transform.childCount; i++)
            //{
            //    Transform child = Target.transform.GetChild(i);
            //    child.localScale = Vector3.one * 0.1f;
            //}
            TargettedIndicator.transform.parent = Target.transform;
            TargettedIndicator.transform.localPosition = Vector3.zero;
        }

        // Do something every update if have a target - every update
        if (Target != null)
        {

        }

        // Locked on target changed - once
        if ((LastTarget != null) && (Target != LastTarget))
        {
            Debug.LogWarning("Stopped targetting " + LastTarget.name);
            //for (int i = 0; i < LastTarget.transform.childCount; i++)
            //{
            //    Transform child = LastTarget.transform.GetChild(i);
            //    child.localScale = Vector3.one * 0.01f;
            //}
            TargettedIndicator.transform.parent = transform;
            TargettedIndicator.transform.localPosition = Vector3.zero;
        }

        // Remember current target
        LastTarget = Target;
    }

    // Called when image target is found. Method is attached as a message that is called
    // from the Default Observer Event Handler component of the image target object.
    public void StartScanning()
    {
        Debug.LogWarning("Found. Start updating, and scanning for nearby objects.");
        Updating = true;
        Target = null;
    }

    // Called when image target is lost. Method is attached as a message that is called
    // from the Default Observer Event Handler component of the image target object.
    public void StopScanning()
    {
        Debug.LogWarning("Lost. Stop updating and scanning. Lose target (if have one).");
        Updating = false;
        Target = null;
    }
}
