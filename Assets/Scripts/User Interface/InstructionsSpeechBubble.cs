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
                speechBubble.setDialogueText("SUGAR!    SPEED!    MAGIC!");
                speechBubble.setBubbleType(SpeechBubbleType.Stress);
                break;
            case 1:
                speechBubble.setDialogueText("Click and hold to charge the crystal \n Release to LAUNCH");
                speechBubble.setBubbleType(SpeechBubbleType.Yell);
                LaunchIcon.SetActive(true);
                BoostIcon.SetActive(false);
                break;
            case 2:
                speechBubble.setDialogueText("Click anywhere to cast Flambé \n Use to Flame-jump or burn through foes");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                LaunchIcon.SetActive(false);
                BoostIcon.SetActive(true);
                break;
            case 3:
                speechBubble.setDialogueText("Mind the ground and ceiling");
                speechBubble.setBubbleType(SpeechBubbleType.Yell);
                BoostIcon.SetActive(false);
                HealthBar.SetActive(true);
                break;
            case 4:
                speechBubble.setDialogueText("Grab upgrades after each run.");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                HealthBar.SetActive(false);
                GoldIcon.SetActive(true);
                break;
            case 5:
                speechBubble.setDialogueText("Collect every sweet this world has to offer!");
                speechBubble.setBubbleType(SpeechBubbleType.Stress);
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
