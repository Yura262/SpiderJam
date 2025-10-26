using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Beam : LavaDamage
{
    public float damagePerSecond = 10f;
    public bool damageEnabled = false;

    private SpriteRenderer sr;
    private Color warnColor = Color.yellow;
    private Color fireColor = Color.red;
    private float warnDuration = 0.6f;
    private float fireDuration = 0.8f;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
        if (sr == null) Debug.LogWarning("Beam: no SpriteRenderer found.");
    }
    
    public void SetColors(Color warn, Color fire)
    {
        warnColor = warn;
        fireColor = fire;
        if (sr) sr.color = warnColor;
    }

    public void SetDurations(float warn, float fire)
    {
        warnDuration = warn;
        fireDuration = fire;
    }

    public void PlayFire()
    {
        if (sr) sr.color = fireColor;
    }

    public void SetDamageEnabled(bool enabled)
    {
        damageEnabled = enabled;
        if (col) col.enabled = enabled;
    }

}
