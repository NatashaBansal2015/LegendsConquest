using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : UnitClass {

    private int currentRange;
    private int specialTimer = 0;

    //gets current attack
    public override int getCurrentAttack() {
        return atk;
    }

    //gets current defence
    public override int getCurrentDefence() {
        return def;
    }

    //gets current range
    public override int getCurrentRange() {
        return currentRange;
    }

    //initializes stats because start and upkeep doesnt work for initializing hp without having hp be refilled at the start of the turn
    public override void initialize(Unit unit) {
        currentHP = maxHP;
        TextField.text = currentHP.ToString();
        currentStamina = maxStamina;
        currentRange = attackRange;
        this.unit = unit;
    }

    //allows the archer to increase their attack range for this turn and the next
    public override void special () {

        if (noSpecialUsed && currentStamina > 0) {
            specialTimer = 0;
            currentRange = attackRange + 1;
            currentStamina--;
        }
    }

    //upkeeps values at the start of turn
    public override void upkeep() {
        currentStamina = maxStamina;

        if (specialTimer > 0) //if unit had extra range for 1 FULL turn revert it
            currentRange = attackRange;
        else
            specialTimer++;
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
