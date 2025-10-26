using UnityEngine;
using SpeechBubble;
using System.Linq;

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
        if (counter != 11) { NextButton.SetActive(false); }
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
        counter = Mathf.Clamp(counter, 0, 11);
        speechBubble.setBubbleType(SpeechBubbleType.Think);

        switch (counter)
        {
            case 0:
                speechBubble.setDialogueText("How to Play");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 1:
                speechBubble.setDialogueText("Launch!\r\n\nFire me as far as possible!");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 2:
                speechBubble.setDialogueText("Boost\r\n\nClick on me while flying while your boost meter is full to give a boost!\n");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                BoostIcon.SetActive(true);
                break;
            case 3:
                speechBubble.setDialogueText("Collect \r\nSnatch tasty treats mid-flight to recover health and earn bonus money");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 4:
                speechBubble.setDialogueText("Upgrade \r\nSpend your gold in the upgrades shop to make me stronger!");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                BoostIcon.SetActive(false);
                break;
            case 5:
                speechBubble.setDialogueText("Repeat\r\nLaunch farther, earn more, and discover new treats!");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 6:
                speechBubble.setDialogueText("Discover All Sweet Treats\r\nCollect each unique treat during your flights to win!");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 7:
                speechBubble.setDialogueText("Playing the Game");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 8:
                speechBubble.setDialogueText("Hold down left click to charge power, release at high power.\n Left click during flight explode your way even further.");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 9:
                speechBubble.setDialogueText("Enemies offer a small boost but hurt a little .\r\nHitting the ground hurts a lot more.");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                HealthBar.SetActive(true);
                break;
            case 10:
                speechBubble.setDialogueText("Get cash for distance traveled, enemies vanquished, and treats snatched.\r\nPurchase upgrades after a run to become more powerful..");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                break;
            case 11:
                speechBubble.setDialogueText("Enjoy!");
                speechBubble.setBubbleType(SpeechBubbleType.Think);
                HealthBar.SetActive(false);
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
        if (counter == 11) { NextButton.SetActive(true); }
    }


}
