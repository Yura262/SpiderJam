using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls where and when powerups spawn.
/// Uses weighted random selection and difficulty-based pacing.
/// </summary>
public class PowerupController : MonoBehaviour
{
    [System.Serializable]
    public class PowerupType
    {
        public string name;
        public GameObject prefab;
        [Tooltip("Base spawn chance weight; higher = more likely")]
        public float spawnWeight = 1f;

        [Tooltip("Maximum number of this powerup type active at once")]
        public int maxActiveCount = 1;
    }

    [Header("Powerups")]
    public List<PowerupType> powerups = new List<PowerupType>();

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 3f;

    public int maxCount=4;

    [Header("Difficulty / Timing")]
    public float difficultyMultiplier = 1f;
    public float difficultyRamp = 0.005f;

    private bool isSpawning = true;
    private readonly List<Transform> activePowerups = new List<Transform>();

    void OnEnable()
    {
        PowerUp.OnPowerUpCollected += OnPowerupCollected;
    }

    void OnDisable()
    {
        PowerUp.OnPowerUpCollected -= OnPowerupCollected;
    }

    void OnPowerupCollected(Transform p)
    {
        if (activePowerups.Contains(p))
            activePowerups.Remove(p);
    }

    void Start()
    {
        // Example: stop spawning on player death
        if (GameManager.Instance != null && GameManager.Instance.playerHealth != null)
            GameManager.Instance.playerHealth.OnDeath += (DamageType dt) => isSpawning = false;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            SpawnPowerup();
            difficultyMultiplier += difficultyRamp * Time.deltaTime;
        }
    }

    void SpawnPowerup()
    {
        if (powerups.Count == 0 || spawnPoints.Length == 0)
            return;

        if (activePowerups.Count >= maxCount)
            return;

        //PowerupType pType = ChooseWeightedPowerup();
        if (!TryChooseWeightedPowerup(out PowerupType pType))
            return;

        Transform spawn = ChooseSpawnPoint();
        if (spawn == transform) return;

        GameObject obj = Instantiate(pType.prefab, spawn.position, Quaternion.identity);
        PowerUp powerup = obj.GetComponent<PowerUp>();

        if (powerup != null)
        {
            powerup.type = pType;
            activePowerups.Add(obj.transform);
        }

        // Adjust delay slightly with difficulty
        minSpawnDelay = Mathf.Max(0.5f, minSpawnDelay - 0.002f * difficultyMultiplier);
        maxSpawnDelay = Mathf.Max(1.5f, maxSpawnDelay - 0.004f * difficultyMultiplier);
    }

    public bool TryChooseWeightedPowerup(out PowerupType result)
    {
        // Count active per type
        Dictionary<string, int> activeCounts = new Dictionary<string, int>();
        foreach (Transform active in activePowerups)
        {
            PowerUp p = active.GetComponent<PowerUp>();

            string name = p.type.name;
            if (!activeCounts.ContainsKey(name))
                activeCounts[name] = 0;
            activeCounts[name]++;
        }

        // Filter available types
        List<PowerupType> availableTypes = new List<PowerupType>();
        foreach (var pt in powerups)
        {
            int count = activeCounts.ContainsKey(pt.name) ? activeCounts[pt.name] : 0;
            if (count < pt.maxActiveCount)
                availableTypes.Add(pt);
        }

        // Handle the case where no powerups are available
        if (availableTypes.Count == 0)
        {
            result = null; // Присвоюємо null, але вище ми повернемо false
            return false;
        }

        // Weighted random
        float totalWeight = 0f;
        foreach (var p in availableTypes) // Замінили selectionPool на availableTypes
            totalWeight += p.spawnWeight * difficultyMultiplier;

        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var p in availableTypes) // Замінили selectionPool на availableTypes
        {
            cumulative += p.spawnWeight * difficultyMultiplier;
            if (randomValue <= cumulative)
            {
                result = p;
                return true;
            }
        }

        result = availableTypes[availableTypes.Count - 1];
        return true;
    }


    Transform ChooseSpawnPoint()
    {
        List<Transform> available = new List<Transform>(spawnPoints);
        foreach (var used in activePowerups)
        {
            if (used != null)
                available.RemoveAll(spawn => spawn.position == used.position);
        }

        if (available.Count == 0)
            return transform;

        return available[Random.Range(0, available.Count)];
    }

    public void SetDifficulty(float difficulty)
    {
        difficultyMultiplier = Mathf.Max(0.1f, difficulty);
    }
}
