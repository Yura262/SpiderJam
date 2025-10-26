using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls where and when hazards (enemies, obstacles, etc.) spawn.
/// Scales spawn rate with difficulty and avoids repeated spawn points.
/// </summary>
public class HazardController : MonoBehaviour
{
    [System.Serializable]
    public class HazardType
    {
        public string name;
        public GameObject prefab;
        [Tooltip("Base spawn chance weight; higher = more likely")]
        public float spawnWeight = 1f;

        [Tooltip("Maximum number of this hazard type that can be active at once")]
        public int maxActiveCount = 3;  // default max is 3, change as needed
        public bool canExceedMaxActiveCount = true;
    }

    [Header("Hazards")]
    public List<HazardType> hazards = new List<HazardType>();

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 2f;


    [Header("Difficulty")]
    public float difficultyMultiplier = 1f;
    public float difficultyRamp = 0.01f; // how fast difficulty increases over time

    private bool isSpawning = true;

    private readonly List<Transform> activeHazards = new List<Transform>();

    void OnEnable()
    {
        Hazard.OnHazardDestroyed += OnHazardDestroyed;
    }

    void OnDisable()
    {
        Hazard.OnHazardDestroyed -= OnHazardDestroyed;
    }

    void OnHazardDestroyed(Transform h)
    {
        if (activeHazards.Contains(h))
            activeHazards.Remove(h);
    }

    void Start()
    {

        GameManager.Instance.playerHealth.OnDeath += (DamageType dt) => isSpawning = false;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            SpawnHazard();

            // Gradually ramp up difficulty
            difficultyMultiplier += difficultyRamp * Time.deltaTime;
        }
    }

    void SpawnHazard()
    {
        if (hazards.Count == 0 || spawnPoints.Length == 0)
            return;

        HazardType hazardType = ChooseWeightedHazard();
        Transform spawn = ChooseSpawnPoint();
        if (spawn == transform)
            return;
        GameObject obj = Instantiate(hazardType.prefab, spawn.position, Quaternion.identity);

        Hazard hazard = obj.GetComponent<Hazard>();
        if (hazard != null)
        {
            hazard.type = hazardType;
            activeHazards.Add(obj.transform);
        }



        // Adjust spawn delay range with difficulty scaling
        minSpawnDelay = Mathf.Max(0.3f, minSpawnDelay - 0.005f * difficultyMultiplier);
        maxSpawnDelay = Mathf.Max(0.8f, maxSpawnDelay - 0.01f * difficultyMultiplier);
    }

    HazardType ChooseWeightedHazard()
    {
        // Count active hazards by type
        Dictionary<string, int> activeCounts = new Dictionary<string, int>();
        foreach (Transform active in activeHazards)
        {
            if (active == null) continue;
            Hazard h = active.GetComponent<Hazard>();
            if (h == null || h.type == null) continue;

            string name = h.type.name;
            if (!activeCounts.ContainsKey(name))
                activeCounts[name] = 0;
            activeCounts[name]++;
        }

        // Filter hazard types that haven't reached their max count
        List<HazardType> availableTypes = new List<HazardType>();
        foreach (var ht in hazards)
        {
            int count = activeCounts.ContainsKey(ht.name) ? activeCounts[ht.name] : 0;
            if (count < ht.maxActiveCount)
                availableTypes.Add(ht);
        }

        // If all types are maxed, use all types anyway (avoid deadlock)
        if (availableTypes.Count == 0)
        {
            foreach (var ht in hazards)
            {
                if (ht.canExceedMaxActiveCount)
                    availableTypes.Add(ht);
            }
        }
        List<HazardType> selectionPool = availableTypes;

        // Weighted random selection among available types
        float totalWeight = 0f;
        foreach (var h in selectionPool)
            totalWeight += h.spawnWeight * difficultyMultiplier;

        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var h in selectionPool)
        {
            cumulative += h.spawnWeight * difficultyMultiplier;
            if (randomValue <= cumulative)
                return h;
        }

        // Fallback
        return selectionPool[selectionPool.Count - 1];
    }


    Transform ChooseSpawnPoint()
    {
        List<Transform> available = new List<Transform>(spawnPoints);
        List<Vector3> positions = new List<Vector3>();

        foreach (Transform t in available)
        {
            if (t != null) 
                positions.Add(t.position);
        }

        foreach (var used in activeHazards)
        {
            available.RemoveAll(spawnPoint =>spawnPoint.position== used.position);
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
