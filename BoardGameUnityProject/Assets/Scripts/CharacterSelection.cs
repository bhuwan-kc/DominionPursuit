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

    public int selected = 0;               //the character number that was selected
    public bool selectedValueSet = false;  //to detect when the player select a character




    //***************************************************************
    //************************* METHODS *****************************
    //***************************************************************

    //to display the character highlighter in the game
    private void showHighlighter(bool show, int player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (player == 1)
            {
                //ignore characters that have reached the end tile
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile() == GameManager.Instance.finalTileNumber + 1)
                    continue;

                //highlighter on the game board
                ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
                //highlighter on the profile tab
                UIManager.Instance.selectors1[i].SetActive(show);
            }
            else if (player == 2)
            {
                //ignore characters that have reached the end tile
                if (ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetCurrentTile() == GameManager.Instance.finalTileNumber + 1)
                    continue;

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
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.buttonClick);
    }

    public void GetCharacter(int player)
    {
        //turn on the highlighter
        showHighlighter(true, player);
        selectedValueSet = false;
        StartCoroutine(CharacterSelectionRoutine(player));
    }

    //------------------- IENUMERATOR -----------------------START

    //to wait for player selection and then return selected character
    IEnumerator CharacterSelectionRoutine(int player)
    {
        while (!selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        //hide highlighters
        showHighlighter(false, player);
    }

    //------------------- IENUMERATOR -----------------------END
}
