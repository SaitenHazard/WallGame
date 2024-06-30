using System.Collections.Generic;
using Enemies;
using Input;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public GameObject continueButton;

    public int currentIndex = -1;

    private readonly List<string> _texts = new List<string>{
        "You take control of Wallther, the ridiculously buff repairman.",
        "Your job is to keep the wall in shape in the midst of an enemy siege.",
        "If you do your job well, the crossbowmen on the walls can do their job and eliminate the enemy army.",
        "Move with the left stick, Jump with A.",
        "In order to repair a wall piece, you need stones. Pick them up from the masonry on the bottom right using [B]\n\nPress [Y] when you picked up some stones.",
        "Mount the wall using L-Stick and [A] to jump until you reach a broken wall piece.\n\nPress [Y] when you reach the broken wall piece.",
        "Now stand in front of the damaged wall and repair it using the right shoulder Button [RB]\n\nPress [Y] when you repaired it.",
        "The Wooden Scaffolding works similarly. Go pick up wood from Wallter's shed on the botton left!]\n\nPress [Y] when you have some wooden planks on your back.",
        "To get to any piece of the wall quickly, use the Catapult on the bottom center. Enter it with [B], aim with L-Stick and fire with [B]",
        "Scaffolding is repaired using the left shoulder button [LB]\n\nPress [Y] when you repaired a piece of scaffolding.",
        "The puny crossbowmen will only reach their slot if the scaffolding on the way and their wall piece are in tiptop shape!",
        "Also, take care! The wall gets damaged more than 50% you will lose and the enemy will take your city.",
        "Now you know everything you need to know, we will reset the level and let you play! Good luck! Don't let the wall falter!"
    };

    void Awake()
    {
        Inputs.YPressed += NextTutorialStep;
    }

    private void OnDestroy()
    {
        Inputs.YPressed -= NextTutorialStep;
    }

    public bool ableToContinue = true;

    private void NextTutorialStep()
    {
        if (!ableToContinue) return;

        ableToContinue = false;
        continueButton.SetActive(false);
        Invoke(nameof(AbleToContinue), 1.0f);

        currentIndex++;
        if (currentIndex == 5)
        {
            ArmyController.instance.SetTargetingScheme(TargetingScheme.Random_NoWallTwice);
        }
        if (currentIndex == _texts.Count)
        {
            SceneManager.LoadScene("Tobi");
        }
        textMeshPro.text = _texts[currentIndex];
    }

    private void AbleToContinue()
    {
        ableToContinue = true;
        continueButton.SetActive(true);
    }
}
