using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    //animator component of the dice object
    private Animator diceAnim;

    // Start is called before the first frame update
    void Start()
    {
        diceAnim = this.GetComponent<Animator>();
    }

    //to generate a random dice output
    public int RollDice()
    {
        int r = Random.Range(1, 7);
        return r;
    }

    //to set the dice to roll state
    public void DiceRollAnimation()
    {
        diceAnim.SetInteger("diceFace", 0);
        diceAnim.SetTrigger("rollDice");
    }

    //to set the dice to a certain face state
    public void SetDiceFace(int face)
    {
        diceAnim.SetInteger("diceFace", face);
    }
}
