using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class to handle character selection when the dice is roll 
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

	//*************************** VARIABLES *********************************
	
	//to track character number selected 
    private int selected = 0;
	//to detect when the character is selected 
    private bool selectedValueSet = false;

	
	//*************************** METHODS ***********************************
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	//to highlight all the possible character [3]
	//show - true: enable highlighters
	//		 false: disable highlighters 
	//Two types of character highlighters
	//1. Profile highlighter - On the left panel [These are also buttons that allow player to select the character]
	//2. Board highlighter - on the game board
    private void showHighlighter(bool show)
    {
        for (int i = 0; i < 3; i++)
        {
			//if current player is Player 1 
            if (GameManager.Instance.currentPlayer == 1)
            {	
				//to access the game board highlighter of the characters and set active status  
				//each character has its own board highlighter and the handler, so it can be directly accessed as follows 
                ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
               
				//get the handler for the profile highlighters from UIManager class and set active status
				UIManager.Instance.selectors1[i].SetActive(show);
            }
			//if current player is Player 2
            else if (GameManager.Instance.currentPlayer == 2)
            {
                ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
                UIManager.Instance.selectors2[i].SetActive(show);
            }
        }
    }

	//to set the character number that was selected by the player 
	//this method is called when player selected a player 
	//number - range: 0-3
    public void CharacterSelected(int number)
    {
        selected = number;
		//to indicate that player has selected a character 
        selectedValueSet = true;
    }

	//To show the character highlighters and wait for character selections 
    public void GetCharacter()
    {
        showHighlighter(true);
        StartCoroutine(CharacterSelectionRoutine());
    }

	//To wait for player to select a character that they want to move 
    IEnumerator CharacterSelectionRoutine()
    {
        //wait until the selectedValueSet is false 
        while(!selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
		//reset the selectedValueSet to false 
        selectedValueSet = false;
		//turn off the character highlighters 
        showHighlighter(false);
		
		//to pass the control flow back to GameManager object for character updates 
		//selected - the character number that was selected by the player, range: 0-3
        GameManager.Instance.CharacterUpdateTile(selected);
    }
}
