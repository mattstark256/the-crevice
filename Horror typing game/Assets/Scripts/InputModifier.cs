using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputModifier : MonoBehaviour
{
    private Text text;

    private string typedString = "";
    private string displayedString;

    private TextLine textLine;
    private List<StringConversion> stringConversions = new List<StringConversion>();

    // This is the index of the coversion that is currently being used.
    int conversionIndex = 0;
    public int GetConversionIndex() { return conversionIndex; }

    private bool inputComplete = false;
    public bool GetInputComplete() { return inputComplete; }


    float cursorTimer = 0;
    bool cursorVisible = false;
    float cursorDuration = 0.7f;


    private void LateUpdate()
    {
        // Show text cursor
        if (textLine != null)
        {
            string s = "> " + displayedString;

            if (cursorVisible)
            {
                cursorTimer -= Time.deltaTime;
                if (cursorTimer <= 0) cursorTimer += cursorDuration;
                if (cursorTimer > cursorDuration / 2) s += "_";
            }

            textLine.SetString(s);
        }
    }


    public void RequestInput(TextLine textLine, List<StringConversion> stringConversions)
    {
        this.textLine = textLine;
        this.stringConversions = stringConversions;
        typedString = "";
        inputComplete = false;
        StartCoroutine(InputCoroutine());
    }


    private IEnumerator InputCoroutine()
    {
        bool typingCompleted = false;
        cursorVisible = true;
        while (!typingCompleted)
        {
            HandleInput();
            conversionIndex = GetClosestMatch(typedString);
            displayedString = ConvertString(typedString, stringConversions[conversionIndex]);
            if (stringConversions[conversionIndex].typedString != "" && typedString == stringConversions[conversionIndex].typedString) { typingCompleted = true; }
            yield return null;
        }

        // Finish off whatever the player has typed
        for (int i = displayedString.Length; i < stringConversions[conversionIndex].displayedString.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            displayedString += stringConversions[conversionIndex].displayedString[i];
            cursorTimer = cursorDuration;
        }

        cursorVisible = false;
        yield return new WaitForSeconds(0.3f);
        inputComplete = true;
    }


    // Convert string s1 using conversion stringConversion.
    private string ConvertString(string s1, StringConversion stringConversion)
    {
        string s2 = "";
        bool mismatchFound = false;
        for (int i = 0; i < s1.Length; i++)
        {
            if (!mismatchFound)
            {
                if (i < stringConversion.typedString.Length && s1[i] == stringConversion.typedString[i])
                {
                    s2 += stringConversion.displayedString[i];
                }
                else
                {
                    mismatchFound = true;
                }
            }
            if (mismatchFound)
            {
                s2 += s1[i];
            }
        }
        return s2;
    }


    // Return the StringConversion that most closely matches the start of string s.
    private int GetClosestMatch(string s)
    {
        int closestMatch = 0;
        int maxMatchedCharacters = 0;
        for (int i = 0; i < stringConversions.Count; i++)
        {
            int matchedCharacters = 0;
            int charactersToCompare = Mathf.Min(stringConversions[i].typedString.Length, s.Length);
            for (int j = 0; j < charactersToCompare; j++)
            {
                if (s[j] == stringConversions[i].typedString[j])
                {
                    matchedCharacters++;
                    if (matchedCharacters > maxMatchedCharacters)
                    {
                        maxMatchedCharacters = matchedCharacters;
                        closestMatch = i;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        return closestMatch;
    }


    // Add any typed characters to inputString. Delete if backspace pressed.
    private void HandleInput()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (typedString.Length != 0)
                {
                    typedString = typedString.Substring(0, typedString.Length - 1);
                }
            }
            else if (c == '\n' || c == '\r')
            {
                Debug.Log("newline");
            }
            else
            {
                typedString += c;
            }

            // Any time a character is entered, the cursor should be made visible   
            cursorTimer = cursorDuration;
        }

        typedString = typedString.ToLower();
    }
}
