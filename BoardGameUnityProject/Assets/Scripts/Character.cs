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
    private string character_name;    //for future use

    // Start is called before the first frame update
    void Start()
    {
        currentTile = 0;          
    }

    //to move the character by the given steps 
    public void UpdateTile(int steps)
    {
        StartCoroutine(TileTransitionRoutine(steps));
    }

    //moves the character through the tiles
    IEnumerator TileTransitionRoutine(int steps)
    {
        //move one tile at a time 
        for (int i = 0; i < steps; i++)
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
        UIManager.Instance.UpdateCurrentTileText(currentTile);

        GameManager.Instance.EndTurn();
    }

}
