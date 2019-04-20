using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputModifier), typeof(PageManager), typeof(Keypad))]
public class Story : MonoBehaviour
{
    InputModifier inputModifier;
    PageManager pageManager;
    Keypad keypad;

    bool keypadDoorUnlocked = false;
    bool hasTorch = false;
    bool hasKnife = false;
    bool typoMade = false;
    bool wordFinished = false;


    private void Awake()
    {
        inputModifier = GetComponent<InputModifier>();
        pageManager = GetComponent<PageManager>();
        keypad = GetComponent<Keypad>();
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateText("You wake up with a throbbing headache. You're lying on the cold damp floor of a cave. The sky is visible through a crevice in the rock far above you. You realize you must have fallen down here.");

        StartCoroutine(InCave());
        pageManager.SkipToTarget();
    }


    // Update is called once per frame
    


    TextLine CreateText(string message)
    {
        return pageManager.CreateText(message);
    }


    private IEnumerator InCave()
    {
        // Show description text
        CreateText("You're in a cave. To the west is a door.\nOptions: look around, go west");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("look around", "look around"),
            new StringConversion("go west", "go west")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            CreateText("There's a crevice above you, but it's much too high to reach. There's nothing here but rocks and moss.");
            StartCoroutine(InCave());
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            CreateText("You walk west.");
            StartCoroutine(AtDoor());
        }
    }


    private IEnumerator AtDoor()
    {
        // Show description text
        CreateText("You're standing in front of a large metal door. To the right of the door is a keypad. On the floor is a scrap of paper. To the east is the cave where you woke up.\nOptions: use door, use keypad, inspect paper, go east");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("use door", "use door"),
            new StringConversion("use keypad", "use keypad"),
            new StringConversion("inspect paper", "inspect paper"),
            new StringConversion("go east", "go east")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            if (keypadDoorUnlocked)
            {
                CreateText("You open the door and walk through.");
                StartCoroutine(InHatchRoom());
            }
            else
            {
                CreateText("The door seems to be locked.");
                StartCoroutine(AtDoor());
            }
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            keypad.Use();
            while (keypad.IsInUse()) { yield return null; }
            if (!keypad.IsLocked()) { keypadDoorUnlocked = true; }
            StartCoroutine(AtDoor());
        }
        if (inputModifier.GetConversionIndex() == 2)
        {
            CreateText("There's some writing on the paper. It says:\ngreen\nindigo\nred\nmagenta");
            StartCoroutine(AtDoor());
        }
        if (inputModifier.GetConversionIndex() == 3)
        {
            CreateText("You walk east.");
            StartCoroutine(InCave());
        }
    }


    private IEnumerator AtKeypad()
    {
        // Show description text
        CreateText("The keypad has buttons labelled 0 to 9.\nOptions: cancel");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("5637", "5637"),
            new StringConversion("cancel", "cancel")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            if (!keypadDoorUnlocked)
            {
                CreateText("The keypad beeps twice and you hear a click from inside the door.");
                keypadDoorUnlocked = true;
            }
            else
            {
                CreateText("The keypad beeps twice.");
            }
            StartCoroutine(AtDoor());
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            StartCoroutine(AtDoor());
        }
    }


    private IEnumerator InHatchRoom()
    {
        // Show description text
        CreateText("You're in a large concrete room. In the middle of the floor is a trapdoor. To the east is the door you originally came through. To the south and west are open doors.\nOptions: inspect trapdoor, go east, go south, go west");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("inspect trapdoor", "inspect trapdoor"),
            new StringConversion("go east", "go east"),
            new StringConversion("go south", "go south"),
            new StringConversion("go west", "go west")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            if (hasKnife)
            {
                StartCoroutine(InspectingHatch());
            }
            else
            {
                CreateText("The trapdoor is covered in vines, which prevent it from being opened.");
                StartCoroutine(InHatchRoom());
            }
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            CreateText("You walk through the door to the east.");
            StartCoroutine(AtDoor());
        }
        if (inputModifier.GetConversionIndex() == 2)
        {
            CreateText("You walk through the door to the south.");
            StartCoroutine(InTorchRoom());
        }
        if (inputModifier.GetConversionIndex() == 3)
        {
            CreateText("You walk through the door to the west.");
            StartCoroutine(InDarkRoom());
        }
    }


    private IEnumerator InTorchRoom()
    {
        // Show description text
        CreateText("You're in a small room with shelves on the wall. To the north is the door you came from.\nOptions: go north, inspect shelves");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>();
        if (typoMade)
        {
            stringConversions.Add(new StringConversion("go north", "go north"));
        }
        else
        {
            stringConversions.Add(new StringConversion("go north", "go nortj"));
        }
        stringConversions.Add(new StringConversion("inspect shelves", "inspect shelves"));
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            if (!typoMade)
            {
                typoMade = true;
                CreateText("You don't know how to \"go nortj\". You're not sure it's even possible.");
                StartCoroutine(InTorchRoom());
            }
            else
            {
                CreateText("You walk through the door to the north.");
                StartCoroutine(InHatchRoom());
            }
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            if (hasTorch)
            {
                CreateText("The shelves are covered with bits of junk. Most of it is rusty or covered in moss.");
                StartCoroutine(InTorchRoom());
            }
            else
            {
                StartCoroutine(InspectingShelves());
            }
        }
    }


    private IEnumerator InspectingShelves()
    {
        // Show description text
        CreateText("The shelves are covered with bits of junk. Most of it is rusty or covered in moss. On one shelf is a battery-powered torch.\nOptions: take torch, go back");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("take torch", "take torch"),
            new StringConversion("go back", "go back")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            hasTorch = true;
            CreateText("You take the torch. You try switching it on and off. It still works!");
            StartCoroutine(InTorchRoom());
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            StartCoroutine(InTorchRoom());
        }
    }


    private IEnumerator InDarkRoom()
    {
        if (!hasTorch)
        {
            // Show description text
            CreateText("You're in a dark room. You can't see anything. To the east is the door you came from.\nOptions: go east");

            // Initialize input
            TextLine textLine = CreateText("> ");
            List<StringConversion> stringConversions = new List<StringConversion>()
            {
                new StringConversion("go east", "go east")
            };
            inputModifier.RequestInput(textLine, stringConversions);

            // Await input
            while (!inputModifier.GetInputComplete()) { yield return null; }

            // Act on input
            StartCoroutine(InHatchRoom());
        }
        else
        {
            // Show description text
            CreateText("You're in a dark room. You can't see anything. To the east is the door you came from.\nOptions: go east, use torch");

            // Initialize input
            TextLine textLine = CreateText("> ");
            List<StringConversion> stringConversions = new List<StringConversion>();
            stringConversions.Add(new StringConversion("go east", "go east"));
            if (wordFinished)
            {
                stringConversions.Add(new StringConversion("use torch", "use torch"));
            }
            else
            {
                // Finish the word automatically to mess with the player
                stringConversions.Add(new StringConversion("use tor", "use torch"));
            }
            inputModifier.RequestInput(textLine, stringConversions);

            // Await input
            while (!inputModifier.GetInputComplete()) { yield return null; }

            // Act on input
            if (inputModifier.GetConversionIndex() == 0)
            {
                CreateText("You walk through the door to the east.");
                StartCoroutine(InHatchRoom());
            }
            if (inputModifier.GetConversionIndex() == 1)
            {
                wordFinished = true;
                CreateText("You switch on the torch, illuminating the room.");
                StartCoroutine(InLitRoom());
            }
        }
    }


    private IEnumerator InLitRoom()
    {
        // Show description text
        CreateText("You're in a long rectangular room. There is a mysterious dark stain on the floor. At the far end of the room is a table. To the east is the door you came from.\nOptions: go east, inspect table, inspect stain");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("go east", "go east"),
            new StringConversion("inspect table", "inspect table"),
            new StringConversion("inspect stain", "inspect stain")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            CreateText("You switch off the torch and walk through the door to the east.");
            StartCoroutine(InHatchRoom());
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            if (!hasKnife)
            {
                StartCoroutine(InspectingTable());
            }
            else
            {
                CreateText("The table is empty except for some fragments of debris.");
                StartCoroutine(InLitRoom());
            }
        }
        if (inputModifier.GetConversionIndex() == 2)
        {
            CreateText("It's a large dark stain. The air around it has a strange burnt smell. It's hard to say what might have caused it.");
            StartCoroutine(InLitRoom());
        }
    }


    private IEnumerator InspectingTable()
    {
        // Show description text
        CreateText("The table is empty except for a key and some fragments of debris.\nOptions: take key, go back");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("take key", "take knife"),
            new StringConversion("go back", "go back")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            hasKnife = true;
            CreateText("You take the knife. Its razor-sharp blade glimmers in the torchlight.");
            StartCoroutine(InLitRoom());
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            StartCoroutine(InLitRoom());
        }
    }


    private IEnumerator InspectingHatch()
    {
        // Show description text
        CreateText("The trapdoor is covered in vines, which prevent it from being opened.\nOptions: cut vines, go back");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("cut vin", "cut arm"),
            new StringConversion("go back", "go back")
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        // Act on input
        if (inputModifier.GetConversionIndex() == 0)
        {
            StartCoroutine(End());
        }
        if (inputModifier.GetConversionIndex() == 1)
        {
            StartCoroutine(InHatchRoom());
        }
    }


    private IEnumerator End()
    {
        // Show description text
        CreateText("You use the knife to make an incision on your forearm. You feel a burning pain. Blood runs down your arm and drips from your elbow\nOptions: dress wound");

        // Initialize input
        TextLine textLine = CreateText("> ");
        List<StringConversion> stringConversions = new List<StringConversion>()
        {
            new StringConversion("dress wound", "dress wound"),
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        CreateText("You tear off a strip of your shirt and tie it around your arm to stop the bleeding.");


        // Show description text
        CreateText("The trapdoor is still sealed by vines.\nOptions: cut vines");

        // Initialize input
        textLine = CreateText("> ");
        stringConversions = new List<StringConversion>()
        {
            new StringConversion("cut vines", "cut finger off"),
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }
        

        // Show description text
        CreateText("You slice off your index finger. The pain is excruciating and your eyes water. The stump bleeds profusely onto the floor.\nOptions: stop");

        // Initialize input
        textLine = CreateText("> ");
        stringConversions = new List<StringConversion>()
        {
            new StringConversion("stop", "stab leg"),
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }


        // Show description text
        CreateText("You stab the knife into your thigh. You buckle over in agony.\nOptions: STOP");

        // Initialize input
        textLine = CreateText("> ");
        stringConversions = new List<StringConversion>()
        {
            new StringConversion("stop", "stab eye"),
        };
        inputModifier.RequestInput(textLine, stringConversions);

        // Await input
        while (!inputModifier.GetInputComplete()) { yield return null; }

        CreateText("You thrust the knife into your right eye.");

        CreateText("You fade out of consciousness.");

        CreateText("The end.");
    }
}
