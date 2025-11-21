using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TreatToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TreatObject treatObject;

    public Image infoBackgroundImage;

    public TMP_Text infoText;

    public bool hintOnHoverEnabled;
    public bool statsOnHoverEnabled;


    private void Update()
    {
        TreatsVisualManagement();
    }

    public void TreatsVisualManagement()
    {
        if (treatObject.ConfirmTreatCollection() >= 5)
        {
            treatObject.activeSprite.sprite = treatObject.treatSprite;
            hintOnHoverEnabled = false;
            statsOnHoverEnabled = true;
        }
        else if (treatObject.ConfirmTreatCollection() >= 1)
        {
            treatObject.activeSprite.sprite = treatObject.unlocked;
            hintOnHoverEnabled = true;
            return;
        }
        else
        {
            treatObject.activeSprite.sprite = treatObject.locked;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        TreatPickUp treatPickUp = treatObject.treatPickUp;

        infoBackgroundImage.gameObject.SetActive(true);

        if (statsOnHoverEnabled)
        {
            infoText.text = treatPickUp.statsText + $" collected: {treatObject.amountCollected}";
        }
        else if (hintOnHoverEnabled)
        {
            infoText.text = treatPickUp.hintText + $" Find {5 - treatObject.amountCollected} to unlock";
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        infoText.text = "";
        infoText.text = "";
        infoBackgroundImage.gameObject.SetActive(false);
    }

}
