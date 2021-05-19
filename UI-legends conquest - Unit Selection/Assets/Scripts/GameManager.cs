using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//LMAO

//manages information on players, the map, and units
//also calcualtes information for classes stated above (ex what tiles the selected unit can move too)
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab; //map prefab
    [SerializeField] private GameObject[] unitPrefabs;
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private GameObject reticle;

    private GameObject map; //will control the map info
    //private GameObject reticle;
    private List<Player> players = new List<Player>(); //our list of players
    private List<Player> team1 = new List<Player>(); //list of players in team 1
    private List<Player> team2 = new List<Player>(); //list of players in team 2

    //info about our selected unit and its surroundings
    private Unit selected;// record which piece has been selected
    private List<Node> moveList = new List<Node>(); //records the possible tiles a unit can move too
    private List<Node> attackList = new List<Node>(); // The list of enemy units in the selected unit's atk range
    private List<Node> startingPositions = new List<Node>();

    private int turn = -1; //-1 because when the game begins the turn counter goes to 0 and describes which players turn it is, NOT the overall turn count
    public Text TextField;
    private string victoryMessage = "no one won";

    // Start is called before the first frame update
    void Start() {
        map = (GameObject)Instantiate(mapPrefab, new Vector3(0, 0, 0), Quaternion.identity); //initalizes the map
        map.GetComponent<TileMap>().InitializeMap();
        reticle.GetComponent<Reticle>().initialize(this, getNode(1,0));

        Debug.Log(" NEW number of players: " + PlayerData.numOfPlayers);

        //if we have 1v1
        if (PlayerData.numOfPlayers == 2) {
            generateStartingNode1v1();
            generate1v1();
        }

        // if we have 2v2
        else if (PlayerData.numOfPlayers == 4) {
            generateStartingNode2v2();
            generate2v2();
        }

        startTurn();
    }

    // Update is called once per frame
    void Update() {

        if (checkWinner())
        { //if we have a winning team
            PlayerData.setVicoryMessage(victoryMessage);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    //gets called to initialize a new turn
    public void startTurn() {

        //so we dont try and access a player that does not exist
        if (turn > -1) {
            players[turn].revokePlayersTurn();
        }

        selectUnitHandler(null);

        turn = (turn + 1) % players.Count;
        TextField.text = players[turn].getName() + "'s turn";
        players[turn].upkeep();
    }

    //exits for abstation purposes
    public Node getNode(int x, int y) {
        return map.GetComponent<TileMap>().getGraphNode(x,y);
    }

    //sets selected unit and will perform various actions like displaying movement possibilites and shit
    private void selectUnitHandler(Unit newUnit) {

        if (newUnit == null || newUnit.getPlayer().isPlayerActive())
            selected = newUnit;
        else
            return;

        //if there is a list of tiles that have its space displayed as movable, remove it before setting a new list of tiles
        if (moveList.Count > 0) { 
            map.GetComponent<TileMap>().hideMovableTiles(moveList);
            map.GetComponent<TileMap>().hideAtkTiles(attackList);
        }
            

        //happens when a new turn starts
        if (selected == null) {
            moveList.Clear();
            attackList.Clear();
        }

        else {
            //gets new list of tiles unit can move to and displays it

            if (selected.getUnitClass().GetComponent<UnitClass>().getCurrentStam() > 0)
            {
                generatePossibleTiles();
                generateListOfAttackableEnemies();
                map.GetComponent<TileMap>().showMovableTiles(moveList);
            }
        }
    }

    //Generates a list of possible nodes a unit can move too using Dijkstra's algorithm
    private void generatePossibleTiles () {

        //clears out the old set of possible legal moves
        moveList.Clear();

        //set up something to keep track of the distance from our starting node and the last visited node
        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        //set up our frontier 
        List<Node> frontier = new List<Node>();

        Node originalPosition = map.GetComponent<TileMap>().getGraphNode(selected.getPosX()
                                                                       , selected.getPosY());

        distance[originalPosition] = 0;
        previous[originalPosition] = null;

        //start by initializing all distances to infinity
        foreach (Node v in map.GetComponent<TileMap>().getGraph()) {
            if (v != originalPosition) {
                distance[v] = Mathf.Infinity;
                previous[v] = null;
            }
            frontier.Add(v);
        }

        while (frontier.Count > 0) {

            //↓IMPLEMENT A PRIORITY QUEUE LATER FOR FRONTIER ketp removable lines together↓
            Node currentNode = null;//current node with smallest distance
            foreach(Node possibleNode in frontier) {
				if(currentNode == null || distance[possibleNode] < distance[currentNode]) {
                    currentNode = possibleNode;
				}
			}
            //↑IMPLEMENT A PRIORITY QUEUE LATER FOR FRONTIER ketp removable lines together↑

            frontier.Remove(currentNode); //remove current node from frontier
            
            //checks to see if we found an alternate (faster) route to an adjacent node
            foreach (Node v in currentNode.getAdjacent()) {
                float alternate = distance[currentNode] 
                                  + map.GetComponent<TileMap>().UnitCanWalkThroughTile(v.getX(), v.getY(), selected);

                if (alternate < distance[v]) {
                    distance[v] = alternate;
                    previous[v] = currentNode;
                }
            }
        }

        //adds each node to the list that our selected unit can move too
        for (int i = 0; i < distance.Count; i++) {

            Node check = distance.ElementAt(i).Key;
            if (distance[check] <= selected.getUnitClass().GetComponent<UnitClass>().getCurrentStam() && check != originalPosition && check.isEmpty()) //checks if unit can move to that tile
                moveList.Add(check);

            if (distance[check] == selected.getUnitClass().GetComponent<UnitClass>().getCurrentRange() && !check.isEmpty() && !selected.sameTeam(check.getUnit())) //checks if there is an enemy unit can attack
                attackList.Add(check);
        }
    }

    //generates a path for the unit to walk through for animation purposes. Uses A*
    //A* algorithm gotten from wikipedia
    private List<Node> generatePath (Node target) {
        Node start = selected.getNode();
        List<Node> frontier = new List<Node>();
        frontier.Add(start);

        Dictionary<Node, float> distance = new Dictionary<Node, float>(); //distance from this node and the starting node
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>(); //the previous node in the quickest path from the starting node
        Dictionary<Node, float> frontierScore = new Dictionary<Node, float>(); //records the score of each node

        //Initializes each distance
        foreach (Node n in map.GetComponent<TileMap>().getGraph()) {
            distance[n] = Mathf.Infinity;
            frontierScore[n] = Mathf.Infinity;
            previous[n] = null;
        }
        distance[start] = 0;
        frontierScore[start] = 0;

        //loop to fine the optimal path
        while (frontier.Count > 0) {

            //↓IMPLEMENT A PRIORITY QUEUE LATER FOR FRONTIER ketp removable lines together↓
            Node currentNode = null;//current node with smallest distance
            foreach(Node possibleNode in frontier) {
				if(currentNode == null || frontierScore[possibleNode] < frontierScore[currentNode]) {
                    currentNode = possibleNode;
				}
			}
            //↑IMPLEMENT A PRIORITY QUEUE LATER FOR FRONTIER ketp removable lines together↑

            if (currentNode == target)
                return reconstructPath(previous, target);

            frontier.Remove(currentNode);

            //checks adjacent Nodes if we found a faster route
            foreach (Node a in currentNode.getAdjacent()) {
                float possibleDistance = distance[currentNode] + 
                                         map.GetComponent<TileMap>().UnitCanWalkThroughTile(a.getX(), a.getY(), selected);
                // if we found a faster route
                if(possibleDistance < distance[a]) {
                    previous[a] = currentNode;
                    distance[a] = possibleDistance;
                    frontierScore[a] = distance[a] + heuristic(a, target);
                    if (!frontier.Contains(a)) //if we found a shorter route add it to the frontier
                        frontier.Add(a);
                }
            }
        }

        return reconstructPath(previous, target);
    }

    //heuristic is calculated by [(difference in x) + (difference in y)]
    private int heuristic(Node current, Node target) {
        return Math.Abs(current.getX() - target.getX()) + Math.Abs(current.getY() - target.getY());
    }

    //reconstructs the optimal path using info given from A*
    private List<Node> reconstructPath(Dictionary<Node, Node> previous, Node target) {

        Node current = target;
        List<Node> currentPath = new List<Node>();
        while (current != null) {
            currentPath.Add(current);
            current = previous[current];
        }

        currentPath.Reverse();
        return currentPath;
    }

    //moves unit to a new tile and updates the visuals to reflect that
    private void moveSelected(List<Node> path) {

        map.GetComponent<TileMap>().hideMovableTiles(moveList); //hides the old movement range of the unit prior to movement
        selected.getUnitClass().GetComponent<UnitClass>().setPathReduceStaminaAndRelocateUnit(path); // moves unit

        //gets new list of tiles unit can move to and displays it
        generatePossibleTiles();
        generateListOfAttackableEnemies();
        map.GetComponent<TileMap>().showMovableTiles(moveList);

        knockOutPlayer(); //since we moved if player captured a base node it will knock them out
    }

    //generates a list of nodes containing enemies that our unit can attack
    private void generateListOfAttackableEnemies() {
        
        //clears out the old set of possible legal moves
        attackList.Clear();

        //set up something to keep track of the distance from our starting node and the last visited node
        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        //set up our frontier 
        List<Node> frontier = new List<Node>();

        Node originalPosition = map.GetComponent<TileMap>().getGraphNode(selected.getPosX()
                                                                       , selected.getPosY());

        distance[originalPosition] = 0;
        previous[originalPosition] = null;

        //start by initializing all distances to infinity
        foreach (Node v in map.GetComponent<TileMap>().getGraph()) {
            if (v != originalPosition) {
                distance[v] = Mathf.Infinity;
                previous[v] = null;
            }
            frontier.Add(v);
        }

        while (frontier.Count > 0) {

            //↓IMPLEMENT A PRIORITY QUEUE LATER FOR FRONTIER ketp removable lines together↓
            Node currentNode = null;//current node with smallest distance
            foreach(Node possibleNode in frontier) {
				if(currentNode == null || distance[possibleNode] < distance[currentNode]) {
                    currentNode = possibleNode;
				}
			}
            //↑IMPLEMENT A PRIORITY QUEUE LATER FOR FRONTIER ketp removable lines together↑

            frontier.Remove(currentNode); //remove current node from frontier
            
            //checks to see if we found an alternate (faster) route to an adjacent node
            foreach (Node v in currentNode.getAdjacent()) {
                float alternate = distance[currentNode] + 1;
                                  
                if (alternate < distance[v]) {
                    distance[v] = alternate;
                    previous[v] = currentNode;
                }
            }
        }

        //adds each node to the list that has an enemy unit at the attack range
        for (int i = 0; i < distance.Count; i++) {

            Node check = distance.ElementAt(i).Key;
            if (distance[check] == selected.getUnitClass().GetComponent<UnitClass>().getCurrentRange() && !check.isEmpty() && !selected.sameTeam(check.getUnit()))
                attackList.Add(check);
        }
    }

    //hides movement tiles and shows attack tiles
    private void showAttackableEnemies() {
        map.GetComponent<TileMap>().hideMovableTiles(moveList); //hide our movement tiles
        map.GetComponent<TileMap>().showAtkTiles(attackList); //show our attack tiles
    }

    //if we decide to no attack hides attack tiles and shows movement tiles
    private void backOutCombatSelection() {
        map.GetComponent<TileMap>().hideAtkTiles(attackList); //show our attack tiles
        map.GetComponent<TileMap>().showMovableTiles(moveList); //hide our movement tiles
    }

    //selected unit fights another unit
    private void combat(Node enemyUnit) {

        //checks to see if node with the enemy is a viable target
        if(selected.getUnitClass().GetComponent<UnitClass>().getName() != "Soldier" && attackList.Contains(enemyUnit) && selected.getUnitClass().GetComponent<UnitClass>().getCurrentStam() > 0 && selected.getPlayer().getTeam() != enemyUnit.getUnit().getPlayer().getTeam()) {

            enemyUnit.getUnit().getUnitClass().GetComponent<UnitClass>().takeDamage(selected);
            selected.getUnitClass().GetComponent<UnitClass>().reduceStamina();

            //if unit loses all HP
            if (enemyUnit.getUnit().getUnitClass().GetComponent<UnitClass>().getCurrentHP() <= 0) {
                
                enemyUnit.getUnit().removeUnit();
                knockOutPlayer(); //since a unit died we need to check if that player got knocked out
            }

            //once we reduce stamina we check new movement options and possibly enemy unit (none)
            map.GetComponent<TileMap>().hideAtkTiles(attackList);
            attackList.Clear(); //since we have no stamina to attack with we can just empty this out
            moveList.Clear(); //since we have no stamina to move with we can just empty this out
        }

        //soldier cant attack if they used a special
        //else if (selected.getUnitClass().GetComponent<UnitClass>().getNoSpecialUsed() && selected.getPlayer().getTeam() != enemyUnit.getUnit().getPlayer().getTeam()) {

        else if (selected.getUnitClass().GetComponent<UnitClass>().getName() == "Soldier" && selected.getUnitClass().GetComponent<UnitClass>().getCurrentStam() > 0 && selected.getUnitClass().GetComponent<UnitClass>().getNoSpecialUsed() && selected.getPlayer().getTeam() != enemyUnit.getUnit().getPlayer().getTeam())
        {
            enemyUnit.getUnit().getUnitClass().GetComponent<UnitClass>().takeDamage(selected);
            selected.getUnitClass().GetComponent<UnitClass>().reduceStamina();

            //if unit loses all HP
            if (enemyUnit.getUnit().getUnitClass().GetComponent<UnitClass>().getCurrentHP() <= 0) {
                
                enemyUnit.getUnit().removeUnit();
                knockOutPlayer(); //since a unit died we need to check if that player got knocked out
            }

            //once we reduce stamina we check new movement options and possibly enemy unit (none)
            map.GetComponent<TileMap>().hideAtkTiles(attackList);
            attackList.Clear(); //since we have no stamina to attack with we can just empty this out
            moveList.Clear(); //since we have no stamina to move with we can just empty this out
        }

    }

    //checks is a win condition has been met
    //when a player is knocked out they will be removed from the team therefore no matter how that player loses the team loses when there are no players
    private bool checkWinner() {

        //if team 1 wins
        if(team2.Count == 0) {
            victoryMessage = "Team 1 wins"; //sets the victory message to play
            //Debug.Log(victoryMessage);
            return true;
        }

        //if team 2 wins
        else if(team1.Count == 0) {
            victoryMessage = "Team 2 wins"; //sets the victory message to play
            //Debug.Log(victoryMessage);
            return true;
        }

        //Debug.Log(victoryMessage);

        return false;
    }

    //knocks out a player if a lose condition is met
    private void knockOutPlayer() {

        Player needToBeDeleted = null;
        foreach(Player p in players) { 

            //if a player has their army wiped out or if they lose their commander
            if (p.getArmy().Count == 1 || !p.isThereCommander() || p.hasBaseCaptured()) {
                needToBeDeleted = p;
            }
        }

        //if we found a player who got knocked out
        //remove player from thier respective team
        if (needToBeDeleted != null && players.IndexOf(needToBeDeleted) < turn) //if the player we r deleting has their turn before our current player adjust turn to not skip the next players turn
            turn--;

        if (!(needToBeDeleted == null)) {

            if (needToBeDeleted.getTeam() == 1) // if in team 1
                team1.Remove(needToBeDeleted);

            else if (needToBeDeleted.getTeam() == 2) //if team 2
                team2.Remove(needToBeDeleted);

            players.Remove(needToBeDeleted);
            needToBeDeleted.deletePlayer();

            checkWinner();// check if someone won the game
            Debug.Log(checkWinner());
        }
    }

    //getter for the map
    public GameObject getMap() {
        return map;
    }

/*________________________________________________________________BUTTONS________________________________________________________________________________________________________________________*/

    //when a button is hit to select a unit
    public void selectUnit() {
        selectUnitHandler(reticle.GetComponent<Reticle>().pointing.getUnit());
        Debug.Log(selected.toString());
        Debug.Log(selected.getUnitClass().GetComponent<UnitClass>().getCurrentAttack());
        Debug.Log(selected.getUnitClass().GetComponent<UnitClass>().getCurrentDefence());
    }

    //when the player no longer wants to select a unit
    public void deselectUnit() {
        selectUnitHandler(null);
    }

    //when button is hit to move, moves unit to desired tile if it is in range
    public void move() {
        if (selected != null && moveList.Contains(reticle.GetComponent<Reticle>().pointing)) {
            moveSelected(generatePath(reticle.GetComponent<Reticle>().pointing));
            selectUnitHandler(null);
        }
    }

    //shows attackable enemies
    public void showAttack() {
        showAttackableEnemies();
    }
    
    //uses the units special
    public void special() {
        selected.getUnitClass().GetComponent<UnitClass>().special();
        selectUnitHandler(null);
    }

    //attacks a unit with currently selected unit
    public void attack() {
        combat(reticle.GetComponent<Reticle>().pointing);
        map.GetComponent<TileMap>().hideAtkTiles(attackList);
        selectUnitHandler(null);
    }

    //to back out of combat options
    public void back() {
        map.GetComponent<TileMap>().hideAtkTiles(attackList); //hide our movement tiles
        map.GetComponent<TileMap>().showMovableTiles(moveList); //show our attack tiles
    }




    /*__________________________________________Generating Game Modes________________________________________________________________________________________________________________*/

    //generates a game of 1v1
    private void generate1v1() {
        Player tempPlayer;
        List<Unit> newArmy = new List<Unit>();

        //make player 1
        tempPlayer = new Player(this, "P1", 1);
        players.Add(tempPlayer);
        tempPlayer.setBase(startingPositions[0]); // sets base node to player 1
        newArmy.Add(generateUnit(0, (int)unitClass.Commander, (int)PlayerColour.P1));//generate commander
        startingPositions.RemoveAt(0); // remove node from out list
        
        //generates infantry for P1
        for (int i = 0; i < PlayerData.infantryP1; i++) {
            newArmy.Add(generateUnit(0, (int)unitClass.Soldier, (int)PlayerColour.P1));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Cavalry for P1
        for (int i = 0; i < PlayerData.cavalryP1; i++) {
            newArmy.Add(generateUnit(0, (int)unitClass.Cavalier, (int)PlayerColour.P1));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Archer for P1
        for (int i = 0; i < PlayerData.archerP1; i++) {
            newArmy.Add(generateUnit(0, (int)unitClass.Archer, (int)PlayerColour.P1));
            startingPositions.RemoveAt(0); // remove node from out list
        }
        players[0].setArmy(newArmy);
        team1.Add(players[0]);

        //make player 2
        //will use the player 3 icon to keep the colour coding for bases
        tempPlayer = new Player(this, "P2", 2);
        players.Add(tempPlayer);
        newArmy = new List<Unit>();
        tempPlayer.setBase(startingPositions[0]); // sets base node to player 2
        newArmy.Add(generateUnit(1, (int)unitClass.Commander, (int)PlayerColour.P3));//generate commander
        startingPositions.RemoveAt(0); // remove node from out list
        
        //generates infantry for P2
        for (int i = 0; i < PlayerData.infantryP2; i++) {
            newArmy.Add(generateUnit(1, (int)unitClass.Soldier, (int)PlayerColour.P3));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Cavalry for P2
        for (int i = 0; i < PlayerData.cavalryP2; i++) {
            newArmy.Add(generateUnit(1, (int)unitClass.Cavalier, (int)PlayerColour.P3));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Archer for P2
        for (int i = 0; i < PlayerData.archerP2; i++) {
            newArmy.Add(generateUnit(1, (int)unitClass.Archer, (int)PlayerColour.P3));
            startingPositions.RemoveAt(0); // remove node from out list
        }
        players[1].setArmy(newArmy);
        team2.Add(players[1]);
    }

    //generates a game of 2v2
    private void generate2v2() {
        Player tempPlayer;
        List<Unit> newArmy = new List<Unit>();

        //make player 1
        tempPlayer = new Player(this, "P1", 1);
        players.Add(tempPlayer);
        tempPlayer.setBase(startingPositions[0]); // sets base node to player 1
        newArmy.Add(generateUnit(0, (int)unitClass.Commander, (int)PlayerColour.P1));//generate commander
        startingPositions.RemoveAt(0); // remove node from out list
        
        //generates infantry for P1
        for (int i = 0; i < PlayerData.infantryP1; i++) {
            newArmy.Add(generateUnit(0, (int)unitClass.Soldier, (int)PlayerColour.P1));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Cavalry for P1
        for (int i = 0; i < PlayerData.cavalryP1; i++) {
            newArmy.Add(generateUnit(0, (int)unitClass.Cavalier, (int)PlayerColour.P1));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Archer for P1
        for (int i = 0; i < PlayerData.archerP1; i++) {
            newArmy.Add(generateUnit(0, (int)unitClass.Archer, (int)PlayerColour.P1));
            startingPositions.RemoveAt(0); // remove node from out list
        }
        players[0].setArmy(newArmy);
        team1.Add(players[0]);

        /*_________________________P2______________________________*/

        //make player 2
        tempPlayer = new Player(this, "P2", 1);
        players.Add(tempPlayer);
        newArmy = new List<Unit>();
        tempPlayer.setBase(startingPositions[0]); // sets base node to player 2
        newArmy.Add(generateUnit(1, (int)unitClass.Commander, (int)PlayerColour.P2));//generate commander
        startingPositions.RemoveAt(0); // remove node from out list
        
        //generates infantry for P2
        for (int i = 0; i < PlayerData.infantryP2; i++) {
            newArmy.Add(generateUnit(1, (int)unitClass.Soldier, (int)PlayerColour.P2));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Cavalry for P2
        for (int i = 0; i < PlayerData.cavalryP2; i++) {
            newArmy.Add(generateUnit(1, (int)unitClass.Cavalier, (int) PlayerColour.P2));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Archer for P2
        for (int i = 0; i < PlayerData.archerP2; i++) {
            newArmy.Add(generateUnit(1, (int)unitClass.Archer, (int)PlayerColour.P2));
            startingPositions.RemoveAt(0); // remove node from out list
        }
        players[1].setArmy(newArmy);
        team1.Add(players[1]);

        /*_________________________P3______________________________*/

        //make player 3
        tempPlayer = new Player(this, "P3", 2);
        players.Add(tempPlayer);
        newArmy = new List<Unit>();
        tempPlayer.setBase(startingPositions[0]); // sets base node to player 3
        newArmy.Add(generateUnit(2, (int)unitClass.Commander, (int)PlayerColour.P3));//generate commander
        startingPositions.RemoveAt(0); // remove node from out list
        
        //generates infantry for P3
        for (int i = 0; i < PlayerData.infantryP3; i++) {
            newArmy.Add(generateUnit(2, (int)unitClass.Soldier, (int)PlayerColour.P3));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Cavalry for P3
        for (int i = 0; i < PlayerData.cavalryP3; i++) {
            newArmy.Add(generateUnit(2, (int)unitClass.Cavalier, (int)PlayerColour.P3));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Archer for P3
        for (int i = 0; i < PlayerData.archerP3; i++) {
            newArmy.Add(generateUnit(2, (int)unitClass.Archer, (int)PlayerColour.P3));
            startingPositions.RemoveAt(0); // remove node from out list
        }
        players[2].setArmy(newArmy);
        team2.Add(players[2]);

        /*_________________________P4______________________________*/

        //make player 2
        tempPlayer = new Player(this, "P4", 2);
        players.Add(tempPlayer);
        newArmy = new List<Unit>();
        tempPlayer.setBase(startingPositions[0]); // sets base node to player 4
        newArmy.Add(generateUnit(3, (int)unitClass.Commander, (int)PlayerColour.P4));//generate commander
        startingPositions.RemoveAt(0); // remove node from out list
        
        //generates infantry for P4
        for (int i = 0; i < PlayerData.infantryP4; i++) {
            newArmy.Add(generateUnit(3, (int)unitClass.Soldier, (int)PlayerColour.P4));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Cavalry for P4
        for (int i = 0; i < PlayerData.cavalryP4; i++) {
            newArmy.Add(generateUnit(3, (int)unitClass.Cavalier, (int)PlayerColour.P4));
            startingPositions.RemoveAt(0); // remove node from out list
        }

        //generates Archer for P4
        for (int i = 0; i < PlayerData.archerP4; i++) {
            newArmy.Add(generateUnit(3, (int)unitClass.Archer, (int)PlayerColour.P4));
            startingPositions.RemoveAt(0); // remove node from out list
        }
        players[3].setArmy(newArmy);
        team2.Add(players[3]);
    }

    //generates a list of starting nodes for the players
    private void generateStartingNode1v1() {

        //starting positions for P1 units
        startingPositions.Add(getNode(0, 0)); //commander's starting position
        startingPositions.Add(getNode(1, 2));
        startingPositions.Add(getNode(2, 2));
        startingPositions.Add(getNode(4, 1));

        //starting positions for P2 units
        startingPositions.Add(getNode(map.GetComponent<TileMap>().getMapWidth() - 1, map.GetComponent<TileMap>().getMapHeight() - 1)); //commander's starting position
        startingPositions.Add(getNode(13, 7));
        startingPositions.Add(getNode(14, 7));
        startingPositions.Add(getNode(11, 8));
    }

    //generates a list of starting nodes for the players
    private void generateStartingNode2v2() {

        //starting positions for P1 units
        startingPositions.Add(getNode(0, 0)); //commander's starting position
        startingPositions.Add(getNode(1, 2));
        startingPositions.Add(getNode(2, 2));
        startingPositions.Add(getNode(4, 1));

        //starting positions for P2 units
        startingPositions.Add(getNode(0, map.GetComponent<TileMap>().getMapHeight() - 1)); //commander's starting position
        startingPositions.Add(getNode(1, 7));
        startingPositions.Add(getNode(2, 7));
        startingPositions.Add(getNode(4, 8));

        //starting positions for P3 units
        startingPositions.Add(getNode(map.GetComponent<TileMap>().getMapWidth() - 1, map.GetComponent<TileMap>().getMapHeight() - 1)); //commander's starting position
        startingPositions.Add(getNode(13, 7));
        startingPositions.Add(getNode(14, 7));
        startingPositions.Add(getNode(11, 8));

        //starting positions for P4 units
        startingPositions.Add(getNode(map.GetComponent<TileMap>().getMapWidth() - 1, 0)); //commander's starting position
        startingPositions.Add(getNode(13, 2));
        startingPositions.Add(getNode(14, 2));
        startingPositions.Add(getNode(11, 1));
    }

    //generates a Unit
    private Unit generateUnit(int playerID, int unitClassID, int playerIcon) {
        Unit generatedUnit;
        GameObject newUnitPrefab;
        GameObject playerIconPrefab;

        newUnitPrefab = (GameObject)Instantiate(unitPrefabs[unitClassID], new Vector3(startingPositions[0].getX(), startingPositions[0].getY(), 0), Quaternion.identity);
        playerIconPrefab = (GameObject)Instantiate(playerPrefabs[playerIcon], new Vector3(startingPositions[0].getX(), startingPositions[0].getY(), 0), Quaternion.identity); //makes and player icon
        playerIconPrefab.transform.parent = newUnitPrefab.transform; //attaches player icon to unit
        generatedUnit = new Unit(players[playerID], newUnitPrefab, map.GetComponent<TileMap>().getGraphNode(startingPositions[0].getX(), startingPositions[0].getY())); //makes the unit
        map.GetComponent<TileMap>().getGraphNode(startingPositions[0].getX(), startingPositions[0].getY()).setUnit(generatedUnit); //tells the node that this unit is on there
        generatedUnit.getUnitClass().GetComponent<UnitClass>().initialize(generatedUnit); //initializes the unit class values and links unit info with its class info

        return generatedUnit;
    }

/*___________________________________________________ENUMS__________________________________________________________*/

    //an enum for unit classes
    private enum unitClass {
        Commander = 0,
        Soldier = 1,
        Cavalier = 2,
        Archer = 3
    }

    //an enum for unit classes
    private enum PlayerColour {
        P1 = 0,
        P2 = 1,
        P3 = 2,
        P4 = 3
    }
}