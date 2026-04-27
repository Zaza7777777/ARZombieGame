using UnityEngine;
using System.Collections;

public class ZombieGrabHandler : MonoBehaviour
{
    public float GrabDistance = 0.5f;
    public float LaunchForce = 6f;
    public float SlamSpeed = 3f;

    private Transform arCamera;
    public bool isGrabbing = false;
    private ZombieBehaviour zombieBehaviour;
    private Rigidbody rb;
    private Vector3 originalPosition;

    void Start()
    {
        arCamera = Camera.main.transform;
        zombieBehaviour = GetComponent<ZombieBehaviour>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (isGrabbing) return;
        if (zombieBehaviour.isDead) return;

        float distance = Vector3.Distance(transform.position, arCamera.position);
        if (distance < GrabDistance)
        {
            StartCoroutine(GrabSequence());
        }
    }

    IEnumerator GrabSequence()
    {
        isGrabbing = true;
        originalPosition = transform.position;

     
        zombieBehaviour.enabled = false;

        
        Vector3 targetPos = arCamera.position + arCamera.forward * 0.4f
                          + Vector3.down * 0.7f;

       
        float t = 0f;
        Vector3 startPos = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * SlamSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            transform.LookAt(arCamera.position);
            yield return null;
        }

       
        ScreenOverlay.Instance.ShowOverlay();

        
        yield return new WaitUntil(() => ScreenOverlay.Instance.IsShakeComplete());

     
        ScreenOverlay.Instance.HideOverlay();

      
        Vector3 launchDir = (transform.position - arCamera.position).normalized;
        launchDir.y = 0.3f;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(launchDir * LaunchForce, ForceMode.Impulse);

       
        yield return new WaitForSeconds(2f);
        zombieBehaviour.isDead = true;
        GameManager.Instance.ZombieKilled();
        Destroy(gameObject);
    }
}