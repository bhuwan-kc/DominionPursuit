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

    public void ArriveOnTile(int team, int id)
    {
        if (!occupied) {
            occupied = true;
            faction = team;
        }
        else if (faction != team)
        {
            faction = 3;
        }

        numPeeps++;

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

    public void LeaveTile(int team)
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
    public int TileEffect(int team, int id)
    {
        //find character that landed on the tile.
        //TODO: make a better way to do this that is less time intensive.
        int charNum = -1; //used to store character location in array.
        if (team == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetID() == id) charNum = i;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetID() == id) charNum = i;
            }
        }
        if (charNum == -1)
        {
            Debug.Log("ERROR: Could not ID character in TileEffect function. Abandoning...");
            return 0;
        }


        //--------- tile effects below --------------------
        //in increasing order.

        //tile that traps a character until a turn is "sacrificed" on them with a dice roll of 7+
        if (tileWeight == -5)
        {
            //code
        }

        //tile that dammages characters that land on it.
        if (tileWeight == -2)
        {
            if (team == 1) ObjectHandler.Instance.player1Characters[charNum].GetComponent<Character>().Damage(3);
            else ObjectHandler.Instance.player2Characters[charNum].GetComponent<Character>().Damage(3);
        }

        if (tileWeight == 2)
        {
            if (team == 1) ObjectHandler.Instance.player1Characters[charNum].GetComponent<Character>().Heal(2);
            else ObjectHandler.Instance.player2Characters[charNum].GetComponent<Character>().Heal(2);
        }

        return 0;
    }
}
