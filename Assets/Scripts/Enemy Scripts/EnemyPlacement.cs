using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlacement : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(55f, 5f, 0f);

    // All pods detected under this parent at Awake()
    [SerializeField] private List<SpawnPod> allPods = new List<SpawnPod>();

    // Pods filtered according to speed rules (pink/purple removal)
    [SerializeField] private List<SpawnPod> activePods = new List<SpawnPod>();


    // -----------------------------
    // Speed Category Breakpoints
    // -----------------------------
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

    // -----------------------------
    // Color priority lists
    // -----------------------------
    private readonly Dictionary<SpeedCategory, PodZone[]> colorPriority = new Dictionary<SpeedCategory, PodZone[]>()
    {
        // Low speed → Blue → Yellow → Green → Red
        { SpeedCategory.Low,     new[] { PodZone.Blue, PodZone.Yellow, PodZone.Green, PodZone.Red } },

        // Medium speed → Green → Red → Yellow → Blue
        { SpeedCategory.Medium,  new[] { PodZone.Green, PodZone.Red, PodZone.Yellow, PodZone.Blue } },

        // High speed → Red → Blue → Green → Yellow
        { SpeedCategory.High,    new[] { PodZone.Red, PodZone.Blue, PodZone.Green, PodZone.Yellow } },

        // Very High speed → Green → Red → Blue (Yellow excluded)
        { SpeedCategory.VeryHigh, new[] { PodZone.Green, PodZone.Red, PodZone.Blue } },
    };



    // ---------------------------------------------------------
    // Speed Category Calculation
    // ---------------------------------------------------------
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

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity;
        }
    }


    // ---------------------------------------------------------
    // BuildActivePodList – Pink / Purple tag filtering
    // ---------------------------------------------------------
    private void BuildActivePodList(SpeedCategory speedCategory)
    {
        activePods = new List<SpawnPod>(allPods);

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


    // ---------------------------------------------------------
    // CalculatePriorityWeights – 5/4/2/1 based on priority order
    // ---------------------------------------------------------
    private Dictionary<SpawnPod, float> CalculatePriorityWeights(SpeedCategory category)
    {
        Dictionary<SpawnPod, float> weighted = new Dictionary<SpawnPod, float>();
        float totalWeight = 0f;

        PodZone[] priorityOrder = colorPriority[category];
        float[] multipliers = { 5f, 4f, 2f, 1f };

        foreach (SpawnPod pod in activePods)
        {
            PodZone zone = pod.data.zone;

            int index = System.Array.IndexOf(priorityOrder, zone);
            if (index < 0)
                continue;

            float priorityMultiplier = multipliers[Mathf.Min(index, multipliers.Length - 1)];

            float finalWeight = pod.data.baseWeight * priorityMultiplier;

            weighted[pod] = finalWeight;
            totalWeight += finalWeight;
        }


        return weighted;
    }


    // ---------------------------------------------------------
    // Public API – Main weighting + cooldown-aware selection
    // ---------------------------------------------------------
    public Transform GetNextPod()
    {
        SpeedCategory category = CurrentCategory;

        BuildActivePodList(category);

        if (activePods.Count == 0)
            return null;

        Dictionary<SpawnPod, float> weightedPods = CalculatePriorityWeights(category);

        if (weightedPods.Count == 0)
            return null;

        // Compute total weight
        float totalWeight = 0f;
        foreach (float weight in weightedPods.Values)
            totalWeight += weight;

        float randomPick = Random.Range(0f, totalWeight);

        // Weighted random selection
        SpawnPod selectedPod = null;

        foreach (KeyValuePair<SpawnPod, float> entry in weightedPods)
        {
            randomPick -= entry.Value;
            if (randomPick <= 0f)
            {
                selectedPod = entry.Key;
                break;
            }
        }

        if (selectedPod == null)
            return null;

        // If free -> use immediately
        if (selectedPod.IsAvailable)
        {
            selectedPod.TriggerCooldown();
            return selectedPod.transform;
        }

        // If not free -> find nearest cooldown pod in same filtered list
        float shortestCooldown = float.MaxValue;
        SpawnPod soonestPod = null;

        foreach (SpawnPod pod in activePods)
        {
            if (pod.cooldownRemaining < shortestCooldown)
            {
                shortestCooldown = pod.cooldownRemaining;
                soonestPod = pod;
            }
        }

        if (soonestPod != null)
            StartCoroutine(WaitAndRetry(shortestCooldown, soonestPod));

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