using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public PlayerPowerUpManager.PowerUpType powerUpType;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.powerUpManager.ActivatePowerUp(powerUpType, duration);

            Destroy(gameObject); // remove pickup
        }
    }
    [HideInInspector]
    public PowerupController.PowerupType type;




    public static event Action<Transform> OnPowerUpCollected;

    void OnDestroy()
    {
        OnPowerUpCollected?.Invoke(transform);
    }
}
