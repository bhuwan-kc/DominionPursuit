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
    private bool diceIsRolling = false;         
    public bool canSetCharacters = true;
    [SerializeField]
    private float diceRollAnimTime = 1.0f;              

    //GAMEOBJECTS
    public GameObject Dice;
    public GameObject Dice2;
    public GameObject player1character1;

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
        if(!diceIsRolling)
        {
            ChangeDiceRollStatus(true);         //to avoid unnecessary dice rolls
            //to get the random dice output
            int diceOutput = Dice.GetComponent<Dice>().RollDice();  
            int diceOutput2 = Dice2.GetComponent<Dice>().RollDice();
            //call for dice roll animation
            StartCoroutine(WaitForDiceRollAnim(diceOutput, diceOutput2));
        }
    }

    //to change the status that tracks if the dice is rollable 
    public void ChangeDiceRollStatus(bool roll)
    {
        diceIsRolling = roll;
        if (roll == true)
        {
            //if dice roll is set to true, play the dice roll animation
            Dice.GetComponent<Dice>().DiceRollAnimation();
            Dice2.GetComponent<Dice>().DiceRollAnimation();
        }
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

    //to set the dice face and send signal to the character script
    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
        Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);
        //wait for some seconds and send the sum of the outputs to the character to move
        yield return new WaitForSeconds(diceRollAnimTime);
        player1character1.GetComponent<Character>().updateTile(diceOutput+diceOutput2);
    }
}
