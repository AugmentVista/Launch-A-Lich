using UnityEngine;
using System;

public class TreatManager : MonoBehaviour
{
    // 9 counters, one for each treat
    private int[] collectedCounts = new int[9];

    public TreatObject[] allTreatObjects;


    private void OnEnable()
    {
        PlayerInteractionHandler.OnFlyingItemCollected += OnTreatCollected;
        PlayerInteractionHandler.OnGroundItemCollected += OnTreatCollected;
    }

    private void OnDisable()
    {
        PlayerInteractionHandler.OnFlyingItemCollected -= OnTreatCollected;
        PlayerInteractionHandler.OnGroundItemCollected -= OnTreatCollected;
    }

    private void OnTreatCollected(int gold, Enum tierEnum) // Array index of treats to store data run by run
    {
        TreatType treat = (TreatType)tierEnum;
        int index = (int)treat;

        collectedCounts[index]++;
    }

    public int GetAndConsumeRunCount(TreatType type) // Empty that index of treats into TreatObjects.
    {
        int index = (int)type;
        int value = collectedCounts[index];
        collectedCounts[index] = 0;
        return value;
    }

    public void ApplyRunResultsToTreatObjects()
    {
        foreach (var obj in allTreatObjects)
        {
            TreatType type = obj.treatPickUp.treatType;
            int count = GetAndConsumeRunCount(type);

            obj.IncrementTotalCollected(count);
        }
    }


    // Access treat count without clearing it for scripts to view
    public int GetCount(TreatType treat)
    {
        return collectedCounts[(int)treat];
    }

    public void ResetAll()
    {
        for (int i = 0; i < collectedCounts.Length; i++)
            collectedCounts[i] = 0;
    }
}
