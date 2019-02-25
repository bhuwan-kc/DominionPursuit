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
    public GameObject[] tiles = new GameObject[78];     //an array of all the tiles on the board
    public bool canSetCharacters = true;
    [SerializeField]
    private float diceRollAnimTime = 1.0f;
    public int currentPlayer = 1;                      //1 for player, 2 for computer

    //GAMEOBJECTS
    public GameObject Dice;
    public GameObject Dice2;
    public GameObject playerCharacter1;
    public GameObject computerCharacter1;

    //METHODS

    //returns the transform property of the index tile 
    public Transform GetTilePosition(int index)
    {
        if(index >= 0 && index < totalTiles)
        {
            return tiles[index].transform;
        }

        //if index out of range 
        Debug.Log("Tile index "+index+" out of range - GetTilePosition - GameManager");
        return tiles[0].transform;
    }

    //to roll the dice when player clicks on the dice
    public void RollDice()
    {
        if(currentPlayer == 1)
            UIManager.Instance.DisableDice(true);

        StartDiceRollAnimation();       
        
        //to get the random dice output
        int diceOutput = Dice.GetComponent<Dice>().RollDice();  
        int diceOutput2 = Dice2.GetComponent<Dice>().RollDice();

        //to wait for dice roll and show a dice face
        StartCoroutine(WaitForDiceRollAnim(diceOutput, diceOutput2));
    }

    //to change the status that tracks if the dice is rollable 
    public void StartDiceRollAnimation()
    {
        //if dice roll is set to true, play the dice roll animation
        Dice.GetComponent<Dice>().DiceRollAnimation();
        Dice2.GetComponent<Dice>().DiceRollAnimation();
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
        {
            currentPlayer = 2;
            UIManager.Instance.DisableDice(true);
            StartCoroutine(WaitForComputerToRollDice());
        }
        else
        {
            currentPlayer = 1;
            UIManager.Instance.DisableDice(false);
        }
        UIManager.Instance.UpdateCurrentTurnText(currentPlayer);
    }

    //to set the dice face and send signal to the character script
    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
        Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);
        //wait for some seconds and send the sum of the outputs to the character to move
        yield return new WaitForSeconds(diceRollAnimTime);

        if(currentPlayer == 1)
            playerCharacter1.GetComponent<Character>().updateTile(diceOutput + diceOutput2);
        else if(currentPlayer == 2)
            computerCharacter1.GetComponent<Character>().updateTile(diceOutput + diceOutput2);
    }

    IEnumerator WaitForComputerToRollDice()
    {
        yield return new WaitForSeconds(0.75f);
        RollDice();
    }
}
