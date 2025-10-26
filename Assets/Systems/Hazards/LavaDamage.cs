using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;



public class Damage : MonoBehaviour
{


    public float damage = 10f;


    protected void OnTriggerStay2D(Collider2D collision)            // оновлюється при руху обєкта
    {
        if (collision.gameObject.tag == "Player")
            GameManager.Instance.playerHealth.TakeDamage(damage);
    }
}



public class LavaDamage : Damage
{

    public float repeatInterval = 1f;

    private float nextDamageTime;

    private new void OnTriggerStay2D(Collider2D colision) { } // override base method to do nothing



    private bool playerInLava = false;
    private Coroutine damageCoroutine; 

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            playerInLava = true;
            if (damageCoroutine == null && gameObject.activeInHierarchy)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            playerInLava = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (playerInLava)
        {
            GameManager.Instance.playerHealth.TakeDamage(damage, DamageType.Fire);
            yield return new WaitForSeconds(repeatInterval);
        }
    }
}
