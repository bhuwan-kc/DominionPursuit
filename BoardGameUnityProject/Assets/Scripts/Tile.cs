using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Used to weight each tile for the AI based on how positive it is.
    //-5 is awful, 0 is neutral, 5 is great.
    [SerializeField] private int tileWeight;
    
    public int getTileWeight()
    {

        return tileWeight;
    }

    public int tileEffect()
    {
        //tile effect will go here. Use weight as an ID for event?
        return 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
