using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int selected = 0;
    private bool selectedValueSet = false;

    private void showHighlighter(bool show)
    {
        for (int i = 0; i < 3; i++)
        {
            if (GameManager.Instance.currentPlayer == 1)
                ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
            else if (GameManager.Instance.currentPlayer == 2)
                ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().Highlighter.SetActive(show);
        }
    }

    public void CharacterSelected(int number)
    {
        selected = number;
        selectedValueSet = true;
    }

    public void GetCharacter()
    {
        showHighlighter(true);
        StartCoroutine(CharacterSelectionRoutine());
    }

    IEnumerator CharacterSelectionRoutine()
    {
        //wait for player selection and then return selected character
        while(!selectedValueSet)
        {
            yield return new WaitForEndOfFrame();
        }
        selectedValueSet = false;
        showHighlighter(false);
        GameManager.Instance.characterUpdateTile(selected);
    }
}
