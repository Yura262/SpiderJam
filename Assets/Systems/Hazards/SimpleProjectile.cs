using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float lifetime = 3f; // Set the desired lifetime in seconds
    public float damage;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
