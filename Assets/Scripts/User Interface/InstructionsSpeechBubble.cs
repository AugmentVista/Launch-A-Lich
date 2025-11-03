using UnityEngine;
using SpeechBubble;

public class InstructionsSpeechBubble : MonoBehaviour
{
    private int counter = 0;

    [SerializeField] GameObject NextButton;

    [SerializeField] GameObject HealthBar;

    [SerializeField] GameObject BoostIcon;

    [SerializeField] GameObject LaunchIcon;

    [SerializeField] GameObject GoldIcon;

    [SerializeField]
    private SpeechBubble_TMP speechBubble;

    [SerializeField]
    private SpeechBubbleStyle defaultStyle;
    [SerializeField]
    private SpeechBubbleStyle darkStyle;

    private int dialogueCount = 5;

    void Start()
    {
        showCorrectSlide();
        if (counter != dialogueCount) { NextButton.SetActive(false); }
        BoostIcon.SetActive(false);
        HealthBar.SetActive(false);
        LaunchIcon.SetActive(false);
        GoldIcon.SetActive(false);
    }

    void OnEnable()
    {
        counter = 0;
    }
    private void OnDisable()
    {
        counter = 0;
    }

    private void showCorrectSlide()
    {
        counter = Mathf.Clamp(counter, 0, dialogueCount);
        speechBubble.setBubbleType(SpeechBubbleType.Think);

        switch (counter)
        {
            case 0:
                speechBubble.setDialogueText("Launch!\n Boost\n Collect\n Upgrade\n Repeat\n Reach 5000m");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 1:
                speechBubble.setDialogueText("Click me with a full meter to Boost!\n");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                BoostIcon.SetActive(true);
                break;
            case 2:
                speechBubble.setDialogueText("Click and hold Launch button to charge");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                BoostIcon.SetActive(false);
                LaunchIcon.SetActive(true);
                break;
            case 3:
                speechBubble.setDialogueText("Enemies hurt a little\n The Ground hurts a lot");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                LaunchIcon.SetActive(false);
                HealthBar.SetActive(true);
                break;
            case 4:
                speechBubble.setDialogueText("Height, distance, enemies, and treats = $$$");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                HealthBar.SetActive(false);
                GoldIcon.SetActive(true);
                break;
            case 5:
                speechBubble.setDialogueText("Purchase upgrades go even farther beyond!");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                GoldIcon.SetActive(false);
                Proceed();
                break;
        }
    }

    public void nextButtonPushed()
    {
        counter += 1;
        showCorrectSlide();
    }

    public void previousButtonPushed()
    {
        counter -= 1;

        showCorrectSlide();
    }

    public void Proceed()
    {
        if (counter == dialogueCount) { NextButton.SetActive(true); }
    }


}
