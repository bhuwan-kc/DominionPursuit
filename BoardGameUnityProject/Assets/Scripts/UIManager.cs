using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A class to manage the game UI
public class UIManager : MonoBehaviour
{
    //setting up a static instance of the class 
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                Debug.Log("UIManager object is null");
                return null;
            }
        }
    }

    private void Awake()
    {
        _instance = this;
    }



    //***************************************************************
    //************************** HANDLES ****************************
    //***************************************************************

    public Text[] player1CurrentTileText = new Text[3];    //the UI text for currentTile
    public Text[] player2CurrentTileText = new Text[3];
    public Text currentTurnText;

    public GameObject diceButton;
    public GameObject[] selectors1 = new GameObject[3];
    public GameObject[] selectors2 = new GameObject[3];
    public GameObject[] player1HealthBars = new GameObject[3];
    public GameObject[] player2HealthBars = new GameObject[3];
    public Text[] player1HealthText = new Text[3];
    public Text[] player2HealthText = new Text[3];

    //***FOR TESTING ONLY***
    public GameObject TdiceSum;
    public Text TdiceSumText;




    //***************************************************************
    //************************** METHODS ****************************
    //***************************************************************

    //updates the current tile indicator for the given character
    public void UpdateCurrentTileText(int tile, int characterNumber)
    {
        int playerIndex = GameManager.Instance.currentPlayer;

        if (playerIndex == 1)
            player1CurrentTileText[characterNumber].text = tile + "";
        else if(playerIndex == 2)
            player2CurrentTileText[characterNumber].text = tile + "";
    }

    public void UpdateCurrentTurnText(int playerIndex, bool vsAI)
    {
        if (playerIndex == 1)
            currentTurnText.text = "Player 1";
        else if (vsAI)
            currentTurnText.text = "Computer";
        else
            currentTurnText.text = "Player 2";
    }

    public void UpdateHealthBar(string characterName, int health)
    {
        for(int i=0; i<3; i++)
        {
            if (characterName.Equals(ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetName()))
            {
                player1HealthBars[i].GetComponent<Slider>().value = health;
                player1HealthText[i].text = health + "";
                return;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (characterName.Equals(ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetName()))
            {
                player2HealthBars[i].GetComponent<Slider>().value = health;
                player2HealthText[i].text = health + "";
                return;
            }
        }
    }

    public void TSetDiceSum()
    {
        GameManager.Instance.SetDiceSum((int)TdiceSum.GetComponent<Slider>().value);
        TdiceSumText.text = TdiceSum.GetComponent<Slider>().value + "";
    }

    public void DisableDice(bool disable)
    {
        diceButton.GetComponent<Button>().interactable = !disable;
    }

}
