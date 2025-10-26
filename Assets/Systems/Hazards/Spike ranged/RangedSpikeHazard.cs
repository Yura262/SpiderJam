using UnityEngine;
using System.Collections;

public class SpikeHazard : Hazard
{
    [Header("Spike Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public float spawnDistance = 15f;   // how far from the hazard to spawn the spike
    public float delayBeforeFire = 0.95f;
    public float destroyDelay = 1f;
    public SpriteRenderer sadas;

    private Transform player;
    protected override void Start()
    {
        base.Start();
        player = GameManager.Instance?.player?.transform;
        StartCoroutine(SpikeRoutine());


    //sadas = GetComponent<SpriteRenderer>();
}



    private IEnumerator SpikeRoutine()
    {

        //if (damageArea) damageArea.SetActive(false);
        // Choose random direction around the hazard
        //float angle = Random.Range(0f, 360f);
        //Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);


        // Compute spawn position far away along that direction

        //transform.rotation = Quaternion.Euler(dir.x, dir.y, 0f);
        //transform.rotation = Quaternion.LookRotation(spawnPos);
        Vector2 dir = (player.position - transform.position).normalized;


        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        //transform.LookAt(new Vector3(spawnPos.x,spawnPos.y,0));
        yield return new WaitForSeconds(delayBeforeFire);

        //if (damageArea) damageArea.SetActive(false);
        sadas.enabled = false;


        

        // Projectile moves through the hazard’s position (toward -dir)
        GameObject proj = Instantiate(projectilePrefab, transform.position+transform.forward* spawnDistance, Quaternion.identity);
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);
        proj.SetActive(true);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * projectileSpeed;




        Destroy(proj, destroyDelay + spawnDistance / projectileSpeed);

        
        Destroy(gameObject, destroyDelay);

    }

}
