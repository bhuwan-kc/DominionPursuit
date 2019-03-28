using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class to handle character behavior
public class Character : MonoBehaviour
{
    private int currentTile;        //to track the current position of the character
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

    }

    //to move the character by the given steps 
    public void updateTile(int steps, int number)
    {
        if(currentTile == -1)
        {
            transform.position = ObjectHandler.Instance.tiles[0].transform.position;
            steps++;
        }
        StartCoroutine(TileTransitionRoutine(steps, number));
    }

    public void Damage(int x)
    {
        health -= x;
        UIManager.Instance.UpdateHealthBar(characterName, health);

        if(health < 0)
        {
            health = 0;
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

    //moves the character through the tiles
    IEnumerator TileTransitionRoutine(int steps, int characterNumber)
    {
        _sprite.sortingOrder = 7;   

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

        currentTile += steps;       //update the currentTile status of the character
        UIManager.Instance.UpdateCurrentTileText(currentTile, characterNumber);

        //check if there was already one of the opponent's character in that tile 
        if(GameManager.Instance.currentPlayer == 1)
        {
            foreach(GameObject x in ObjectHandler.Instance.player2Characters)
            {
                if (this.currentTile == x.GetComponent<Character>().currentTile && this != x.GetComponent<Character>())
                    x.GetComponent<Character>().Damage(5);
            }
        }
        else if(GameManager.Instance.currentPlayer == 2)
        {
            foreach (GameObject x in ObjectHandler.Instance.player1Characters)
            {
                if (this.currentTile == x.GetComponent<Character>().currentTile && this!=x.GetComponent<Character>())
                    x.GetComponent<Character>().Damage(5);
            }
        }

        
        _sprite.sortingOrder = 6;

        GameManager.Instance.EndTurn();
    }

}
