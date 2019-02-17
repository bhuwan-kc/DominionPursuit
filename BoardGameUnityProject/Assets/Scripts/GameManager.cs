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
    private bool diceIsRolling = false;
    public bool canSetCharacters = true;
    [SerializeField]
    private float diceRollAnimTime = 1.0f;

    //GAMEOBJECTS
    public GameObject Dice;
    public GameObject Dice2;
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
        if(!diceIsRolling)
        {
            ChangeDiceRollStatus(true);
            int diceOutput = Dice.GetComponent<Dice>().RollDice();
            int diceOutput2 = Dice2.GetComponent<Dice>().RollDice();
            StartCoroutine(WaitForDiceRollAnim(diceOutput, diceOutput2));
        }
    }

    public void ChangeDiceRollStatus(bool roll)
    {
        diceIsRolling = roll;
        if (roll == true)
        {
            Dice.GetComponent<Dice>().DiceRollAnimation();
            Dice2.GetComponent<Dice>().DiceRollAnimation();
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

    IEnumerator WaitForDiceRollAnim(int diceOutput, int diceOutput2)
    {
        Dice.GetComponent<Dice>().SetDiceFace(diceOutput);
        Dice2.GetComponent<Dice>().SetDiceFace(diceOutput2);
        yield return new WaitForSeconds(diceRollAnimTime);
        player1character1.GetComponent<Character>().updateTile(diceOutput+diceOutput2);
    }
}
