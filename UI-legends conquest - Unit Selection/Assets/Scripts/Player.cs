using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//REMOVED PLAYER AS AN OBJECT BECAUSE ALL IT NEEDS TO DO IS DISTINGUISED WHO IS WHO AND WHICH PLAYER OWNS WHICH UNITS
//HAS NO PHYSICL SPACE ON MAP

public class Player
{

    private GameManager gm; // the game manager that will handel this player along with its info
    private bool playersTurn = false;
    private int teamNumber;

    private List<Unit> army; //the army under the players control
    private string name;
    private bool baseCaptured = false;
    private Node objectiveNode; //the players node to protect

    public Player(GameManager gm, string name, int team)
    {
        this.gm = gm;
        this.name = name;
        teamNumber = team;
    }

    //setter for player's army
    public void setArmy(List<Unit> army)
    {
        this.army = army;
    }

    //Getter for Army
    public List<Unit> getArmy()
    {
        return army;
    }

    //returns the player's base
    public Node getBase()
    {
        return objectiveNode;
    }

    //checks if there is a commander in the player's army, true if there is
    public bool isThereCommander()
    {
        foreach (Unit u in army)
        {
            if (u.getUnitClass().GetComponent<UnitClass>().getName() == "Commander")
                return true;
        }

        return false;
    }

    //sets the base
    public void setBase(Node b)
    {
        objectiveNode = b;
        objectiveNode.setPlayer(this);
    }

    //player lose the base
    public void lostBase()
    {
        baseCaptured = true;
    }

    //getter for Players team
    public int getTeam()
    {
        return teamNumber;
    }

    //runs through the player's army and performs each unit's upkeep actions
    public void upkeep()
    {

        playersTurn = true;
        foreach (Unit u in army)
        {
            u.getUnitClass().GetComponent<UnitClass>().upkeep();
        }
    }

    //if a unit dies we remove it from the army
    public void removeUnit(Unit deadGuy)
    {

        if (army.Contains(deadGuy))
        {
            army.Remove(deadGuy);
        }

        else
        {
            Debug.Log("tried to delete unit that does not belong to this player");
        }
    }

    //when a player loses the game remove them from the game
    public void deletePlayer()
    {

        //delete the army
        while (army.Count > 0)
        {
            army[0].removeUnit();
        }
        gm = null;
    }

    //returns true if that players base has been caputred
    public bool hasBaseCaptured()
    {
        return baseCaptured;
    }


    //if its the players turn
    public bool isPlayerActive()
    {
        return playersTurn;
    }

    //ends the players turn
    public void revokePlayersTurn()
    {
        playersTurn = false;
    }

    public string getName()
    {
        return name;
    }
}
