using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//WILL PROBABLY MOVE METHODS AROUND WHEN CLASSES R IMPLEMENTED


public class Unit {

    //info about its position
    private GameObject unitClass; //info about this units class (ie archer) including its sprite and space in the game
    private Node currentLocation; //info about its location on the map in terms of graph theory

    //associations
    private Player general;    

    //Constructor for unit
    public Unit (Player owner, GameObject unitRolePrefab, Node starting) {
        general = owner;
        currentLocation = starting;
        unitClass = unitRolePrefab;
    }

    //getter for posX
    public int getPosX() {
        return currentLocation.getX();
    }

    //getter for posY
    public int getPosY() {
        return currentLocation.getY();
    }

    //get Units current HP
    public int getCurrentHP() {
        return unitClass.GetComponent<UnitClass>().getCurrentHP();
    }
    
    //getter for its node
    public Node getNode() {
        return currentLocation;
    }
    
    //getter for units player
    public Player getPlayer() {
        return general;
    }

    //gets info about the units class
    public GameObject getUnitClass() {
        return unitClass;
    }

    //sets our position to a new node
    public void setPosition(Node newLocation) {

        currentLocation.removeUnit(); //needs to remove unit info from current node
        currentLocation = newLocation; //sets our location to the new node
        currentLocation.setUnit(this); //gives the new node info about the unit
    }

    //returns true if this unit and another unit are on the same team
    public bool sameTeam(Unit unit) {
        return (getPlayer().getTeam() == unit.getPlayer().getTeam());
    }

    //removes unit from game when they die
    //removes unit reference from everything connected to it and the garbage collector will take care of the rest
    public void removeUnit() {
        unitClass.GetComponent<UnitClass>().destroy();
        currentLocation.removeUnit();
        general.removeUnit(this);
    }

    //DEBUGGING PURPOSES TO KNOW WHO THIS UNIT BELONGS TOO
    public string toString() {
        return general.getName() + "'s " + unitClass.name;
    }
}
