using UnityEngine;
using System.Collections;

public class RotatingHazard : Hazard
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;       // degrees per second at full speed
    public float accelerationTime = 2f;    // seconds to reach full speed
    public float decelerationTime = 1f;    // seconds to stop
    //public float /*continuousTime*/ = 1.5f;      // how long it spins before slowing down

    private float currentSpeed = 0f;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(RotationRoutine());
        GameManager.Instance.playerHealth.OnDeath += (DamageType any) => lifetime = 0;
    }

    private IEnumerator RotationRoutine()
    {
        // === STAGE 1: Slow start + damage area OFF ===
        if (damageArea) damageArea.SetActive(false);

        float t = 0f;
        while (t < accelerationTime)
        {
            t += Time.deltaTime;
            currentSpeed = Mathf.Lerp(0f, rotationSpeed, t / accelerationTime);
            transform.Rotate(Vector3.forward, currentSpeed * Time.deltaTime);
            yield return null;
        }
        currentSpeed = rotationSpeed;

        // === STAGE 2: Continuous rotation + damage area ON ===
        if (damageArea) damageArea.SetActive(true);
        float elapsed = 0f;
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            transform.Rotate(Vector3.forward, currentSpeed * Time.deltaTime);
            yield return null;
        }

        // === STAGE 3: Slowdown & self-destroy ===
        if (damageArea) damageArea.SetActive(false);
        t = 0f;
        float startSpeed = currentSpeed;
        while (t < decelerationTime)
        {
            t += Time.deltaTime;
            currentSpeed = Mathf.Lerp(startSpeed, 0f, t / decelerationTime);
            transform.Rotate(Vector3.forward, currentSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

}
