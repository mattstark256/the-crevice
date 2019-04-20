using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputModifier), typeof(PageManager))]
public class Keypad : MonoBehaviour
{
    [SerializeField]
    private string correctCode = "5637";

    private int maxFailedAttempts = 4;

    InputModifier inputModifier;
    PageManager pageManager;

    private bool inUse = false;
    public bool IsInUse() { return inUse; }
    private bool locked = true;
    public bool IsLocked() { return locked; }


    private void Awake()
    {
        inputModifier = GetComponent<InputModifier>();
        pageManager = GetComponent<PageManager>();
    }


    public void Use()
    {
        if (locked)
        {
            inUse = true;
            StartCoroutine(UseCoroutine());
        }
        else
        {
            pageManager.CreateText("The keypad appears to have stopped working.");
        }
    }
    private IEnumerator UseCoroutine()
    {
        pageManager.CreateText("The keypad has buttons labelled 0 to 9 and a small screen.\nOptions: 0-9, cancel");

        bool quitting = false;
        string enteredCode = "";
        int failedAttempts = 0;
        while (!quitting)
        {
            // Initialize input
            TextLine textLine = pageManager.CreateText("> ");
            List<StringConversion> stringConversions = new List<StringConversion>()
            {
                new StringConversion("0", "0"),
                new StringConversion("1", "1"),
                new StringConversion("2", "2"),
                new StringConversion("3", "3"),
                new StringConversion("4", "4"),
                new StringConversion("5", "5"),
                new StringConversion("6", "6"),
                new StringConversion("7", "7"),
                new StringConversion("8", "8"),
                new StringConversion("9", "9"),
                new StringConversion("cancel", "cancel")
            };
            inputModifier.RequestInput(textLine, stringConversions);

            // Await input
            while (!inputModifier.GetInputComplete()) { yield return null; }

            // Act on input
            if (inputModifier.GetConversionIndex() == 10)
            {
                quitting = true;
            }
            else
            {
                string enteredDigit = stringConversions[inputModifier.GetConversionIndex()].typedString;
                enteredCode += enteredDigit;
                if (enteredCode.Length == 4)
                {
                    if (enteredCode == correctCode)
                    {
                        pageManager.CreateText("The screen now reads " + enteredCode + ". The keypad beeps twice and you hear a clicking noise from inside the door. You've unlocked it!");
                        locked = false;
                        quitting = true;
                    }
                    else
                    {
                        pageManager.CreateText("The screen now reads " + enteredCode + ". The keypad buzzes and nothing happens. That wasn't the right code.\nOptions: 0-9, cancel");
                        enteredCode = "";
                        failedAttempts++;
                    }
                }
                // After several attempts the player just has to type 1 digit.
                else if (enteredCode.Length == 1 && failedAttempts == maxFailedAttempts)
                {
                    pageManager.CreateText("The screen now reads " + enteredCode + ". The keypad beeps twice and you hear a clicking noise from inside the door. You've unlocked it! You're somewhat surprised that the code was just " + enteredCode + "...");
                    locked = false;
                    quitting = true;
                }
                else
                {
                    pageManager.CreateText("The screen now reads " + enteredCode + ".\nOptions: 0-9, cancel");
                }
            }
        }

        inUse = false;
    }
}
