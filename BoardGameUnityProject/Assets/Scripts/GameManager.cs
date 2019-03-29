using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that co-ordinates communication between all the game objects in the scene and handles gameplay 
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

    //************ VARIABLES *******************
    private int totalTiles = 78;
    public bool canSetCharacters = true;
    [SerializeField]
    private float diceRollAnimTime = 1.0f;
    public int currentPlayer = 1;                      //1 for player, 2 for computer
    private int diceSum = 0;
    private int presetdiceSum = 1;

	
    //*********** METHODS **********************

    //returns the transform property of the index tile
    //index range: 0 - total tiles 
    public Transform GetTilePosition(int index)
    {
        if(index >= 0 && index < totalTiles)
        {
			//get the particular tile object from the ObjectHandler and return its transform 
            return ObjectHandler.Instance.tiles[index].transform;
        }

        //if index out of range 
        Debug.Log("Tile index "+index+" out of range - GameManager.cs - GetTilePosition()");
        return ObjectHandler.Instance.tiles[0].transform;
    }

    //To generate random dice number 1-6 and start dice roll animation 
	//This method is called when player clicks on the dice 
    public void RollDice()
    {
		//to allow player to click only once per turn 
        UIManager.Instance.DisableDice(true);
		
		//to start the dice rolling animation 
        StartDiceRollAnimation();       
        
        //to get the random dice number 1-6
        int diceOutput = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();  
        int diceOutput2 = ObjectHandler.Instance.Dice2.GetComponent<Dice>().RollDice();

        //**************************************
        //******** FOR TESTING ONLY ************
        //**************************************
        //to preset the dice output 
		if (presetdiceSum != 1)
        {
            diceOutput = presetdiceSum - 1;
            diceOutput2 = 1;
        }
		
        //to wait for dice roll and show a dice face
        StartCoroutine(WaitForDiceRollAnim(diceOutput, diceOutput2));
    }

    //To start the dice roll animation 
    public void StartDiceRollAnimation()
    {
        ObjectHandler.Instance.Dice.GetComponent<Dice>().DiceRollAnimation();
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().DiceRollAnimation();
    }

    //for FUTURE purpose
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

    //to set the dice face and go to character selection 
    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
		//set the dice face to the obtained output 
        ObjectHandler.Instance.Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);
        //store the total sum
		diceSum = diceOutput + diceOutput2;

        //wait for some seconds and send the sum of the outputs to the character to move
        yield return new WaitForSeconds(diceRollAnimTime); //TODO - look into this wait time again

		//control flow for character selection 
        CharacterSelection.Instance.GetCharacter();
    }
}
