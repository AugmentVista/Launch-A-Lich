using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlacement : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(55f, 5f, 0f);

    // All pods (unfiltered)
    private List<SpawnPod> allPods = new List<SpawnPod>();

    // Active pods (filtered each time depending on speed)
    private List<SpawnPod> activePods = new List<SpawnPod>();


    public float lowMax = 30f;
    public float mediumMax = 80f;
    public float highMax = 150f;

    public enum SpeedCategory
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    public SpeedCategory CurrentCategory
    {
        get
        {
            float speed = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);

            if (speed < lowMax) return SpeedCategory.Low;
            if (speed < mediumMax) return SpeedCategory.Medium;
            if (speed < highMax) return SpeedCategory.High;
            return SpeedCategory.VeryHigh;
        }
    }

    public Dictionary<PodZone, float> zoneWeights = new Dictionary<PodZone, float>()
    {
        { PodZone.Red,    1f },
        { PodZone.Green,  1f },
        { PodZone.Yellow, 1f },
        { PodZone.Blue,   1f },
    };

    private void Awake()
    {
        allPods.Clear();

        foreach (SpawnPod pod in GetComponentsInChildren<SpawnPod>())
        {
            allPods.Add(pod);
        }
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity;
        }
    }

    private void BuildActivePodList(SpeedCategory speedCategory)
    {
        // Start fresh each time
        activePods = new List<SpawnPod>(allPods);

        // SPEED RULES:
        // Low Speed     → include everything
        // Medium Speed  → include everything
        // High Speed    → exclude Pink only
        // Very High     → exclude Pink and Purple

        switch (speedCategory)
        {
            case SpeedCategory.Low:
            case SpeedCategory.Medium:
                return; // no exclusions

            case SpeedCategory.High:
                RemovePodsWithTag("Pink_Pod_Zones");
                break;

            case SpeedCategory.VeryHigh:
                RemovePodsWithTag("Pink_Pod_Zones");
                RemovePodsWithTag("Purple_Pod_Zones");
                break;
        }
    }

    private void RemovePodsWithTag(string tagToRemove)
    {
        for (int i = activePods.Count - 1; i >= 0; i--)
        {
            if (activePods[i].CompareTag(tagToRemove))
                activePods.RemoveAt(i);
        }
    }

    public Transform GetNextPod()
    {
        SpeedCategory speedCategory = CurrentCategory;

        // Build filtered list for this category
        BuildActivePodList(speedCategory);

        if (activePods.Count == 0)
            return null;

        // Weighting by base color
        Dictionary<SpawnPod, float> weightedPods = new Dictionary<SpawnPod, float>();
        float totalWeight = 0f;

        foreach (SpawnPod pod in activePods)
        {
            float podWeight = zoneWeights[pod.data.zone];
            weightedPods[pod] = podWeight;
            totalWeight += podWeight;
        }

        if (totalWeight <= 0f)
            return null;

        // Weighted random
        float randomPick = Random.Range(0f, totalWeight);

        SpawnPod selected = null;

        foreach ((SpawnPod pod, float weight) in weightedPods)
        {
            randomPick -= weight;

            if (randomPick <= 0f)
            {
                selected = pod;
                break;
            }
        }


        if (selected == null)
            return null;

        // Cooldown handling
        if (selected.IsAvailable)
        {
            selected.TriggerCooldown();
            return selected.transform;
        }

        // If unavailable → find shortest cooldown pod
        float shortestTime = float.MaxValue;
        SpawnPod soonestPod = null;

        foreach (SpawnPod pod in activePods)
        {
            if (pod.cooldownRemaining < shortestTime)
            {
                shortestTime = pod.cooldownRemaining;
                soonestPod = pod;
            }
        }

        if (soonestPod != null)
            StartCoroutine(WaitAndRetry(shortestTime, soonestPod));

        return null;
    }

    private IEnumerator WaitAndRetry(float waitTime, SpawnPod podToUse)
    {
        yield return new WaitForSeconds(waitTime + 0.05f);

        if (podToUse.IsAvailable)
        {
            podToUse.TriggerCooldown();
            spawner.SpawnUsingPod(podToUse.transform);
        }
    }
}