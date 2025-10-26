using UnityEngine;
using System.Collections;

public class LaserHazard : Hazard
{
    [Header("Beam Setup")]
    public GameObject beamPrefab;      // Prefab for the beam (SpriteRenderer + BoxCollider2D)
    public float extraLength = 0.5f;   // extend beam a bit beyond both points
    public float beamThickness = 0.2f; // local Y scale of the beam
    public float spriteUnitLength = 1f; // how many world units the beam sprite covers at localScale.x = 1

    [Header("Timing / Colors")]
    public float warnDuration = 1f;  // yellow
    public float fireDuration = 1.3f;  // red (damage enabled)
    public Color warnColor = Color.yellow;
    public Color fireColor = Color.red;


    private Transform player;

    protected override void Start()
    {
        base.Start();
        player = GameManager.Instance?.player?.transform;
        StartCoroutine(LaserRoutine());
        GameManager.Instance.playerHealth.OnDeath += (DamageType any) => fireDuration = 0;
        GameManager.Instance.playerHealth.OnDeath += (DamageType any) => warnDuration = 0;
    }

    private IEnumerator LaserRoutine()
    {
        if (beamPrefab == null || player == null)
        {
            Debug.LogWarning("LaserHazard: missing beamPrefab or player, destroying hazard.");
            Destroy(gameObject);
            yield break;
        }

        // Capture positions at spawn time (line that passes through these two points)
        Vector3 a = transform.position;
        Vector3 b = player.position;

        // Direction from hazard to player
        Vector3 dir = (b - a);
        float distance = dir.magnitude;
        if (distance <= 0.001f) distance = 0.001f;
        Vector3 dirNormalized = dir / distance;

        // Compute midpoint position for the beam
        Vector3 mid = a + dirNormalized * (distance * 0.5f);

        // Compute rotation angle (degrees)
        float angleDeg = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;

        // Spawn beam
        GameObject beamGO = Instantiate(beamPrefab, mid, Quaternion.Euler(0f, 0f, angleDeg));
        beamGO.transform.SetParent(null); // keep it independent (or change as you like)

        // Try to set up scale so that beam's length covers distance + extraLength
        // This assumes the beam sprite is oriented along local +X and spriteUnitLength world units long when localScale.x == 1
        float targetLength = distance + extraLength;
        Vector3 localScale = beamGO.transform.localScale;
        localScale.x = (targetLength / spriteUnitLength);
        localScale.y = beamThickness;
        beamGO.transform.localScale = localScale;

        // Get Beam component (optional helper) and configure
        Beam beamComp = beamGO.GetComponent<Beam>();
        if (beamComp != null)
        {
            beamComp.SetColors(warnColor, fireColor);
            beamComp.SetDurations(warnDuration, fireDuration);
            beamComp.SetDamageEnabled(false);
        }
        else
        {
            // If there is no Beam script, we still configure SpriteRenderer and Collider2D directly
            SpriteRenderer sr = beamGO.GetComponent<SpriteRenderer>();
            if (sr) sr.color = warnColor;

            Collider2D col = beamGO.GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }

        // Optionally use your existing damageArea: disable it during warning (we're using beam collider instead)
        if (damageArea) damageArea.SetActive(false);

        // Wait WARNING phase (beam visible in yellow, but not damaging)
        float t = 0f;
        while (t < warnDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Switch to FIRE: change color to red and enable damage (collider or beam component)
        if (beamComp != null)
        {
            beamComp.SetDamageEnabled(true);
            beamComp.PlayFire(); // ensure color updates
        }
        else
        {
            SpriteRenderer sr = beamGO.GetComponent<SpriteRenderer>();
            if (sr) sr.color = fireColor;
            Collider2D col = beamGO.GetComponent<Collider2D>();
            if (col) col.enabled = true;
        }

        // Fire phase
        t = 0f;
        while (t < fireDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // End: disable damage and destroy beam/hazard
        if (beamComp != null)
        {
            beamComp.SetDamageEnabled(false);
        }
        else
        {
            Collider2D col = beamGO.GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }


        if (beamGO) Destroy(beamGO);
        if (damageArea)
            damageArea.SetActive(false);
        Destroy(gameObject);

    }

}
