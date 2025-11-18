using UnityEngine;
using System;

public class TreatManager : MonoBehaviour
{
    // 9 counters, one for each treat
    private int[] collectedCounts = new int[9];

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

    private void OnTreatCollected(int gold, Enum tierEnum)
    {
        //int index = (int)tierEnum;

        //collectedCounts[index] += 1;

    }

    // Called by ResultsScreen UI
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
