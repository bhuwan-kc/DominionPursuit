using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A collection of game objects handlers
public class ObjectHandler : MonoBehaviour
{
    //setting up a static instance of the class 
    private static ObjectHandler _instance;

    public static ObjectHandler Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                Debug.Log("ObjectHandler object is null");
                return null;
            }
        }
    }

    private void Awake()
    {
        _instance = this;
    }


    //GAMEOBJECTS
    public GameObject Dice;
    public GameObject Dice2;
    public GameObject[] player1Characters = new GameObject[3];
    public GameObject[] player2Characters = new GameObject[3];
	//an array of all the tiles on the board
    public GameObject[] tiles = new GameObject[80];     



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
