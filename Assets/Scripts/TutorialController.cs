using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Enemies;
using Input;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wall;

public class TutorialController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public GameObject continueButton;
    public ThirdPersonController player;

    public int currentIndex = -1;

    private List<TutorialStep> _texts;

    private bool CheckWalltherHasStones() { return player.stone != 0; }
    private bool CheckWalltherHasWood() { return player.wood != 0; }
    private bool CheckWallHealthMax() { 
        return WallManager.instance._wallHealth == WallManager.instance._maxWallHealth;
    }
    private bool CheckWalltherInsideCatapult() { return player.inCatapult; }
    private void HaltEnemyAttack()
    {
        ArmyController.instance.SetTargetingScheme(TargetingScheme.HoldFire);
    }

    void Awake()
    {
        Inputs.YPressed += IfPossibleNext;
        player = FindObjectOfType<ThirdPersonController>();
    }

    private void Start()
    {
        _texts = new List<TutorialStep>{
        new TutorialStep("You take control of Wallther, the ridiculously buff repairman."),
        new TutorialStep("Your job is to keep the wall in shape in the midst of an enemy siege."),
        new TutorialStep("If you do your job well, the crossbowmen on the walls can do their job and eliminate the enemy army."),
        new TutorialStep("Move with the left stick, Jump with A."),
        new TutorialStep("Watch out! The enemy has commenced their attack!"
            , new Func<int>(() => { ArmyController.instance.SetTargetingScheme(TargetingScheme.Succession); Invoke(nameof(HaltEnemyAttack), 2.5f); return 0; })
            , new Func<bool>(() => { return !CheckWallHealthMax(); })),
        new TutorialStep("A trebuchet round hit the wall! To repair it, you first need stones. Pick them up from the masonry on the bottom right using [B]"
            , new Func<bool>(() => { return CheckWalltherHasStones(); })),
        new TutorialStep("Good job! Now make your way up there and repair the broken wall piece using the upper right shoulder button [RB]."
            , new Func<bool>(() => { return CheckWallHealthMax(); })),
        new TutorialStep("Now that the wall is safe again, a well-rested crossbowman will take the place of his fallen comrade and continue to return fire."),
        new TutorialStep("By the gods, fire arrows!\nThe scaffolding is damaged! Go pick up wood from Wallter's shed on the botton left unsing [B]"
            , new Func<int>(() => { ArmyController.instance.SetTargetingScheme(TargetingScheme.Random_NoWallTwice);  ArmyController.instance.LaunchFireArrows(); ArmyController.instance.SetTargetingScheme(TargetingScheme.HoldFire); return 0; })
            , new Func<bool>(() => { return CheckWalltherHasWood(); })),
        new TutorialStep("No time to waste! To get to any piece of the wall quickly, Wallther can launch himself using the catapult. Stand next to it and use [B] to enter."
            , new Func<bool>(() => { return CheckWalltherInsideCatapult(); })),
        new TutorialStep("Now aim using the left stick and launch yourself with [B]!"
            , new Func<bool>(() => { return !CheckWalltherInsideCatapult(); })),
        new TutorialStep("The Wooden scaffolding is repaired using the left shoulder button [LB]."),
        new TutorialStep("If the wall gets damaged more than 50% you will lose and the enemy will take your city."),
        new TutorialStep("If the crossbowmen are able to repel the attack, you win! You can tell how many soldiers the enemy has left with the amount of flags on the horizon."),
        new TutorialStep("Now you know everything you need to know, we will reset the level and let you play! Good luck! Don't let the wall falter!")
    };
    }

    private void IfPossibleNext()
    {
        if (currentIndex == -1 || _texts[currentIndex].skippable) NextTutorialStep();
    }

    private void OnDestroy()
    {
        Inputs.YPressed -= IfPossibleNext;
    }

    public bool ableToContinue = true;

    private void NextTutorialStep()
    {
        if ((currentIndex == -1 || _texts[currentIndex].skippable) && !ableToContinue) return;

        ableToContinue = false;
        continueButton.SetActive(false);
        Invoke(nameof(AbleToContinue), .5f);

        currentIndex++;
        
        
        if (currentIndex == _texts.Count)
        {
            SceneManager.LoadScene("Tobi");
        }
        if (-1 < currentIndex && currentIndex < _texts.Count)
        {
            _texts[currentIndex].onEnter?.Invoke();
            textMeshPro.text = _texts[currentIndex].tutorialmessage;
        }
    }

    private void AbleToContinue()
    {
        if (_texts[currentIndex].skippable)
        {
            ableToContinue = true;
            continueButton.SetActive(true);
        }
    }

    public void Update()
    {
        if (-1 < currentIndex && currentIndex < _texts.Count && !_texts[currentIndex].skippable && _texts[currentIndex].condition.Invoke()) NextTutorialStep();
    }
}

internal class TutorialStep
{
    public TutorialStep(string _message, Func<bool> _condition)
    {
        tutorialmessage = _message;
        skippable = false;
        condition = _condition;
        onEnter = new Func<int>(() => { return 0; });
    }
    public TutorialStep(string _message, Func<int> _onEnter, Func<bool> _condition)
    {
        tutorialmessage = _message;
        skippable = false;
        condition = _condition;
        onEnter = _onEnter;
    }

    public TutorialStep(string _message)
    {
        tutorialmessage = _message;
        skippable = true;
        condition = new Func<bool>(() => { return false; });
        onEnter = new Func<int>(() => { return 0; });
    }

    public string tutorialmessage;
    public bool skippable;
    public Func<bool> condition;
    public Func<int> onEnter;
}

