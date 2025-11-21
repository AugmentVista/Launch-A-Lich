using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TreatManager : MonoBehaviour
{
    // 9 counters, one for each treat
    private int[] collectedCounts = new int[9];

    public TreatObject[] allTreatObjects;

    public TreatToolTip[] CompletetionCheck;

    [Header("UI")]
    public Image progressBar;
    public TMP_Text progressText;


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

    public bool CheckVictoryCondition()
    {
        for (int i = 0; i < CompletetionCheck.Length; i++)
        {
            if (!CompletetionCheck[i].discovered) { return false; }
        }
        return true;
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
        UpdateTreatProgress();
    }

    public void UpdateTreatProgress()
    {
        float fill = 0f;

        for (int i = 0; i < CompletetionCheck.Length; i++)
        {
            var tip = CompletetionCheck[i];
            int collected = tip.treatObject.ConfirmTreatCollection();

            if (collected >= 5)
            {
                fill += 0.07f;     // fully discovered
            }
            else if (collected >= 1)
            {
                fill += 0.04f;     // revealed but not complete
            }
        }

        if (fill >= 0.99f)
            fill = 1f;

        if (progressBar != null)
            progressBar.fillAmount = fill;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(fill * 100f) + "% Treat Completion";
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
