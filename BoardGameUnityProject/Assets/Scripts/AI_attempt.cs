using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_attempt : MonoBehaviour
{
    //create static instance of ai_attempt
    private static AI_attempt _ai;
    public static AI_attempt ai
    {
        get
        {
            if (_ai != null)
                return _ai;
            else
            {
                Debug.Log("ai object is null");
                return null;
            }
        }
    }

    private void Awake()
    {
        _ai = this;
    }

    //variable dec.
    private int lastMove = -1; //last move tracks what character was moved last, to help prevent only moving the same char.

    public int Comp_turn()
    {
        //This function returns an integer, 0-2, which represents which character to move.
        //This return value will probabbly be unused.

        //variable dec.
        int[] characterLocations = new int[3]; //used only to check where characters will go. does NOT store current location.
        int[] tileWeight = new int[3]; //used to save weights of updated characterLocation tiles.
        int diceRoll1, diceRoll2; //store dice rolls for rolling animation.
        int move = 0;
        int best = -6; //default value that will be overwritten by all other options.
        int charToMove = 0; //character to move. default is character 1.

        //grab character current location.
        for (int i = 0; i < 3; i++) {
            characterLocations[i] = ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().getCurrentTile();
            //Debug.Log("Start: characterLocation[" + i + "] is " + characterLocations[i]);
            if (characterLocations[i] >= 78) characterLocations[i] = -3; //-3 means never pick it.
            //-1 is off the board, but isn't really a space. So the AI will look at the space it will actually land on.
            else if (characterLocations[i] == -1) characterLocations[i] = 0;
        }

        //generate diceroll for character movement.
        diceRoll1 = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();
        diceRoll2 = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();
        move = diceRoll1 + diceRoll2;

        //testing AI moving more than just character 0
        //move = 3;  

        //see what the weight of where each character moves to would be.
        for (int i = 0; i < 3; i++)
        {
            //if character is at start and all move are neutral, get character out of start safely.
            if (characterLocations[i] == 0 && ObjectHandler.Instance.tiles[move].GetComponent<Tile>().getTileWeight() == 0)
                tileWeight[i] = 1;
            else if (characterLocations[i] != -3)
            {
                characterLocations[i] += move;
                //Debug.Log("Final: characterLocation[" + i + "] is " + characterLocations[i]);
                tileWeight[i] = ObjectHandler.Instance.tiles[characterLocations[i]].GetComponent<Tile>().getTileWeight();
            }
            else
            {
                tileWeight[i] = -6; //character cannot be chosen to move.
            }
        }

        //output move distance and weights of tiles characters would land on.
        Debug.Log("AI rolled a total of " + move);
        Debug.Log("tileWeight[0] is " + tileWeight[0] + " TileWeight[1] is " + tileWeight[1] + " Tileweight[2] is " + tileWeight[2]);

        //find best move possible.
        for (int i = 0; i < 3; i++)
        {
            if (best < tileWeight[i])
            {
                best = tileWeight[i];
                charToMove = i;
            }
        }

        //code to prevent always moving the same character.
        if (charToMove == lastMove)
        {
            int random = Random.Range(0, 2);
        }

        lastMove = charToMove;

        //make the move
        ObjectHandler.Instance.player2Characters[charToMove].GetComponent<Character>().UpdateTile(move, charToMove);

        return charToMove; //return what character is moved. Unused atm.
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //???
    }

    // Update is called once per frame
    void Update()
    {
        //???
    }

    

}

