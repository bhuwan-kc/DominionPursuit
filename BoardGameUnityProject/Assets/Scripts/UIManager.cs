using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //Handles
    public Text[] player1CurrentTileText = new Text[3];    //the UI text for currentTile
    public Text[] player2CurrentTileText = new Text[3];
    public Text currentTurnText;

    public GameObject diceButton;
    public GameObject[] selectors1 = new GameObject[3];
    public GameObject[] selectors2 = new GameObject[3];

    //METHODS
    public void UpdateCurrentTileText(int tile, int characterNumber)
    {
        int playerIndex = GameManager.Instance.currentPlayer;

        if (playerIndex == 1)
            player1CurrentTileText[characterNumber].text = tile + "";
        else if(playerIndex == 2)
            player2CurrentTileText[characterNumber].text = tile + "";
    }

    public void UpdateCurrentTurnText(int playerIndex)
    {
        if (playerIndex == 1)
            currentTurnText.text = "Player 1";
        else
            currentTurnText.text = "Player 2";
    }

    public void DisableDice(bool disable)
    {
        diceButton.GetComponent<Button>().interactable = !disable;
    }
}
