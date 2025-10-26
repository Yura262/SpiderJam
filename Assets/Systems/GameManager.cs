using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject player;
    [HideInInspector]
    public PlayerHealth playerHealth;
    [HideInInspector]
    public PlayerPowerUpManager powerUpManager;
    [HideInInspector]
    public SpiderController spiderController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Ensure only one instance

        playerHealth = player.GetComponent<PlayerHealth>();
        powerUpManager = player.GetComponent<PlayerPowerUpManager>();
        spiderController = player.GetComponent<SpiderController>();
    }

    private void Start()
    {
        
    }
}
