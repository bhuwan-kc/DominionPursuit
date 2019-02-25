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
    public Text playerCurrentTileText;    //the UI text for currentTile
    public Text computerCurrentTileText;
    public Text currentTurnText;
    public GameObject diceButton;

    //METHODS
    public void UpdateCurrentTileText(int tile)
    {
        int playerIndex = GameManager.Instance.currentPlayer;

        if (playerIndex == 1)
            playerCurrentTileText.text = tile + "";
        else
            computerCurrentTileText.text = tile + "";
    }

    public void UpdateCurrentTurnText(int playerIndex)
    {
        if (playerIndex == 1)
            currentTurnText.text = "Player 1";
        else
            currentTurnText.text = "Computer";
    }

    public void DisableDice(bool disable)
    {
        diceButton.GetComponent<Button>().interactable = !disable;
    }
}
