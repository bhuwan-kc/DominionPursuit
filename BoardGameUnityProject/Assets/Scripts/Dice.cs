using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int RollDice()
    {
        int r = Random.Range(1, 6);

        Debug.Log("Dice - " + r);

        return r;
    }
}
