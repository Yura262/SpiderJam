
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour//animation and sound :^)
{
    private Animator animator;
    private bool isGrappling = false;
    private bool moving;
    [SerializeField]
    private bool DisableFallingAnimation = false;
    private Transform player;
    public GameObject deathScreen;





    public AudioSource damageSound;
    public AudioSource deathSound;
    public AudioSource PickupSound;


    void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.Instance.playerHealth.OnDeath += HandleDeath;
        GameManager.Instance.playerHealth.OnDamageTaken += HandleDamage;
        player = GameManager.Instance.playerHealth.transform;
        if(deathScreen)
            deathScreen.SetActive(false);


        PowerUp.OnPowerUpCollected +=(Transform t)=> powerupPickup();
    }
    void powerupPickup()
    {
        if (PickupSound)
        PickupSound.Play();
    }
    private void Update()
    {
        if (isGrappling != GameManager.Instance.spiderController.IsGrappling)
        {
            isGrappling = GameManager.Instance.spiderController.IsGrappling;
            animator.SetBool("isGrapling", isGrappling);
        }


        if (GameManager.Instance.spiderController.rb.linearVelocity.y != 0 != moving)
        {
            moving = !moving;
            if (!DisableFallingAnimation)
                animator.SetBool("Moving", moving);

        }
        Vector2 movement = GameManager.Instance.spiderController.rb.linearVelocity.normalized;
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        player.rotation  = Quaternion.Euler(0f, 0f, angle-90);




    }


    private void HandleDeath(DamageType type)
    {
        if (deathScreen)
            deathScreen.SetActive(true);
        if (gameObject.CompareTag("DeadPlayer"))
            return;

        if(deathSound)
            deathSound.Play();
        switch (type)
        {
            //case DamageType.Projectile:
            //    animator.SetTrigger("DieProjectile");
            //    break;
            //case DamageType.Fire:
            //    animator.SetTrigger("DieBurn");
            //    break;
            default:
                animator.SetTrigger("Die");
                break;
        }
    }
    private void HandleDamage(float amount, DamageType type)
    {
        
        if (gameObject.CompareTag("DeadPlayer"))
            return;

        damageSound.Play();
        switch (type)
        {
            //case DamageType.Projectile:
            //    animator.SetTrigger("ProjectileDamage");
            //    break;
            case DamageType.Fire:
                animator.SetTrigger("BurnDamage");
                break;
            default:
                animator.SetTrigger("Damage");
                break;
        }
    }
}

