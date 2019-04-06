using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that deals with choosing a character for movement
public class CharacterSelection : MonoBehaviour
{
    //setting up a static instance of the class 
    private static CharacterSelection _instance;

    public static CharacterSelection Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                Debug.Log("CharacterSelection object is null");
                return null;
            }
        }
    }

    private void Awake()
    {
        _instance = this;
    }


    //***************************************************************
    //************************* PROPERTIES **************************
    //***************************************************************

    private int selected = 0;               //the character number that was selected
    private bool selectedValueSet = false;  //to detect when the player select a character




    //***************************************************************
    //************************* METHODS *****************************
    //***************************************************************

    //to display the character highlighter in the game
    private void showHighlighter(bool show)
    {
        for (int i = 0; i < 3; i++)
        {
            if (GameManager.Instance.currentPlayer == 1)
            {
                //highlighter on the game board
                ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
                //highlighter on the profile tab
                UIManager.Instance.selectors1[i].SetActive(show);
            }
            else if (GameManager.Instance.currentPlayer == 2)
            {
                ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
                UIManager.Instance.selectors2[i].SetActive(show);
            }
        }
    }
    
    //this method is called when player clicks on a character
    //number is the character number that player selected
    public void CharacterSelected(int number)
    {
        selected = number;
        selectedValueSet = true;
    }

    public void GetCharacter()
    {
        //turn on the highlighter
        showHighlighter(true);
        StartCoroutine(CharacterSelectionRoutine());
    }

    //------------------- IENUMERATOR -----------------------START

    //to wait for player selection and then return selected character
    IEnumerator CharacterSelectionRoutine()
    {
        while(!selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        selectedValueSet = false;
        //hide highlighters
        showHighlighter(false);
        //return the selected character number to the GameManager
        GameManager.Instance.CharacterUpdateTile(selected);
    }

    //------------------- IENUMERATOR -----------------------END
}
