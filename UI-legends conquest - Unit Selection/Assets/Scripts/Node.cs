using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A REMINDER TO REMOVE UNIT FROM NODE WHEN THEY MOVE OFF IT LATER


public class Node {

	//info about node's location
	private List<Node> adjacent;
	private int x;
	private int y;

	//info about units on tile
	private Unit unitOnTile;
	private bool noUnit = true;

	//info about node if it is a goal node for a player to capture
	private bool baseNode = false;
	private Player playerBase;

	//constructor for Node to initialize its position on the map and who it is adjacent to
	public Node(int x, int y) {
		
		adjacent = new List<Node>();
		
		this.x = x;
		this.y = y;
	}

	//Getter for adjacent nodes
	public List<Node> getAdjacent() {
		return adjacent;
	}

	//getter for unit in this node
	public Unit getUnit() {
		return unitOnTile;
	}

	public Player getPlayer() {
		return playerBase;
	}

	//getter for the x coordinate
	public int getX() {
		return x;
	}

	//getter for the y coordinate
	public int getY() {
		return y;
	}

	//returns true if there is no unit on tile
	public bool isEmpty() {
		return noUnit;
	}

	//returns the world coordinates for the node
	public Vector3 getWorldCoordinates() {
		return new Vector3(x, y, 0);
	}

	//setter for the unit on the node
	public void setUnit(Unit thisGuy) {
		unitOnTile = thisGuy;
		noUnit = false;

		if (baseNode && unitOnTile.getPlayer().getTeam() != playerBase.getTeam()) {
			playerBase.lostBase();
		}
	}

	//gives the node to a player if this node is a base node
	public void setPlayer(Player player) {
		playerBase = player;
		baseNode = true; //also need to distinguish this as a base node
	}

	//removes info about current unit on this tile so they can be moved to somewhere else
	public void removeUnit() {
		unitOnTile = null;
		noUnit = true;
	}

	//returns false if there is no enemy on node or there is no unit on node
	//returns true is there is an enemy unit on node
	public bool isUnitEnemy(Unit unit) { 
		
		if(noUnit || unitOnTile.sameTeam(unit))
			return false;
		return true;
	}

	public string toString() {
		return "(" + x + ", " + y + ")";
	}
}