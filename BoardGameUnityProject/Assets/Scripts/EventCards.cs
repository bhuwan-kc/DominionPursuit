using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCards : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            player1EventCardCounts[i] = 1;
            player2EventCardCounts[i] = 1;
        }
        //give two random event cards to both the players
        UpdateEventCardCount(1, Random.Range(0, 4), false);
        UpdateEventCardCount(2, Random.Range(0, 4), false);

        UpdateSlots(GameManager.Instance.currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int[] player1EventCardCounts = new int[4];
    private int[] player2EventCardCounts = new int[4];
    public string[] eventCardNames = new string[4];

    //grabbing player 2 event cards for the AI.
    public int getPlayer2EventCardCounts(int i)
    {
        return player2EventCardCounts[i];
    }

    public void ActivateEventCard(int index)
    {
        //prevent activating event card when its not allowed (only allowed before the dice roll on the turn)
        if (!GameManager.Instance.canActivateEventCard)
            return;

        if ((GameManager.Instance.currentPlayer==1 && player1EventCardCounts[index] == 0) || (GameManager.Instance.currentPlayer == 2 && player2EventCardCounts[index] == 0))
        {
            ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(eventCardNames[index] + " has not been collected!");
            return;
        }

        //disable dice 
        UIManager.Instance.DisableDice(true);
        //limit use of event card once per turn
        GameManager.Instance.canActivateEventCard = false;
        //decrease the event card count for the type
        UpdateEventCardCount(GameManager.Instance.currentPlayer, index, false);
        //play the sound
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.powerUp);

        switch(index)
        {
            case 0: StartCoroutine(ActivateEventCard1()); break;
            case 1: StartCoroutine(ActivateEventCard2()); break;
            case 2: ActivateEventCard3(); break;
            case 3: StartCoroutine(ActivateEventCard4()); break;
            default:
                Debug.Log("Invalid event card selection! " + Time.time);
                UIManager.Instance.DisableDice(false);
                break;
        }
    }

    //to increase or decrease the event cards count
    public void UpdateEventCardCount(int player, int type, bool increase)
    {
        if (player == 1)
        {
            if (increase)
                player1EventCardCounts[type]++;
            else if(player1EventCardCounts[type]>0)
                player1EventCardCounts[type]--;
            UIManager.Instance.player1EventCardCounts[type].text = player1EventCardCounts[type] + "";
        }
        else
        {
            if (increase)
                player2EventCardCounts[type]++;
            else if (player2EventCardCounts[type] > 0)
                player2EventCardCounts[type]--;
            UIManager.Instance.player2EventCardCounts[type].text = player2EventCardCounts[type] + "";
        }
    }

    //Update event card slots according to the current player
    public void UpdateSlots(int player)
    {
        for(int i=0; i<4; i++)
        {
            if (player == 1)
            {
                UIManager.Instance.player1EventCardCounts[i].text = player1EventCardCounts[i] + "";
            }
            else
            {
                UIManager.Instance.player2EventCardCounts[i].text = player2EventCardCounts[i] + "";
            }
        }
    }

    //Event card - MedKit
    IEnumerator ActivateEventCard1()
    {
        //let player select the character
        CharacterSelection.Instance.GetCharacter(GameManager.Instance.currentPlayer);
        while (!CharacterSelection.Instance.selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        int characterNum = CharacterSelection.Instance.selected;

        if (GameManager.Instance.currentPlayer == 1)
        {
            ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().Heal(GameManager.Instance.GetEventHeal());
            ObjectHandler.Instance.messageBoxObj.GetComponent<MessageBox>().DisplayMessageContinued(ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().GetName() +
                " heals "+GameManager.Instance.GetEventHeal()+" HP");
        }
        else
        {
            ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().Heal(GameManager.Instance.GetEventHeal());
            ObjectHandler.Instance.messageBoxObj.GetComponent<MessageBox>().DisplayMessageContinued(ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().GetName() +
                " heals " + GameManager.Instance.GetEventHeal() + " HP");
        }

        yield return new WaitForSeconds(0.75f);
        UIManager.Instance.DisableDice(false);
    }

    //Event card - Sabotage
    IEnumerator ActivateEventCard2()
    {
        //get the opponent
        int opponent;
        if (GameManager.Instance.currentPlayer == 1)
            opponent = 2;
        else
            opponent = 1;

        //let player select the character
        CharacterSelection.Instance.GetCharacter(opponent);
        while (!CharacterSelection.Instance.selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        int characterNum = CharacterSelection.Instance.selected;

        if (opponent == 1)
        {
            ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().Damage(GameManager.Instance.GetEventDamage());
            ObjectHandler.Instance.messageBoxObj.GetComponent<MessageBox>().DisplayMessageContinued(ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().GetName() +
                " suffers "+ GameManager.Instance.GetEventDamage()+" damage");
        }
        else
        {
            ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().Damage(GameManager.Instance.GetEventDamage());
            ObjectHandler.Instance.messageBoxObj.GetComponent<MessageBox>().DisplayMessageContinued(ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().GetName() +
                " suffers "+ GameManager.Instance.GetEventDamage()+" damage");
        }

        yield return new WaitForSeconds(0.75f);
        UIManager.Instance.DisableDice(false);
    }

    //Event card - Shortcut
    void ActivateEventCard3()
    {
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Taking the shortcut...");
        GameManager.Instance.bonusSteps = GameManager.Instance.GetEventForwardMoves();
        UIManager.Instance.DisableDice(false);
    }

    //Event card - Detour
    IEnumerator ActivateEventCard4()
    {
        //get the opponent
        int opponent;
        if (GameManager.Instance.currentPlayer == 1)
            opponent = 2;
        else
            opponent = 1;

        //let player select the character
        CharacterSelection.Instance.GetCharacter(opponent);
        while (!CharacterSelection.Instance.selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        int characterNum = CharacterSelection.Instance.selected;

        if (opponent == 1)
        {
            ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().UpdateTile(-1*GameManager.Instance.GetEventBackwardMoves(), false, false);
            ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().GetName() +
                        " Is moving backwards "+GameManager.Instance.GetEventBackwardMoves()+" tiles.");
        }
        else
        {
            ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().UpdateTile(-1*GameManager.Instance.GetEventBackwardMoves(), false, false);
            ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().GetName() +
                        " Is moving backwards " + GameManager.Instance.GetEventBackwardMoves() + " tiles.");
        }

        yield return new WaitForSeconds(0.75f);
        UIManager.Instance.DisableDice(false);
    }
}
