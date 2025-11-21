using UnityEngine;
using UnityEngine.UI;

public class ManaPool : MonoBehaviour
{
    public Image manaFill; // your red rectangles or blue bar

    public void UpdateMana(float fillAmount)
    {
        manaFill.fillAmount = fillAmount;
    }
}