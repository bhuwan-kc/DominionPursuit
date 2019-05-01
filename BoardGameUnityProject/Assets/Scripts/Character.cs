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
    private Sprite characterImage = null;
    [SerializeField]
    private int maxHealth = 10;
    private int health;
    [SerializeField]
    private int team = 1;                       //what team a character is on. Need to make a function to set this later.
    [SerializeField] private int idNum = 0;     //used to uniquely ID a character. Used to ID characters for tile effects, along with team.
    //TODO: Make more useful character ID numbers. Maybe using their numbers their images are saved as.

    private SpriteRenderer _sprite;
    public GameObject Highlighter;

    private bool onAlternatePath = false;

    // Start is called before the first frame update
    void Start()
    {
        _sprite = this.GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    void Update()
    {
        if (health == 0 && (currentTile <= 0 || currentTile == 38))
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
        UIManager.Instance.UpdateCurrentTileText(currentTile, GetCharacterNumber(), team);
    }

    //get character team.
    public int GetTeam()
    {
        return team;
    }

    public Sprite GetSprite()
    {
        return characterImage;
    }

    //to move the character by the given steps 
    public void UpdateTile(int steps, bool activateTileEffect, bool endTurn)
    {
        //taking the character from it's starting position to tile 0
        if(currentTile == -1)
        {
            transform.position = ObjectHandler.Instance.tiles[0].transform.position;
            steps++;
        }

        if (currentTile > 0 && !onAlternatePath)
            ObjectHandler.Instance.tiles[currentTile].GetComponent<Tile>().LeaveTile(team, idNum); //updating current tile as the character leaves
        else if(onAlternatePath && currentTile > 47 && currentTile < 54)
            ObjectHandler.Instance.tilesAlternatePath[currentTile-47].GetComponent<Tile>().LeaveTile(team, idNum);

        StartCoroutine(TileTransitionStepsRoutine(steps, activateTileEffect, endTurn)); 
    }

    public void Damage(int x)
    {
        health -= x;

        if(health <= 0)
        {
            SoundManagerScript.PlaySound(SoundManagerScript.Sound.death);
            health = 0;
            //make game wait for character movement to complete
            GameManager.Instance.waitForCharacterMovement = true;
            //move to start if dead and have not reached checkpoint.
            if (currentTile < 38)
                StartCoroutine(TileTransitionDirectRoutine(0, false));
            else
                StartCoroutine(TileTransitionDirectRoutine(38, false));
        }
        else
            SoundManagerScript.PlaySound(SoundManagerScript.Sound.damage);

        UIManager.Instance.UpdateHealthBar(characterName, health);
    }

    public void Heal(int x)
    {
        if (health != maxHealth)
        {
            SoundManagerScript.PlaySound(SoundManagerScript.Sound.powerUp);
            ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("HEALING...\n" + name);
        }
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

    public void SetTeam(int team)
    {
        this.team = team;
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

    public void StackCharacterOnTile(bool attack)
    {
        //bring the character back to the ground level
        int newSortingOrder = 6;

        //Check if there are any characters from player2 on the new tile 
        foreach (GameObject x in ObjectHandler.Instance.player2Characters)
        {
            if (this.currentTile == x.GetComponent<Character>().currentTile && this != x.GetComponent<Character>())
            {
                //if on different paths 
                if (this.currentTile > 46 && this.currentTile < 54 && (this.onAlternatePath != x.GetComponent<Character>().onAlternatePath))
                    continue;

                //deal damage if current character belongs to player1
                if (attack && this.team != x.GetComponent<Character>().GetTeam() && this.currentTile != GameManager.Instance.finalTileNumber + 1)
                    x.GetComponent<Character>().Damage(GameManager.Instance.GetCharacterDamage());
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
                //if on different paths 
                if (this.currentTile > 46 && this.currentTile < 54 && (this.onAlternatePath != x.GetComponent<Character>().onAlternatePath))
                    continue;

                //deal damage if current character belongs to player2
                if (attack && this.team != x.GetComponent<Character>().GetTeam() && currentTile != GameManager.Instance.finalTileNumber + 1)
                    x.GetComponent<Character>().Damage(GameManager.Instance.GetCharacterDamage());
                if (newSortingOrder <= x.GetComponent<SpriteRenderer>().sortingOrder)
                    newSortingOrder = x.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }
        //move the character to the top of the stack
        _sprite.sortingOrder = newSortingOrder;
    }

    public void StackCharacterOnTileAndAttack()
    {
        StackCharacterOnTile(true);
    }

    public void AIPathDecision(int steps)
    {
        int targetTile = steps + currentTile;
        if (targetTile > 53 && targetTile < 47)
            return;

        //right side
        int rightWeight = ObjectHandler.Instance.tilesAlternatePath[targetTile-47].GetComponent<Tile>().GetTileWeight();
        if (rightWeight == -2 && this.health < GameManager.Instance.GetEventDamage())
            rightWeight -= 4; //discourage choosing death.
        if (ObjectHandler.Instance.tilesAlternatePath[targetTile-47].GetComponent<Tile>().CheckFaction() == 1 ||
            ObjectHandler.Instance.tilesAlternatePath[targetTile-47].GetComponent<Tile>().CheckFaction() == 3)
        {
            rightWeight += 1;
            if (ObjectHandler.Instance.AI.GetComponent<AI_attempt>().getAggression())
                rightWeight += 1;
        }

        //left side
        int leftWeight = ObjectHandler.Instance.tiles[targetTile].GetComponent<Tile>().GetTileWeight();
        if (leftWeight == -2 && this.health < GameManager.Instance.GetEventDamage())
            leftWeight -= 4; //discourage choosing death.
        if (ObjectHandler.Instance.tiles[targetTile].GetComponent<Tile>().CheckFaction() == 1 ||
            ObjectHandler.Instance.tiles[targetTile].GetComponent<Tile>().CheckFaction() == 3)
        {
            leftWeight += 1;
            if (ObjectHandler.Instance.AI.GetComponent<AI_attempt>().getAggression())
                leftWeight += 1;
        }

        //if weight of right path is better than left, go right.
        if (rightWeight > leftWeight)
            onAlternatePath = true;
    }

    //moves the character to the target tile directly
    IEnumerator TileTransitionDirectRoutine(int targetTile, bool endTurn)
    {
        //bring the character to the top layer
        _sprite.sortingOrder = 10;

        //update the currentTile counter of the character
        //this is done before the while loop to indicate any event happenning during the loop execution that the character's tile is targetTile
        SetCurrentTile(targetTile);             

        //get the position of the target Tile
        Vector3 targetPosition = GameManager.Instance.GetTilePosition(targetTile).position;

        //move the character towards the targetPosition
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed+2.0f) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPosition;    //for better alignment

        //position the character properly on the new tile
        StackCharacterOnTile(false);

        if (currentTile > 0)
            ObjectHandler.Instance.tiles[currentTile].GetComponent<Tile>().ArriveOnTile(team, idNum, false); //mark character is on new tile.

        //end the player's turn
        if (endTurn)
            GameManager.Instance.EndTurn();
        GameManager.Instance.waitForCharacterMovement = false;
    }

    //moves the character through the tiles by given steps
    IEnumerator TileTransitionStepsRoutine(int steps, bool activateTileEffect, bool endTurn)
    {
        //bring the character to the top layer
        _sprite.sortingOrder = 10;

        //move one tile at a time 
        for (int i = 1; i <= Mathf.Abs(steps); i++)
        {
            //division - give player an option to choose the path
            if(currentTile+i == 47 && steps > 0 && (currentTile+steps) < 54)
            {
                //AI decision
                if (GameManager.Instance.vsAI && GameManager.Instance.currentPlayer == 2)
                {
                    AIPathDecision(steps);
                }
                //player decision
                else
                {
                    MessageBox msg = ObjectHandler.Instance.GetMessageBox();
                    msg.DisplayMessageContinued("Which path do you want to move through?");
                    msg.DisplayButtons("Left", "Right");
                    while (!msg.buttonWasClicked)
                        yield return new WaitForEndOfFrame();
                    //if second route was selected
                    if (msg.buttonClicked == 2)
                        onAlternatePath = true;
                }
            }

            //get the position of next tile as a destination 
            Vector3 targetPosition;
            if (steps >= 0)
            {
                if(onAlternatePath && currentTile+i >= 47 && currentTile+i <= 53)
                    targetPosition = GameManager.Instance.GetAlternatePathTilePosition(currentTile + i).position;
                else
                    targetPosition = GameManager.Instance.GetTilePosition(currentTile + i).position;

                //reset onAlternatePath indicator
                if (onAlternatePath && (currentTile + i > 53 || currentTile + i < 47))
                    onAlternatePath = false;
            }
            else    //if moving backwards 
            {
                if (currentTile - i < 1)
                {
                    currentTile = 1 - steps;
                    break;
                }
                if (onAlternatePath && currentTile-i >= 47)
                    targetPosition = GameManager.Instance.GetAlternatePathTilePosition(currentTile - i).position;
                else 
                    targetPosition = GameManager.Instance.GetTilePosition(currentTile - i).position;

                //reset onAlternatePath indicator
                if (onAlternatePath && (currentTile - i > 53 || currentTile - i < 47))
                    onAlternatePath = false;
            }

            //move the character towards the targetPosition
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed) * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = targetPosition;    //to better position the character on the tile

            //if on final tile and is moving forward
            if (currentTile + i == GameManager.Instance.finalTileNumber && steps > 0)
            {
                //setting up the steps such that the next loop would take the character to tile 72 and break
                i = steps - 1;
                currentTile = GameManager.Instance.finalTileNumber - i;
                health = 10;
                UIManager.Instance.UpdateHealthBar(characterName, health);
                ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued(characterName + " reached the final tile!");
                yield return new WaitForSeconds(0.25f);
            }
        }
        SetCurrentTile(currentTile + steps);       //update the currentTile status of the character

        //position the character properly on the new tile
        StackCharacterOnTileAndAttack();

        //wait before the tile effect and end turn
        yield return new WaitForSeconds(0.5f);

        if (currentTile > 0 && currentTile < GameManager.Instance.finalTileNumber)
        {
            if(onAlternatePath)
                ObjectHandler.Instance.tilesAlternatePath[currentTile-47].GetComponent<Tile>().ArriveOnTile(team, idNum, activateTileEffect);
            else
                ObjectHandler.Instance.tiles[currentTile].GetComponent<Tile>().ArriveOnTile(team, idNum, activateTileEffect); //mark character is on new tile.
        }
        //end the player's turn
        if (endTurn || currentTile == GameManager.Instance.finalTileNumber+1)
            GameManager.Instance.EndTurn();
    }

}
