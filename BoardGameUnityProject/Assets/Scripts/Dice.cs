using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private Animator diceAnim;

    // Start is called before the first frame update
    void Start()
    {
        diceAnim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int RollDice()
    {
        int r = Random.Range(1, 6);
        return r;
    }

    public void DiceRollAnimation()
    {
        diceAnim.SetInteger("diceFace", 0);
        diceAnim.SetTrigger("rollDice");
    }

    public void SetDiceFace(int face)
    {
        diceAnim.SetInteger("diceFace", face);
    }
}
