using System;
using TMPro;
using UnityEngine;

public enum DamageType
{
    Default,
    Projectile,
    Fire,
    DiedFalling,

}


/// <summary>
/// відповідає за зміну здоров’я, перевіряє наявність бафів, 
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    private PlayerPowerUpManager powerUpManager;
    public TextMeshProUGUI healthTextTemporary;
    [SerializeField]
    private float health = 100;

    public event Action<float, DamageType> OnDamageTaken;
    public event Action<DamageType> OnDeath;
    public bool isDead;                                     //is it dead on animation 
    
    private void Awake()
    {
        powerUpManager = GetComponent<PlayerPowerUpManager>();
    }

    public bool isAlive => health > 0;                      //is it dead inside

    public void TakeDamage(float amount, DamageType damageType=DamageType.Default)
    {
        if (isDead)
            return;


        if (powerUpManager.hasShield)
        {
            Debug.Log("Damage blocked by shield!");
            return;
        }



        OnDamageTaken?.Invoke(amount,damageType);

        health -= amount;
        if (health <= 0)
        {
            Die(damageType);
            health = 0;

        }




        healthTextTemporary.text = health.ToString();
    }
    
    private void Die(DamageType type)
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke(type);

        Debug.Log("Player Died");

    }
}
