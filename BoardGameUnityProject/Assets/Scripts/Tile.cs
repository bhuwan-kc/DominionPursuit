using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Used to weight each tile for the AI based on how positive it is.
    //-5 is awful, 0 is neutral, 5 is great.
    [SerializeField]
    private int tileWeight = 0;
    private bool occupied = false;  //by default, a tile has no one on it.
    private int numPeeps = 0;       //by default, a tile has no one on it.
    private int faction = 0;        //0: no one. 1: team 1. 2: team 2. 3: both teams present.
    
    public int GetTileWeight()
    {

        return tileWeight;
    }

    public void ArriveOnTile(int team, int id, bool activateTileEffect)
    {
        if (!occupied)
        {
            occupied = true;
            faction = team;
        }
        else if (faction != team)
        {
            faction = 3;
        }

        numPeeps++;

        if(activateTileEffect)
            TileEffect(team, id); //since tileWeight is stored per object, don't need to pass anything to this function?
    }

    public bool CheckEmpty()
    {
        return occupied;
    }

    public int CheckFaction()
    {
        return faction;
    }

    public void LeaveTile(int team, int id)
    {
        numPeeps--;
        if (numPeeps == 0)
        {
            occupied = false;
            faction = 0;
        }
        //BUG: currently no way to change faction from 3 to the correct team #.
        //will need to figure out a fix, leaving as-is for now (4/6/19). Niche bug that only affects AI competence.
    }

    //tile effects. Weight is used as an ID for what happens.
    public void TileEffect(int team, int id)
    {
        //find character that landed on the tile.
            
        GameObject currentCharacter = null;

        if (team == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetID() == id)
                    currentCharacter = ObjectHandler.Instance.player1Characters[i];
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetID() == id)
                    currentCharacter = ObjectHandler.Instance.player2Characters[i];
            }
        }
        if (currentCharacter == null)
        {
            Debug.Log("ERROR: Could not ID character in TileEffect function. Abandoning...");
            return;
        }

        //--------- tile effects below --------------------
        //in increasing order.

        switch(tileWeight)
        {
            case -5:    //tile that traps a character until a turn is "sacrificed" on them with a dice roll of 7+
                {
                    //code
                }break;

            case -3:    //tile that moves a character backwards if they land on it. Do NOT proc events on the tile you land on. 
                {
                    //regenerating random steps 
                    int steps = Random.Range(-2, -8);
                    Debug.Log("Moving back " + steps + " tiles!");
                    currentCharacter.GetComponent<Character>().UpdateTile(steps, false);
                }break;

            case -2:    //tile that dammages characters that land on it.
                {
                    currentCharacter.GetComponent<Character>().Damage(3);
                }break;

            case 2:     //tile that heals characters that land on it.
                {
                    currentCharacter.GetComponent<Character>().Heal(5);
                }break;

            case 3:     //tile that moves a character forwards if they land on it. Do NOT proc events on the tile you land on.
                {
                    //regenerating random steps 
                    int steps = Random.Range(2, 8);
                    Debug.Log("Moving forward " + steps + " tiles!");
                    currentCharacter.GetComponent<Character>().UpdateTile(steps, false);
                }
                break;

            case 4:     //tile that gives a player an event card.
                {
                    //code
                }break;

            case 5:     //portal tile
                {
                    //code
                }break;

            default:
                Debug.Log("No tile effect on this tile");
                break;
        }

        //if no effects related to character movement, then end the turn
        //otherwise, the turn will end from the character script after the movement
        if (tileWeight != -3 && tileWeight != 3)
            GameManager.Instance.EndTurn();
    }
}
