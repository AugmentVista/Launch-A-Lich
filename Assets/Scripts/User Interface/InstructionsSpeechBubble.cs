using UnityEngine;
using SpeechBubble;

public class InstructionsSpeechBubble : MonoBehaviour
{
    private int counter = 0;

    [SerializeField] GameObject NextButton;

    [SerializeField] GameObject HealthBar;

    [SerializeField] GameObject BoostIcon;

    [SerializeField]
    private SpeechBubble_TMP speechBubble;

    [SerializeField]
    private SpeechBubbleStyle defaultStyle;
    [SerializeField]
    private SpeechBubbleStyle darkStyle;

    void Start()
    {
        showCorrectSlide();
        if (counter != 6) { NextButton.SetActive(false); }
        BoostIcon.SetActive(false);
        HealthBar.SetActive(false);
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
        counter = Mathf.Clamp(counter, 0, 6);
        speechBubble.setBubbleType(SpeechBubbleType.Think);

        switch (counter)
        {
            case 0:
                speechBubble.setDialogueText("Launch!\n Boost\n Collect\n Upgrade\n Repeat\n Reach 5000m");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 1:
                speechBubble.setDialogueText("Click while flying to use your boost!\n");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                BoostIcon.SetActive(true);
                break;
            case 2:
                speechBubble.setDialogueText("Playing the Game");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                BoostIcon.SetActive(false);
                break;
            case 3:
                speechBubble.setDialogueText("Hold down left click to charge power, release at high power.");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 4:
                speechBubble.setDialogueText("Enemies hurt a little\n The ground hurts a lot");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                HealthBar.SetActive(true);
                break;
            case 5:
                speechBubble.setDialogueText("Get cash from distance, enemies, and treats");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                HealthBar.SetActive(false);
                break;
            case 6:
                speechBubble.setDialogueText("Purchase upgrades and break records to unlock more treats");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
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
        if (counter == 6) { NextButton.SetActive(true); }
    }


}
