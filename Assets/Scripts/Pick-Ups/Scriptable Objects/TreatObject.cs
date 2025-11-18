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
    

    private void Awake()
    {
        if (treatPickUp != null)
            SetupTreat();
    }

    public void SetupTreat()
    {
        Debug.Log("Setup Treat called");
        hasBeenCollected = false;
        amountCollected = 0;
        activeSprite.sprite = locked;
        switch (treatPickUp.treatType)
        {
            case TreatType.Candy:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[0];
                }
                break;
            case TreatType.Donuts:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[1];
                }
                break;
            case TreatType.Cupcake:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[2];
                }
                break;
            case TreatType.Cake:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[3];
                }
                break;
            case TreatType.CheeseCake:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[4];
                }
                break;
            case TreatType.CakeDeluxe:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[5];
                }
                break;
            case TreatType.Pie:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[6];
                }
                break;
            case TreatType.Chocolate:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[7];
                }
                break;
            case TreatType.Jelly:
                {
                    activeSprite.sprite = treatPickUp.allTreatSprites[8];
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