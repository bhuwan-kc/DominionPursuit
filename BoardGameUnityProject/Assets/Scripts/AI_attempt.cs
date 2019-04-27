using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_attempt : MonoBehaviour
{
    private static AI_attempt _ai;
    private static bool aggressive; //used to hold if the AI is aggressive or not.

    private void Start()
    {
        _ai = this;
        //ai is always aggressive, because it's more interesting this way (and not properly playtested the other way)
        aggressive = true;
    }

    public bool getAggression()
    {
        return aggressive;
    }

    //class variable decleration
    int[] tileWeight = new int[3]; //used to save weights of updated characterLocation tiles.


    //================================= main computer turn =======================================
    //computer's turn
    public void Comp_turn()
    {
        //declare to the player it is the AI's turn.
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Computer Turn.");


        //variable dec.
        int[] characterLocations = new int[3]; //used only to check where characters will go. does NOT store current location.
        int diceRoll1, diceRoll2; //store dice rolls for rolling animation.
        int move = 0;
        int cardDecision = -1; //used to mark what event card was used, in case later decisions need it.

        //grab character current location.
        for (int i = 0; i < 3; i++) {
            characterLocations[i] = ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetCurrentTile();
            if (characterLocations[i] >= GameManager.Instance.finalTileNumber) characterLocations[i] = -3; //-3 means never pick it.
            //-1 is off the board, but isn't really a space. So the AI will look at the space it will actually land on.
            else if (characterLocations[i] == -1) characterLocations[i] = 0;
        }

        //decide to use/not use event cards
        cardDecision = DecideEventCard();

        //generate diceroll for character movement.
        diceRoll1 = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();
        diceRoll2 = ObjectHandler.Instance.Dice.GetComponent<Dice>().RollDice();
        move = diceRoll1 + diceRoll2;
        if (cardDecision == 2) move += 4;

        //see what the weight of where each character moves to would be.
        for (int i = 0; i < 3; i++)
        {
            tileWeight[i] = FindMoveWeight(characterLocations[i], move, i);
        }

        //output move distance and weights of tiles characters would land on.
        //Debug.Log("AI rolled a total of " + move);
        //Debug.Log("tileWeight[0] is " + tileWeight[0] + " TileWeight[1] is " + tileWeight[1] + " Tileweight[2] is " + tileWeight[2]);

        StartCoroutine(DisplayDiceRoll(diceRoll1, diceRoll2, move)); //displays dice roll, then moves the appropriate character.

        return;
    }


    //========================================== AI event card stuff ============================================
    //AI deciding to use/not use event cards
    public int DecideEventCard()
    {
        int decision = -1; //-1: use no event card. x: use card type x (0-3)
        int target = -1; //-1: no target. Otherwise 0-2 indicate target character in array.
        bool cantDecide = false; //if conditions exist but aren't ideal, mark for AI to think about it later
        int tileMin = 12; //minimum location to use event cards on a character. Exclusive.

        //print ai event cards
        /*
        Debug.Log("AI Hand:\n " +
            "Medkits: " + ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(0) +
            " Sabotage: " + ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(1) +
            " Shortcut: " + ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(2) +
            " Detour: " + ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(3));
        */

        //if AI has detour card, use it on the furthermost enemy character.
        if (ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(3) > 0)
        {
            int furthest = 1;
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile() == 72)
                    continue;
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile() > furthest &&
                    ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile() > tileMin)
                {
                    furthest = ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile();
                    decision = 3;
                    target = i;
                    cantDecide = true;
                }
            }
        }

        //if AI has a shortcut card, use it
        //doesn't care about tile minimums, but only a 50% chance to use it every turn.
        if (ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(2) > 0 &&
            Random.Range(0, 2) == 0)
        {
            decision = 2;
            target = -1; //no target needed for this function. 
        }

        //if AI has a damage card, search for anyone at less than max hp at or beyond tile 12.
        //TODO: consider character location in decision
        if (ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(1) > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile() == 72)
                    continue;
                if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetCurrentTile() > tileMin)
                {
                    //if (event damage) or less hp, KILL THEM
                    if (ObjectHandler.Instance.player1Characters[i].GetComponent<Character>().GetHealth() <= GameManager.Instance.GetEventDamage())
                    {
                        decision = 1;
                        target = i;
                        break;
                    }
                    //if >(event damage) hp, consider damaging them.
                    else if(ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(1) > 1)
                    {
                        decision = 1;
                        target = i;
                        cantDecide = true;
                    }
                }
            }
            //if none of the opponent's character have less hp and AI has more than 1 damage card, then use at randome
            if(decision != 1 && ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(1) > 1)
            {
                decision = 1;
                target = Random.Range(0, 3);
                cantDecide = true;
            }
        }

        //If AI has a healing card, search for anyone at (tile damage) or less beyond tile 12. If true, heal them.
        if (ObjectHandler.Instance.eventCards.GetComponent<EventCards>().getPlayer2EventCardCounts(0) > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetCurrentTile() == 72)
                    continue;
                if ((ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetHealth() <=
                    ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetMaxHealth() - GameManager.Instance.GetEventHeal()) &&
                    ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetCurrentTile() > tileMin)
                {
                    //if already decided to heal someone, check if new person has less hp than other character.
                    //if so, heal them instead.
                    //TODO: Consider location of characters in healing
                    if (decision == 0 && ObjectHandler.Instance.player2Characters[target].GetComponent<Character>().GetHealth()
                        > ObjectHandler.Instance.player2Characters[i].GetComponent<Character>().GetHealth())
                    {
                        target = i;
                    }
                    else
                    {
                        decision = 0;
                        target = i;
                    }

                }
            }
        }

        //if needed to think on it, do so here.
        //for now, just randomly determines (25-75% chance of yes/no)
        //TODO: More complicated thought processes.
        if (cantDecide)
        {
            if (Random.Range(0f, 1f) <= 0.75f)
            {
                decision = -1;
            }
        }

        //Debug.Log("decision within eventCardDecision is " + decision);

        //events listed in order they're checked above
        if (decision == -1) return decision; //here just to make computer not do the following checks if it doesn't want to use an event card
        else if (decision == 0) StartCoroutine(useMedkit(target));
        else if (decision == 3) StartCoroutine(useDetour(target));
        else if (decision == 1) StartCoroutine(useSabotage(target));
        else if (decision == 2) StartCoroutine(useShortcut());

        GameManager.Instance.waitForCharacterMovement = true;
        return decision;
    }

    //==================================== AI version of using event cards ======================================
    //healing card
    private IEnumerator useMedkit(int target)
    {
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.powerUp);
        ObjectHandler.Instance.eventCards.GetComponent<EventCards>().UpdateEventCardCount(GameManager.Instance.currentPlayer, 0, false);
        if (target < 0 || target > 2)
        {
            Debug.Log("invalid target passed to useCard0 by AI. Defaulting to 0.");
            target = 0;
        }
        ObjectHandler.Instance.player2Characters[target].GetComponent<Character>().Heal(GameManager.Instance.GetEventHeal());
        ObjectHandler.Instance.messageBoxObj.GetComponent<MessageBox>().DisplayMessageContinued(ObjectHandler.Instance.player2Characters[target].GetComponent<Character>().GetName() +
            " heals from a Medkit.");
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.waitForCharacterMovement = false;
    }

    //damaging card
    private IEnumerator useSabotage(int target)
    {
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.powerUp);
        ObjectHandler.Instance.eventCards.GetComponent<EventCards>().UpdateEventCardCount(GameManager.Instance.currentPlayer, 1, false);
        if (target < 0 || target > 2)
        {
            Debug.Log("invalid target passed to useCard1 by AI. Defaulting to 0.");
            target = 0;
        }
        ObjectHandler.Instance.player1Characters[target].GetComponent<Character>().Damage(GameManager.Instance.GetEventDamage());
        ObjectHandler.Instance.messageBoxObj.GetComponent<MessageBox>().DisplayMessageContinued(ObjectHandler.Instance.player1Characters[target].GetComponent<Character>().GetName() +
            " suffers "+GameManager.Instance.GetEventDamage()+" damage from a Sabotage!");
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.waitForCharacterMovement = false;
    }

    //movement card
    private IEnumerator useShortcut()
    {
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.powerUp);
        ObjectHandler.Instance.eventCards.GetComponent<EventCards>().UpdateEventCardCount(GameManager.Instance.currentPlayer, 2, false);
        //NOTE Ai doesn't do anything here, as it doesn't use the same diceroll functions as a player does.
        //merely marking it used this card is enough (the return on the DecideEventCard function).
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("AI is taking a shortcut!");
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.waitForCharacterMovement = false;
    }

    //backwards movement card
    private IEnumerator useDetour(int target)
    {
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.powerUp);
        ObjectHandler.Instance.player1Characters[target].GetComponent<Character>().UpdateTile(GameManager.Instance.GetEventBackwardMoves()*(-1), false, false);
        ObjectHandler.Instance.eventCards.GetComponent<EventCards>().UpdateEventCardCount(GameManager.Instance.currentPlayer, 3, false);
        if (target < 0 || target > 2)
        {
            Debug.Log("invalid target passed to useCard3 by AI. Defaulting to 0.");
            target = 0;
        }
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("AI uses Detour on " + ObjectHandler.Instance.player1Characters[target].GetComponent<Character>().GetName());
        yield return new WaitForSeconds(3.0f);
        GameManager.Instance.waitForCharacterMovement = false;
    }


    //===================================== AI movement stuff =========================================
    //find weight of next move. All movement related decision making goes here.
    private int FindMoveWeight(int location, int move, int arrayLocation)
    {
        int tileWeight = -10;

        //if character is at start and all moves are neutral, get character out of start safely.
        if (location == 0 && ObjectHandler.Instance.tiles[move].GetComponent<Tile>().GetTileWeight() == 0)
        {
            tileWeight = 1;
            //if tile is occupied it'll favor moving a character to that location.
            if (ObjectHandler.Instance.tiles[move].GetComponent<Tile>().CheckFaction() == 1 || ObjectHandler.Instance.tiles[move].GetComponent<Tile>().CheckFaction() == 3)
            {
                tileWeight += 2;
                if (aggressive) tileWeight += 3;
            }
        }

        //if a character will move to the end of the board, do that. 
        else if (location != -3 && location + move >= GameManager.Instance.finalTileNumber) tileWeight = 20;

        //else if the move stats before the split and ends after, decide based off both paths.
        else if (location != -3 && location <= 46 && location + move <= 53 && location + move >= 47)
        {
            int targetTile = location + move;

            //right side
            int rightTileWeight = ObjectHandler.Instance.tilesAlternatePath[targetTile - 47].GetComponent<Tile>().GetTileWeight();
            if (rightTileWeight == -2 && ObjectHandler.Instance.player2Characters[arrayLocation].GetComponent<Character>().GetHealth() 
                < GameManager.Instance.GetEventDamage())
                rightTileWeight -= 4; //discourage choosing death.
            if (ObjectHandler.Instance.tilesAlternatePath[targetTile - 47].GetComponent<Tile>().CheckFaction() == 1 ||
                ObjectHandler.Instance.tilesAlternatePath[targetTile - 47].GetComponent<Tile>().CheckFaction() == 3)
            {
                rightTileWeight += 1;
                if (ObjectHandler.Instance.AI.GetComponent<AI_attempt>().getAggression())
                    rightTileWeight += 1;
            }

            //left side
            int leftTileWeight = ObjectHandler.Instance.tiles[targetTile].GetComponent<Tile>().GetTileWeight();
            if (leftTileWeight == -2 && ObjectHandler.Instance.player2Characters[arrayLocation].GetComponent<Character>().GetHealth() 
                < GameManager.Instance.GetEventDamage())
                leftTileWeight -= 4; //discourage choosing death.
            if (ObjectHandler.Instance.tiles[targetTile].GetComponent<Tile>().CheckFaction() == 1 ||
                ObjectHandler.Instance.tiles[targetTile].GetComponent<Tile>().CheckFaction() == 3)
            {
                leftTileWeight += 1;
                if (ObjectHandler.Instance.AI.GetComponent<AI_attempt>().getAggression())
                    leftTileWeight += 1;
            }
            
            //--------------------------------------final decision------------------------------------------
            //Debug.Log("Route Decision: leftWeight is " + leftTileWeight + " and rightWeight is " + rightTileWeight);
            if (rightTileWeight > leftTileWeight)
                tileWeight = rightTileWeight;
            else
                tileWeight = leftTileWeight;
        }

        //else find weight of general movement
        else if (location != -3)
        {
            location += move;
            tileWeight = ObjectHandler.Instance.tiles[location].GetComponent<Tile>().GetTileWeight();

            //if a character would die from landing on the space, heavily discourage the move.
            if (tileWeight == -2 && ObjectHandler.Instance.player2Characters[arrayLocation].GetComponent<Character>().GetHealth() <= GameManager.Instance.GetEventDamage())
                tileWeight -= 4;

            //if tile is occupied by an enemy, view it as more important to land on if it isn't a trap location.
            if ((ObjectHandler.Instance.tiles[location].GetComponent<Tile>().CheckFaction() == 1 ||
                ObjectHandler.Instance.tiles[location].GetComponent<Tile>().CheckFaction() == 3) &&
                    tileWeight > -5)
            {
                //if a character is at max HP, don't bother landing on them on a damage tile.
                //will crash if no character from player 1 is at the required tile. Should be ok due to faction check above.
                int characterNum = -1; //initialized before assignment.
                characterNum = GameManager.Instance.findCharacterAtLocation(location);
                if (characterNum != -1 && ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().GetHealth() == ObjectHandler.Instance.player1Characters[characterNum].GetComponent<Character>().GetMaxHealth() && tileWeight == -2)
                {
                        //don't do anything if it's a damage tile and they have full hp
                        //likely a better way to do this, but I'm too tired to figure it out atm.
                        //TODO: Make this better.
                }
                else { 
                    if (!aggressive) tileWeight += 1;
                    else tileWeight += 3;
                }
            }
            //if a character is at full hp, a health tile is neutral.
            if (tileWeight == 2 && ObjectHandler.Instance.player2Characters[arrayLocation].GetComponent<Character>().GetHealth() ==
                ObjectHandler.Instance.player2Characters[arrayLocation].GetComponent<Character>().GetMaxHealth())
            {
                tileWeight -= 2;
            }
            //if a character is moving from before the checkpoint to after, weight it higher.
            //have to subtract move from initial location due to move being added in earlier. Easier to read than function call.
            if (location - move < 38 && location > 38) tileWeight += 5;
        }
        return tileWeight;
    }

    //========================= dice roll animation ==============================
    //wait for dice roll and then move best character choice.
    private IEnumerator DisplayDiceRoll(int roll1, int roll2, int move)
    {
        //this function plays the animation to roll the dice.
        while (GameManager.Instance.waitForCharacterMovement)
            yield return new WaitForEndOfFrame();

        //to play the dice roll animation + sound
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.rollDice);
        ObjectHandler.Instance.Dice.GetComponent<Dice>().DiceRollAnimation();
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().DiceRollAnimation();

        float waitTime = GameManager.Instance.GetDiceRollAnimTime();

        //set dice face.
        ObjectHandler.Instance.Dice.GetComponent<Dice>().SetDiceFace(roll1);
        ObjectHandler.Instance.Dice2.GetComponent<Dice>().SetDiceFace(roll2);
        yield return new WaitForSeconds(waitTime);
        MakeBestMove(move);

    }

    //=============================== actually make the move ===============================
    //find the best move in the array. Returns what character moved, though it's unused.
    private int MakeBestMove(int move)
    {
        int charToMove = 0; //default to moving character 0.
        int best = -10; //default value that will be overwritten by all other options.

        //find best move possible.
        for (int i = 0; i < 3; i++)
        {
            if (best < tileWeight[i])
            {
                best = tileWeight[i];
                charToMove = i;
            }
        }

        //if characters have identical weights with a move, randomly choose one to move.
        //this is to encourage the AI not to just move one person across the board at a time.
        //While effective, that strategy is boring to play against
        if (tileWeight[0] == tileWeight[1] && tileWeight[1] == tileWeight[2]) charToMove = Random.Range(0, 3);
        else if (tileWeight[0] == tileWeight[1] && best == tileWeight[0]) charToMove = Random.Range(0, 2);
        else if (tileWeight[1] == tileWeight[2] && best == tileWeight[1]) charToMove = Random.Range(1, 3);
        else if (tileWeight[0] == tileWeight[2] && best == tileWeight[0])
        {
            int temp = Random.Range(0, 2);
            if (temp == 0) charToMove = 0;
            else charToMove = 2;
        }

        //make the move
        ObjectHandler.Instance.player2Characters[charToMove].GetComponent<Character>().UpdateTile(move, true, false);
        ObjectHandler.Instance.GetMessageBox().DisplayMessageContinued("Moving " + 
            ObjectHandler.Instance.player2Characters[charToMove].GetComponent<Character>().GetName() + " to " + (move +
                ObjectHandler.Instance.player2Characters[charToMove].GetComponent<Character>().GetCurrentTile()));

        return charToMove;
    }
}

