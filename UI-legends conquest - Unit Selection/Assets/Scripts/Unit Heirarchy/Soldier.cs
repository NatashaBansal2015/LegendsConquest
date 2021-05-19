using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : UnitClass {
    int currentAtk;
    int currentDef;

    //gets current attack
    public override int getCurrentAttack() {
        return currentAtk;
    }

    //gets current defence
    public override int getCurrentDefence() {
        return currentDef;
    }

    //gets current attack range
    public override int getCurrentRange() {
        return attackRange;
    }

    //initializes stats because start and upkeep doesnt work for initializing hp without having hp be refilled at the start of the turn
    public override void initialize(Unit unit) {
        currentHP = maxHP;
        currentStamina = maxStamina;
        currentAtk = atk;
        currentDef = def;
        this.unit = unit;
        TextField.text = currentHP.ToString();
    }

    //if unit did not use special already this turn raises atk and def until the start of the next turn
    public override void special() { 
        if (noSpecialUsed && currentStamina > 0) {
            currentAtk = atk * 2;
            currentDef = def * 2;
            currentStamina --;
            noSpecialUsed = false;
        }
    }

    //upkeeps values at the start of turn
    public override void upkeep() {
        currentStamina = maxStamina;
        noSpecialUsed = true;
        currentAtk = atk;
        currentDef = def;
    }

    // Start is called before the first frame update
    void Start()
    {
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
