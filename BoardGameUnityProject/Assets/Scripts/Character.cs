using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class to handle character behavior
public class Character : MonoBehaviour
{
    private int currentTile = -1;        //to track the current position of the character
    [SerializeField]
    private float  speed = 1.0f;     //the speed with which character moves across tiles
    [SerializeField]
    private string characterName;    //for future use
    private int health;

    private SpriteRenderer _sprite;
    public GameObject Highlighter;

    // Start is called before the first frame update
    void Start()
    {
        health = 10;
        currentTile = -1;
        _sprite = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (health == 0 && currentTile == 0)
        {
            health = 10;
            UIManager.Instance.UpdateHealthBar(characterName, health);
        }
    }

    //grab current character tile.
    public int getCurrentTile()
    {
        return currentTile;
    }

    //to move the character by the given steps 
    public void UpdateTile(int steps, int number)
    {
        if(currentTile == -1)
        {
            transform.position = ObjectHandler.Instance.tiles[0].transform.position;
            steps++;
        }
        StartCoroutine(TileTransitionRoutine(steps, number, -1));
    }

    public void Damage(int x)
    {
        health -= x;
        UIManager.Instance.UpdateHealthBar(characterName, health);

        if(health <= 0)
        {
            health = 0;
            StartCoroutine(TileTransitionRoutine(0, -1, 0));
            //move to start position
            //wait for some seconds
            //refill health
        }
    }

    public int GetHealth()
    {
        return health; 
    }

    public string GetName()
    {
        return characterName;
    }

    public int GetCharacterNumber()
    {
        for(int i=0; i<3; i++)
        {
            if (characterName.Equals(ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetName()))
                return i;
            if (characterName.Equals(ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetName()))
                return i;
        }
        Debug.Log("Error: Character number could not be determined in CharacterSelection.cs - GetCharacterNumber()");
        return 1;
    }

    //moves the character through the tiles
    //if targetTile != -1, steps must be 0 ---> Jump to targetTile directly
    //if targetTile == -1, steps > 0 ---> stepwise tile transition 
    IEnumerator TileTransitionRoutine(int steps, int characterNumber, int targetTile)
    {
        _sprite.sortingOrder = 10;   

        //move one tile at a time 
        for (int i = 1; i <= steps; i++)
        {
            //get the position of next tile as a destination 
            Vector3 targetPosition = GameManager.Instance.GetTilePosition(currentTile + i).position;

            //move the character towards the targetPosition
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed)*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            
            transform.position = targetPosition;
        }

        //jump to particular tile directly
        if(targetTile!=-1)
        {
            //get the position of next tile as a destination 
            Vector3 targetPosition = GameManager.Instance.GetTilePosition(targetTile).position;

            //move the character towards the targetPosition
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed) * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = targetPosition;
            currentTile = targetTile;
        }

        currentTile += steps;       //update the currentTile status of the character

        //determine characterNumber if -1 was passed
        if (characterNumber == -1)
            characterNumber = GetCharacterNumber();
        
        UIManager.Instance.UpdateCurrentTileText(currentTile, characterNumber);

        int newSortingOrder = 6;

        //check if there was already one of the opponent's character in that tile 
        if(GameManager.Instance.currentPlayer == 1)
        {
            foreach(GameObject x in ObjectHandler.Instance.player2Characters)
            {
                if (this.currentTile == x.GetComponent<Character>().currentTile && this != x.GetComponent<Character>())
                {
                    x.GetComponent<Character>().Damage(5);
                    //the following code will stack the latest character on the top of the stack in the tile
                    if (newSortingOrder <= x.GetComponent<SpriteRenderer>().sortingOrder)
                        newSortingOrder = x.GetComponent<SpriteRenderer>().sortingOrder+1;
                }
            }
        }
        else if(GameManager.Instance.currentPlayer == 2)
        {
            foreach (GameObject x in ObjectHandler.Instance.player1Characters)
            {
                if (this.currentTile == x.GetComponent<Character>().currentTile && this!=x.GetComponent<Character>())
                {
                    x.GetComponent<Character>().Damage(5);
                    if (newSortingOrder <= x.GetComponent<SpriteRenderer>().sortingOrder)
                        newSortingOrder = x.GetComponent<SpriteRenderer>().sortingOrder + 1;
                }
            }
        }

        
        _sprite.sortingOrder = newSortingOrder;

        GameManager.Instance.EndTurn();
    }

}
