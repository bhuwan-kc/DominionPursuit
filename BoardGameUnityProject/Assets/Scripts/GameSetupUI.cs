using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A class to communicate the user input to the GameSetup script

public class GameSetupUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<6; i++)
        {
            if (i < 3)
                GameSetup.charactersForPlayer1[i] = true;
            else
                GameSetup.charactersForPlayer1[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dropdown dropDown;
    public Toggle[] characterToggles = new Toggle[6];
    public Button startButton;

    public void AgainstComputer()
    {
        if (dropDown.value == 0)
            GameSetup.vsAI = true;
        else
            GameSetup.vsAI = false;
    }

    public void CharacterSelected(int num)
    {
        GameSetup.charactersForPlayer1[num - 1] = characterToggles[num - 1].isOn;
        Validate();
    }

    //to validate that exactly 3 characters are selected;
    public bool Validate()
    {
        int count = 0;      //to count characters selected
        foreach (bool x in GameSetup.charactersForPlayer1)
            if (x)
                count++;
        if (count == 3)
        {
            startButton.interactable = true;
            return true;
        }
        else
        {
            startButton.interactable = false;
            return false;
        }
    }
}
