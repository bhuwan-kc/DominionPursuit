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
    private int numPeeps1 = 0;       //number of team 1 people on this tile.
    private int numPeeps2 = 0;      //number of team 2 people on this tile.
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

        if (team == 1) numPeeps1++;
        else numPeeps2++;

        if(activateTileEffect)
            TileEffect(team, id);
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
        if (team == 1) numPeeps1--;
        else numPeeps2--;
        if (numPeeps1 + numPeeps2 == 0)
        {
            occupied = false;
            faction = 0;
        }
        //if person leaving is last of team present and there is still someone on the tile, mark change in ownership.
        else if (team == 1 && numPeeps2 > 0 && numPeeps1 == 0) faction = 2;
        else if (team == 2 && numPeeps1 > 0 && numPeeps2 == 0) faction = 1;
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
                    int steps = Random.Range(-1, -6);
                    ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Moving backwards " + Mathf.Abs(steps) + " tiles.");
                    currentCharacter.GetComponent<Character>().UpdateTile(steps, false, true);
                    callsEndTurn = true;
                }break;

            case -2:    //tile that damages characters that land on it. Deals damage.
                {
                    currentCharacter.GetComponent<Character>().Damage(GameManager.Instance.GetTileDamage());
                    ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(currentCharacter.GetComponent<Character>().GetName() +
                        " suffered "+GameManager.Instance.GetTileDamage()+" damage.");
                }break;

            case 2:     //tile that heals characters that land on it. Heals hp.
                {
                    currentCharacter.GetComponent<Character>().Heal(GameManager.Instance.GetTileHeal());
                    ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(currentCharacter.GetComponent<Character>().GetName() +
                        " healed "+GameManager.Instance.GetTileHeal()+" hp.");
                }
                break;

            case 3:     //tile that moves a character forwards if they land on it. Do NOT proc events on the tile you land on.
                {
                    //regenerating random steps 
                    int steps = Random.Range(1, 6);
                    ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Moving forward " + steps + " tiles.");
                    currentCharacter.GetComponent<Character>().UpdateTile(steps, false, true);
                    callsEndTurn = true;
                }
                break;

            case 4:     //tile that gives a player an event card.
                {
                    StartCoroutine(EventCardCollectionRoutine(team));
                    callsEndTurn = true;
                }break;

            case 5:     //portal tile
                {
                    StartCoroutine(PortalTransitionRoutine(currentCharacter));
                    ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(currentCharacter.GetComponent<Character>().GetName() + 
                        " teleporting to tile 45!");
                    callsEndTurn = true;
                }break;

            default:
                //Debug.Log("No tile effect on this tile");
                break;
        }

        //tile effects -3, 3, 4, 5 call their own endturn 
        if (!callsEndTurn)
            GameManager.Instance.EndTurn(0.5f);
    }     

    IEnumerator PortalTransitionRoutine(GameObject character)
    {
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.portal);
        yield return new WaitForSeconds(0.75f);
        character.SetActive(false);
        character.transform.position = ObjectHandler.Instance.tiles[45].transform.position;
        character.GetComponent<Character>().SetCurrentTile(45);
        yield return new WaitForSeconds(1.0f);
        character.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        character.GetComponent<Character>().StackCharacterOnTile();
        GameManager.Instance.EndTurn(1.0f);
    }

    IEnumerator EventCardCollectionRoutine(int team)
    {
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Event Card Wizard has a gift for you...");
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
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("--------------\n"+ObjectHandler.Instance.eventCards.GetComponent<EventCards>().eventCardNames[eventCard]+ "\n--------------");
        yield return new WaitForSeconds(1f);
        ObjectHandler.Instance.eventCards.GetComponent<EventCards>().UpdateEventCardCount(team, eventCard, true);
        GameManager.Instance.EndTurn();
    }
}
