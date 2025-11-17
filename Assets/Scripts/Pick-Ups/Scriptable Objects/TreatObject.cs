using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreatObject : MonoBehaviour
{
    [Header("Data")]
    public TreatPickUp treatPickUp;

    [Header("UI")]
    public TMP_Text hintText;
    public TMP_Text statsText;
    public Image activeSprite;
    public Sprite locked;
    public Sprite unlocked;
    public Sprite treatSprite;

    [Header("Variables")]
    public bool hasBeenCollected;
    public int amountCollected;  // convert this to a TMP_Text.text to update visual count
    

    private void Start()
    {
        if (treatPickUp != null)
            SetupTreat(treatPickUp);
    }

    public void SetupTreat(TreatPickUp treat)
    {
        Debug.Log("Setup Treat called");
        hasBeenCollected = false;
        amountCollected = 0;
        activeSprite.sprite = locked;
        switch (treat.treatType)
        {
            case TreatType.Candy:
                {
                        activeSprite.sprite = treat.allTreatSprites[0];
                }
                break;
            case TreatType.Donuts:
                {
                        activeSprite.sprite = treat.allTreatSprites[1];
                }
                break;
            case TreatType.Cupcake:
                {
                    activeSprite.sprite = treat.allTreatSprites[2];
                }
                break;
            case TreatType.Cake:
                {
                    activeSprite.sprite = treat.allTreatSprites[3];
                }
                break;
            case TreatType.CheeseCake:
                {
                    activeSprite.sprite = treat.allTreatSprites[4];
                }
                break;
            case TreatType.CakeDeluxe:
                {
                    activeSprite.sprite = treat.allTreatSprites[5];
                }
                break;
            case TreatType.Pie:
                {
                    activeSprite.sprite = treat.allTreatSprites[6];
                }
                break;
            case TreatType.Chocolate:
                {
                    activeSprite.sprite = treat.allTreatSprites[7];
                }
                break;
            case TreatType.Jelly:
                {
                    activeSprite.sprite = treat.allTreatSprites[8];
                }
                break;
        }
        treatSprite = treatPickUp.treatUnlocked;
        Debug.Log(treatSprite.name.ToString());
        UpdateCompendium();
    }

    public void IncrementTotalCollected()
    {
        amountCollected += 1;
    }

    private void UpdateCompendium()
    {
        Debug.Log("UpdateCompendium is running");
    }

    public int ConfirmTreatCollection() => amountCollected;
}