using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class to handle character behavior
public class Character : MonoBehaviour
{
    [SerializeField]
    private int currentTile = -1;       //to track the current position of the character
    [SerializeField]
    private float  speed = 1.25f;       //the speed with which character moves across tiles
    [SerializeField]
    private string characterName = "Unnamed";
    [SerializeField]
    private int maxHealth = 10;
    private int health;
    [SerializeField]
    private int team = 1;               //what team a character is on. Need to make a function to set this later.
    [SerializeField] private int idNum = 0; //used to uniquely ID a character. Used to ID characters for tile effects, along with team.
    //TODO: Make more useful character ID numbers. Maybe using their numbers their images are saved as.

    private SpriteRenderer _sprite;
    public GameObject Highlighter;

    // Start is called before the first frame update
    void Start()
    {
        _sprite = this.GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    void Update()
    {
        if (health == 0 && (currentTile == 0 || currentTile == 38))
        {
            health = maxHealth;
            UIManager.Instance.UpdateHealthBar(characterName, health);
        }
    }

    //grab current character tile.
    public int GetCurrentTile()
    {
        if (currentTile < 0)
            return 0;
        return currentTile;
    }

    public void SetCurrentTile(int tile)
    {
        if (tile < 0)
            tile = 0;
        else if (tile > 78)
            tile = 78;

        currentTile = tile;
        UIManager.Instance.UpdateCurrentTileText(currentTile, GetCharacterNumber());
    }

    //get character team.
    public int GetTeam()
    {
        return team;
    }

    //to move the character by the given steps 
    public void UpdateTile(int steps, bool activateTileEffect)
    {
        //taking the character from it's starting position to tile 0
        if(currentTile == -1)
        {
            transform.position = ObjectHandler.Instance.tiles[0].transform.position;
            steps++;
        }

        if (currentTile > 0)
            ObjectHandler.Instance.tiles[currentTile].GetComponent<Tile>().LeaveTile(team, idNum); //updating current tile as the character leaves

        StartCoroutine(TileTransitionStepsRoutine(steps, activateTileEffect));  //!waitForTileEffect == do not end the turn
    }

    public void Damage(int x)
    {
        health -= x;

        if(health <= 0)
        {
            health = 0;
            //move to start if dead and have not reached checkpoint.
            if (currentTile < 38)
                StartCoroutine(TileTransitionDirectRoutine(0, false));
            else
                StartCoroutine(TileTransitionDirectRoutine(38, false));
        }

        UIManager.Instance.UpdateHealthBar(characterName, health);
    }

    public void Heal(int x)
    {
        //check if healing would take over max. If so, set health to max health. Otherwise heal.
        if (health + x > maxHealth)
            health = maxHealth;
        else
            health += x;

        UIManager.Instance.UpdateHealthBar(characterName, health);
        return;
    }

    public int GetHealth()
    {
        return health; 
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public string GetName()
    {
        return characterName;
    }

    public int GetID()
    {
        return idNum;
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

    public void StackCharacterOnTile()
    {
        //bring the character back to the ground level
        int newSortingOrder = 6;
        
        //Check if there are any characters from player2 on the new tile 
        foreach (GameObject x in ObjectHandler.Instance.player2Characters)
        {
            if (this.currentTile == x.GetComponent<Character>().currentTile && this != x.GetComponent<Character>())
            {
                //deal damage if current character belongs to player1
                if (GameManager.Instance.currentPlayer == 1)
                    x.GetComponent<Character>().Damage(5);
                //to determine the layer sorting order so that the current character can be kept at the top
                if (newSortingOrder <= x.GetComponent<SpriteRenderer>().sortingOrder)
                    newSortingOrder = x.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }
        //Check if there are any characters from player1 on the new tile 
        foreach (GameObject x in ObjectHandler.Instance.player1Characters)
        {
            if (this.currentTile == x.GetComponent<Character>().currentTile && this != x.GetComponent<Character>())
            {
                //deal damage if current character belongs to player2
                if (GameManager.Instance.currentPlayer == 2)
                    x.GetComponent<Character>().Damage(5);
                if (newSortingOrder <= x.GetComponent<SpriteRenderer>().sortingOrder)
                    newSortingOrder = x.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }
        //move the character to the top of the stack
        _sprite.sortingOrder = newSortingOrder;
    }

    //moves the character to the target tile directly
    IEnumerator TileTransitionDirectRoutine(int targetTile, bool endTurn)
    {
        //bring the character to the top layer
        _sprite.sortingOrder = 10;

        //get the position of the target Tile
        Vector3 targetPosition = GameManager.Instance.GetTilePosition(targetTile).position;

        //move the character towards the targetPosition
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed+2.5f) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPosition;    //for better alignment
        SetCurrentTile(targetTile);             //update the currentTile counter of the character

        //position the character properly on the new tile
        StackCharacterOnTile();

        if (currentTile > 0)
            ObjectHandler.Instance.tiles[currentTile].GetComponent<Tile>().ArriveOnTile(team, idNum, !endTurn); //mark character is on new tile.

        //end the player's turn
        if (endTurn)
            GameManager.Instance.EndTurn();
    }

    //moves the character through the tiles by given steps
    IEnumerator TileTransitionStepsRoutine(int steps, bool activateTileEffect)
    {
        //bring the character to the top layer
        _sprite.sortingOrder = 10;

        //move one tile at a time 
        for (int i = 1; i <= Mathf.Abs(steps); i++)
        {
            //to connect tile 53 with 61
            if(currentTile+i == 54)
            {
                currentTile = 61-i;
            }

            //division - give player an option to choose the path
            else if(currentTile+i == 47)
            {
                MessageBox msg = ObjectHandler.Instance.GetMessageBox();
                msg.DisplayMessage("Which path do you want to move through?");
                msg.DisplayButtons("47","54");
                while (!msg.buttonWasClicked)
                    yield return new WaitForEndOfFrame();
                //if second route was selected
                if (msg.buttonClicked == 2)
                    currentTile = 54-i;
            }

            //get the position of next tile as a destination 
            Vector3 targetPosition;
            if (steps >= 0)
            {
                if (currentTile + i > 78)
                    break;
                targetPosition = GameManager.Instance.GetTilePosition(currentTile + i).position;
            }
            else    //if moving backwards 
            {
                if (currentTile - i < 1)
                    break;
                targetPosition = GameManager.Instance.GetTilePosition(currentTile - i).position;
            }
            //move the character towards the targetPosition
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed) * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = targetPosition;    //to better position the character on the tile
        }
        SetCurrentTile(currentTile + steps);       //update the currentTile status of the character

        //position the character properly on the new tile
        StackCharacterOnTile();

        //wait before the tile effect and end turn
        yield return new WaitForSeconds(0.5f);

        if (currentTile > 0 && currentTile < 78)
            ObjectHandler.Instance.tiles[currentTile].GetComponent<Tile>().ArriveOnTile(team, idNum, activateTileEffect); //mark character is on new tile.

        //end the player's turn
        if (!activateTileEffect)
            GameManager.Instance.EndTurn();
    }

}
