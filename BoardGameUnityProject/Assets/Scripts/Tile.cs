using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Used to weight each tile for the AI based on how positive it is.
    //-5 is awful, 0 is neutral, 5 is great.
    [SerializeField] private int tileWeight;
    private bool occupied = false;  //by default, a tile has no one on it.
    private int numPeeps = 0;       //by default, a tile has no one on it.
    private int faction = 0;        //0: no one. 1: team 1. 2: team 2. 3: both teams present.
    
    public int GetTileWeight()
    {

        return tileWeight;
    }

    public void ArriveOnTile(int team)
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

        TileEffect(); //since tileWeight is stored per object, don't need to pass anything to this function?
    }

    public bool CheckEmpty()
    {
        if (occupied) return true;
        else return false;
    }

    public int CheckFaction()
    {
        return faction;
    }

    public void LeaveTile()
    {
        numPeeps--;
        if (numPeeps == 0)
        {
            occupied = false;
        }
    }

    public int TileEffect()
    {
        //tile effect will go here. Use weight as an ID for event?
        return 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        //???
    }

    // Update is called once per frame
    void Update()
    {
        //????
    }
}
