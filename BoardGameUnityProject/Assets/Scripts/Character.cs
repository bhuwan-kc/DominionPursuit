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

    public SpriteRenderer _sprite;
    public GameObject Highlighter;

    // Start is called before the first frame update
    void Start()
    {
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
        }
        StartCoroutine(TileTransitionRoutine(steps, number));
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
        _sprite.sortingOrder = 6;

        GameManager.Instance.EndTurn();
    }

}
