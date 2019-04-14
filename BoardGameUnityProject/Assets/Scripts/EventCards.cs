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
            player1EventCardCounts[i] = 0;
            player2EventCardCounts[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int[] player1EventCardCounts = new int[4];
    private int[] player2EventCardCounts = new int[4];
    public string[] eventCardNames = new string[4];

    public void ActivateEventCard(int index)
    {
        //prevent activating event card when its not allowed (only allowed before the dice roll on the turn)
        if (!GameManager.Instance.canActivateEventCard)
            return;

        if (player1EventCardCounts[index] == 0)
        {
            ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(eventCardNames[index] + " has not been collected yet!");
            return;
        }

        //disable dice 
        UIManager.Instance.DisableDice(true);
        //limit use of event card once per turn
        GameManager.Instance.canActivateEventCard = false;
        //decrease the event card count for the type
        UpdateEventCardCount(GameManager.Instance.currentPlayer, index, false);

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

        if(GameManager.Instance.currentPlayer == 1)
            ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().Heal(3);
        else
            ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().Heal(3);

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
            ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().Damage(4);
        else
            ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().Damage(4);

        yield return new WaitForSeconds(0.75f);
        UIManager.Instance.DisableDice(false);
    }

    //Event card - Shortcut
    void ActivateEventCard3()
    {
        ObjectHandler.Instance.GetMessageBox().DisplayMessage(new string[] {"Taking the shortcut!","5 extra steps will be added to your dice roll..."});
        GameManager.Instance.bonusSteps = 5;
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
            ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().UpdateTile(-5, false);
        else
            ObjectHandler.Instance.player2Characters[characterNum].GetComponent<Character>().UpdateTile(-5, false);

        yield return new WaitForSeconds(0.75f);
        UIManager.Instance.DisableDice(false);
    }
}
