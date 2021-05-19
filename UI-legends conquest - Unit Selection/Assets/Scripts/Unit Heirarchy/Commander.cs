using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : UnitClass {
    private int currentAtk;
    private int specialTurnCounter = 0;

    //gets current attack
    public override int getCurrentAttack() {
        return currentAtk;
    }

    //gets current defence
    public override int getCurrentDefence() {
        return def;
    }

    //gets current attack range
    public override int getCurrentRange() {
        return attackRange;
    }

    //increases atk for x amount of turns
    public override void special () {

        if(noSpecialUsed && currentStamina > 1) {
            currentAtk = atk * 2;
            specialTurnCounter = 0;
            currentStamina = currentStamina - 1;
            noSpecialUsed = false;
        }
    }

    //initializes stats because start and upkeep doesnt work for initializing hp without having hp be refilled at the start of the turn
    public override void initialize(Unit unit) {
        currentHP = maxHP;
        currentStamina = maxStamina;
        currentAtk = atk;
        this.unit = unit;
        TextField.text = currentHP.ToString();
    }

    //upkeeps values at the start of turn
    public override void upkeep() {
        currentStamina = maxStamina;
        noSpecialUsed = true;

        //sets atk if its the units first turn of action or if its special wares off this turn
        if (specialTurnCounter < 1) //special buff is on a 2 turn clock
            specialTurnCounter++;

        else
            currentAtk = atk;
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

        //moves a unit until they arrive at thier destination
        if (currentPath.Count > 1) {

            //if unit gets close enough to tile's center will set units position to it so we can start moving to the next tile
            if (Vector3.Distance(transform.position, currentPath[1].getWorldCoordinates()) < 0.05f)
                transform.position = currentPath[1].getWorldCoordinates();	// Update our unity world position

            transform.position = Vector3.Lerp(transform.position, currentPath[1].getWorldCoordinates(), movementSpeed * Time.deltaTime);

            //once we made it to the new tile lets us start moving to the next
            if (Vector3.Distance(transform.position, currentPath[1].getWorldCoordinates()) == 0)
                currentPath.RemoveAt(0); //removes the current location in our path
        }
    }
}
