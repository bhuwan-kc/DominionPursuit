using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that co-ordinates communication between all the game objects in the scene
public class GameManager : MonoBehaviour
{
    //setting up a static instance of the class 
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                Debug.Log("GameManager object is null");
                return null;
            }
        }
    }

    private void Awake()
    {
        _instance = this;
    }




    //***************************************************************
    //************************* PROPERTIES **************************
    //***************************************************************

    private int totalTiles = 78;                //total number of tiles on the game board
    private float diceRollAnimTime = 1.0f;      //time that dice rolls for
    public int currentPlayer = 1;               //indicates turn --> 1 for player1, 2 for player2
    public bool vsAI = true;                    //if game is vs AI or not.
    private int diceSum = 0;                    //sum of numbers from the die
    private int TpresetdiceSum = 1;             //preset dice sum for next dice roll ***FOR TESTING ONLY***
    public bool canActivateEventCard = true;
    public int bonusSteps = 0;                  //Used for shortcut event card

    public GameObject AI;


    //***************************************************************
    //************************* METHODS *****************************
    //***************************************************************

    //------------------- GETTERS AND SETTERS -----------------------START

    public float getDiceRollAnimTime()
    {
        return diceRollAnimTime;
    }

    //------------------- GETTERS AND SETTERS -----------------------END

    //returns the transform property of the index tile 
    public Transform GetTilePosition(int index)
    {
        if(index >= 0 && index < totalTiles)
        {
            return ObjectHandler.Instance.tiles[index].transform;
        }

        //if index out of range 
        Debug.Log("Tile index "+index+" out of range - GetTilePosition - GameManager");
        return ObjectHandler.Instance.tiles[0].transform;
    }

    //to roll the dice when player clicks on the dice
    public void RollDice()
    {
        UIManager.Instance.DisableDice(true);
        canActivateEventCard = false;
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.rollDice);

        //to play the dice roll animation
        ObjectHandler.Instance.Dice.GetComponent<Dice>().DiceRollAnimation();
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().DiceRollAnimation();

        //to get the random dice output
        int diceOutput = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();
        int diceOutput2 = ObjectHandler.Instance.Dice2.GetComponent<Dice>().RollDice();

        //**************************************
        //******** FOR TESTING ONLY ************
        //**************************************
        if (TpresetdiceSum != 1)
        {
            diceOutput = TpresetdiceSum - 1;
            diceOutput2 = 1;
        }

        //to wait for dice roll and show a dice face
        StartCoroutine(WaitForDiceRollAnim(diceOutput, diceOutput2));
    }

    //to end a turn
    public void EndTurn(float waitTime)
    {
        StartCoroutine(EndTurnRoutine(waitTime));
    }

    public void EndTurn()
    {
        EndTurn(0.25f);
    }

    //to send signal to the character n about the diceSum 
    //n is the character number selected by the player for movement
    public void CharacterUpdateTile(int n)
    {
        if (currentPlayer == 1)
            ObjectHandler.Instance.player1Characters[n].GetComponent<Character>().UpdateTile(diceSum, true);
        else if (currentPlayer == 2)
            ObjectHandler.Instance.player2Characters[n].GetComponent<Character>().UpdateTile(diceSum, true);
    }

    public void SetDiceSum(int sum)
    {
        TpresetdiceSum = sum;
    }

    //------------------- IENUMERATOR -----------------------START

    //to set the dice face and send signal to allow choosing character for the movement
    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
        ObjectHandler.Instance.Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);
        diceSum = diceOutput + diceOutput2 + bonusSteps;    //adding bonusSteps, non zero when shortcut event card was played
        bonusSteps = 0;                                     //reseting the bonusSteps 

        //wait for some seconds and send the sum of the outputs to the character to move
        yield return new WaitForSeconds(diceRollAnimTime);

        //display the new tile number for each character
        GameObject[] characters = new GameObject[3];
        if (currentPlayer == 1)
            characters = ObjectHandler.Instance.player1Characters;
        else
            characters = ObjectHandler.Instance.player2Characters;
        string msg = "Dice roll : "+diceSum+"\n\n";
        foreach (GameObject x in characters)
            msg += x.GetComponent<Character>().GetName() + " -> " + (x.GetComponent<Character>().GetCurrentTile()+diceSum)+"\n";   //TODO: replace target tile calculation with better method
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(msg);

        //passing control to the characterSelection script to let player select a character
        //for movement, the control is passed back to the CharacterUpdateTile method
        CharacterSelection.Instance.GetCharacter(currentPlayer);
        while(!CharacterSelection.Instance.selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        CharacterUpdateTile(CharacterSelection.Instance.selected);
    }

    //waits for some time before ending the turn
    IEnumerator EndTurnRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (currentPlayer == 1)
            currentPlayer = 2;
        else
            currentPlayer = 1;

        //Update the current turn text indicator
        UIManager.Instance.UpdateCurrentTurnText(currentPlayer, vsAI);

        //if playing against AI
        if (vsAI && currentPlayer == 2)
        {
            UIManager.Instance.DisableDice(true);
            AI.GetComponent<AI_attempt>().Comp_turn();
        }
        else
        {
            UIManager.Instance.DisableDice(false);
            canActivateEventCard = true;
        }
    }

    //------------------- IENUMERATOR -----------------------END
}
