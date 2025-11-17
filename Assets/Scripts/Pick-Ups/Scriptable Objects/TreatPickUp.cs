using UnityEngine;

public enum TreatType
{
    Candy, // 1
    Donuts,
    Cupcake,
    Cake,
    CheeseCake,
    CakeDeluxe,
    Pie,
    Chocolate,
    Jelly // 9
}

[CreateAssetMenu(fileName = "New Treat", menuName = "Treats/Treat PickUp")]
public class TreatPickUp : ScriptableObject
{
    [Header("Static Data")]
    public string nameOfTreat;

    [TextArea] public string hintText;

    [TextArea] public string statsText;

    public Sprite[] allTreatSprites;

    public Sprite treatUnlocked;

    public int amountCollected;

    public float improvementModifier;

    public bool hasBeenCollected;

    public TreatType treatType;
}