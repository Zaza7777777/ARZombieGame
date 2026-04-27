using System.Collections;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public float recoilAmount = 0.05f;
    public float recoilSpeed = 10f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void PlayRecoil()
    {
        StopAllCoroutines();
        StartCoroutine(RecoilCoroutine());
    }

    IEnumerator RecoilCoroutine()
    {
        
        Vector3 recoilPos = originalPosition + new Vector3(0, 0.02f, -recoilAmount);
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(originalPosition, recoilPos, t);
            yield return null;
        }

        
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(recoilPos, originalPosition, t);
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}