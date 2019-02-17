using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    public GameObject[] tiles = new GameObject[78];
    public bool canRollDice = true;
    public bool canSetCharacters = true;

    //GAMEOBJECTS
    public GameObject Dice;
    public GameObject player1character1;

    //METHODS
    public Transform GetTilePosition(int index)
    {
        if(index >= 0 && index < totalTiles)
        {
            return tiles[index].transform;
        }

        Debug.Log("Tile index "+index+" out of range - GetTilePosition - GameManager");
        return tiles[0].transform;
    }

    public void RollDice()
    {
        if(canRollDice)
        {
            canRollDice = false;
            int diceOutput = Dice.GetComponent<Dice>().RollDice();
            player1character1.GetComponent<Character>().updateTile(diceOutput);
        }
    }

    public void SetCharacters()
    {
        if(canSetCharacters)
        {
            //set characters for players
        }
        canSetCharacters = false;
    }
}
