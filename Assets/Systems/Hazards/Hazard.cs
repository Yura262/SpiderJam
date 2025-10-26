// on start appear
// once apeared enable damage child
// on timeout disapear and delete

using System;
using UnityEngine;

/// <summary>
/// відтворює анімацію при появі, після її закінчення вмикає damageArea на визначений період, відтворює анімацію зникання і видаляється
/// </summary>
public class Hazard : MonoBehaviour
{
    public Animator animator;
    public GameObject damageArea;
    public float lifetime = 3f; // how long it stays active before disappearing
    public HazardController.HazardType type;
    protected virtual void Start()
    {
        GameManager.Instance.playerHealth.OnDeath += (DamageType dt) => BeginDisappear(); // disapear after death



        if (damageArea)
            damageArea.SetActive(false);

        if (animator)
            animator.SetTrigger("Appear");
    }

    /// call when apearing animation ends
    public virtual void OnAppearEnd()
    {
        if (damageArea)
            damageArea.SetActive(true);

        Invoke(nameof(BeginDisappear), lifetime);
    }


    protected virtual void BeginDisappear()
    {
        if (damageArea)
            damageArea.SetActive(false);

        if (animator)
            animator.SetTrigger("Disappear");
    }

    /// call when disapearing animation ends
    public virtual void OnDisappearEnd()
    {
        Destroy(gameObject);
    }



    [HideInInspector]
    public static event Action<Transform> OnHazardDestroyed;

    void OnDestroy()
    {
        // Notify listeners (like HazardController)
        OnHazardDestroyed?.Invoke(transform);
    }

}