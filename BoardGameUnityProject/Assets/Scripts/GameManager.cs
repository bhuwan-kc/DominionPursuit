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
        UIManager.Instance.DisableDice(false);
    }

    //to set the dice face and send signal to the character script
    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
        ObjectHandler.Instance.Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);

        //wait for some seconds and send the sum of the outputs to the character to move
        yield return new WaitForSeconds(diceRollAnimTime);

        if(currentPlayer == 1)
            ObjectHandler.Instance.playerCharacter1.GetComponent<Character>().updateTile(diceOutput + diceOutput2);
        else if(currentPlayer == 2)
            ObjectHandler.Instance.playerCharacter2.GetComponent<Character>().updateTile(diceOutput + diceOutput2);
    }
}
