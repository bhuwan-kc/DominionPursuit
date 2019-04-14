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

        bool callsEndTurn = false;  //to indicate if that tile effect has its own call to end turn method

        switch(tileWeight)
        {
            case -5:    //tile that traps a character until a turn is "sacrificed" on them with a dice roll of 7+
                {
                    //code
                }break;

            case -3:    //tile that moves a character backwards if they land on it. Do NOT proc events on the tile you land on. 
                {
                    //regenerating random steps 
                    int steps = Random.Range(-1, -7);
                    Debug.Log("Moving back " + steps + " tiles!");
                    currentCharacter.GetComponent<Character>().UpdateTile(steps, false);
                    callsEndTurn = true;
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
                    int steps = Random.Range(1, 7);
                    Debug.Log("Moving forward " + steps + " tiles!");
                    currentCharacter.GetComponent<Character>().UpdateTile(steps, false);
                    callsEndTurn = true;
                }
                break;

            case 4:     //tile that gives a player an event card.
                {
                    if (!GameManager.Instance.vsAI || GameManager.Instance.currentPlayer != 2)
                    {
                        StartCoroutine(EventCardCollectionRoutine(team));
                        callsEndTurn = true;
                    }
                }break;

            case 5:     //portal tile
                {
                    StartCoroutine(PortalTransitionRoutine(currentCharacter));
                    callsEndTurn = true;
                }break;

            default:
                Debug.Log("No tile effect on this tile");
                break;
        }

        //tile effects -3, 3, 4, 5 call their own endturn 
        if (!callsEndTurn)
            GameManager.Instance.EndTurn();
    }     

    IEnumerator PortalTransitionRoutine(GameObject character)
    {
        yield return new WaitForSeconds(0.75f);
        character.SetActive(false);
        character.transform.position = ObjectHandler.Instance.tiles[45].transform.position;
        character.GetComponent<Character>().SetCurrentTile(45);
        yield return new WaitForSeconds(1.0f);
        character.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        GameManager.Instance.EndTurn();
    }

    IEnumerator EventCardCollectionRoutine(int team)
    {
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Event Cards Wizard has a gift for you...");
        yield return new WaitForSeconds(1.75f);
        for(int i=0; i<30; i++)
        {
            ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(ObjectHandler.Instance.eventCards.GetComponent<EventCards>().eventCardNames[i%4]);
            if (i < 25)
                yield return new WaitForSeconds(0.05f);
            else
                yield return new WaitForSeconds(0.25f);
        }
        int eventCard = Random.Range(0, 4);
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(ObjectHandler.Instance.eventCards.GetComponent<EventCards>().eventCardNames[eventCard]);
        yield return new WaitForSeconds(1f);
        ObjectHandler.Instance.eventCards.GetComponent<EventCards>().UpdateEventCardCount(team, eventCard, true);
        GameManager.Instance.EndTurn();
    }
}
