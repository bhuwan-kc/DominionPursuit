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

    //VARIABLES
    private int totalTiles = 78;
    public bool canSetCharacters = true;
    [SerializeField]
    private float diceRollAnimTime = 1.0f;
    public int currentPlayer = 1;                      //1 for player, 2 for computer
    public bool vsAI = true;            //if game is vs AI or not.
    private int diceSum = 0;
    private int presetdiceSum = 1;

    //METHODS

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
        StartDiceRollAnimation();       
        
        //to get the random dice output
        int diceOutput = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();  
        int diceOutput2 = ObjectHandler.Instance.Dice2.GetComponent<Dice>().RollDice();

        //checking what dice outputs are, as we don't seem to be rolling any 6's.
        //Debug.Log("diceOutput = " + diceOutput + "diceOutput2 = " + diceOutput2);

        //**************************************
        //******** FOR TESTING ONLY ************
        //**************************************
        if (presetdiceSum != 1)
        {
            diceOutput = presetdiceSum - 1;
            diceOutput2 = 1;
        }
        //to wait for dice roll and show a dice face
        StartCoroutine(WaitForDiceRollAnim(diceOutput, diceOutput2));
    }

    //to change the status that tracks if the dice is rollable 
    public void StartDiceRollAnimation()
    {
        //if dice roll is set to true, play the dice roll animation
        ObjectHandler.Instance.Dice.GetComponent<Dice>().DiceRollAnimation();
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().DiceRollAnimation();
    }

    //for future purpose
    public void SetCharacters()
    {
        if(canSetCharacters)
        {
            //set characters for players
        }
        canSetCharacters = false;
    }

    //to end a turn
    public void EndTurn()
    {
        if (currentPlayer == 1)
            currentPlayer = 2;
        else
            currentPlayer = 1;
        
        UIManager.Instance.UpdateCurrentTurnText(currentPlayer);

        if (vsAI && currentPlayer == 2)
        {
            Debug.Log("Ai turn");
            UIManager.Instance.DisableDice(true);
            AI_attempt.ai.Comp_turn();
            UIManager.Instance.UpdateCurrentTurnText(currentPlayer);
        }

        UIManager.Instance.DisableDice(false);
    }

    //update the tiles
    public void CharacterUpdateTile(int number)
    {
        if (currentPlayer == 1)
            ObjectHandler.Instance.player1Characters[number].GetComponent<Character>().UpdateTile(diceSum, number);
        else if (currentPlayer == 2)
            ObjectHandler.Instance.player2Characters[number].GetComponent<Character>().UpdateTile(diceSum, number);
    }

    public void SetDiceSum(int sum)
    {
        presetdiceSum = sum;
    }

    //to set the dice face and send signal to the character script
    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
        ObjectHandler.Instance.Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);
        diceSum = diceOutput + diceOutput2;

        //wait for some seconds and send the sum of the outputs to the character to move
        yield return new WaitForSeconds(diceRollAnimTime);

        CharacterSelection.Instance.GetCharacter();
    }
}
