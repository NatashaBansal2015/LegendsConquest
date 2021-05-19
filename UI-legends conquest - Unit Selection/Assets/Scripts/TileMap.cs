using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* JUST TRYING TO MAKE THINGS WORK
 * WILL ORGINAIZE LATER BY CLEANING UP CODE AND SPLITING IT INTO A SEPERATE CLASS (POSSIBLY GameLogic)
 * 
 * GOING TO MAKE PLAYER/UNITS THEY LINK UP THE MAP WITH THEM
 * THEN WILL FIX THE INITIALIZATION, MOVEMENT, ETC 
 * 
 * EVENTUALLY MAKE A CONSTRUCT AFTER WE KNOW HOW WE ORGANIZE THINGS AND WE KNOW WHAT INFO MAP NEEDS RIGHT AWAY
 *
 *
 * NOTE: anything in all caps is a note to change something later, while everything else is an explaination on code and shit
*/


//This is the map of the game
public class TileMap : MonoBehaviour {

    //Info about the map's tiles
    [SerializeField] private TileType[] tileTypes;
    [SerializeField] private GameObject[] selectedVisualPrefab;
    private int[,] tiles;
    private Node[,] graph;
    private GameObject[,] visualForMovement; //to turn on and off a visual for where a unit can move too
    private GameObject[,] visualForAttackRange; //to turn on and off a visual for who a unit can attack


    //Info about map's size
    private const int MAP_WIDTH = 16;
    private const int MAP_HEIGHT = 10;

    // Start is called before the first frame update
    void Start() {
    }

    //generates all info about the map
    //needs to happen during game logic's start method and not this ones
    public void InitializeMap() {

        //makes the object kinda transparent
        selectedVisualPrefab[(int)VisualTiles.Move].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        selectedVisualPrefab[(int)VisualTiles.Atk].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

        //generate info about the map itself
        GenerateMap();
        GenerateGraphOfMap();
        GenerateMapVisual();
    }

    //Generates data about our map's tiles 
    private void GenerateMap() {
        
        //set our map size 
        tiles = new int[MAP_WIDTH, MAP_HEIGHT];
        
        //initialized everything as grass
        for (int x = 0; x < MAP_WIDTH; x++) {
            for (int y = 0; y < MAP_HEIGHT; y++) {
                tiles[x, y] = (int)Tiles.Grass;
            }
        }

        //fortress 1
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 5; j++) { 
                tiles[j, i] = (int)Tiles.FortressFloor;
            }
        }
        for (int i = 0; i < 5; i++)
            if (i == 0 || i > 2)
                tiles[i, 2] = (int)Tiles.BrickWall;
        tiles[4, 0] = (int)Tiles.BrickWall;


        //fortress 2
        for (int i = 7; i < MAP_HEIGHT; i++) {
            for (int j = 0; j < 5; j++) { 
                tiles[j, i] = (int)Tiles.FortressFloor;
            }
        }
        for (int i = 0; i < 5; i++)
            if (i == 0 || i > 2)
                tiles[i, 7] = (int)Tiles.BrickWall;
        tiles[4, 9] = (int)Tiles.BrickWall;

        //fortress 3
        for (int i = 7; i < MAP_HEIGHT; i++) {
            for (int j = MAP_WIDTH -1 ; j > 10; j--) { 
                tiles[j, i] = (int)Tiles.FortressFloor;
            }
        }
        for (int i = 11; i < MAP_WIDTH; i++)
            if (i == 15 || i < 13)
                tiles[i, 7] = (int)Tiles.BrickWall;
        tiles[11, 9] = (int)Tiles.BrickWall;

        //fortress 4
        for (int i = 0; i < 3; i++) {
            for (int j = MAP_WIDTH - 1; j > 10; j--) { 
                tiles[j, i] = (int)Tiles.FortressFloor;
            }
        }
        for (int i = 11; i < MAP_WIDTH; i++)
            if (i == 15 || i < 13)
                tiles[i, 2] = (int)Tiles.BrickWall;
        tiles[11, 0] = (int)Tiles.BrickWall;


        //places the bases
        tiles[0, 0] = (int)Tiles.BaseTile1;
        tiles[0, (MAP_HEIGHT - 1)] = (int)Tiles.BaseTile2;
        tiles[(MAP_WIDTH - 1), 0] = (int)Tiles.BaseTile4;
        tiles[(MAP_WIDTH - 1), (MAP_HEIGHT - 1)] = (int)Tiles.BaseTile3;
    }

    //generates a graph reperesnation of the map so the system knows how each tile is connected
    private void GenerateGraphOfMap() {

        //Initialize the graph
        graph = new Node[MAP_WIDTH, MAP_HEIGHT];

        //Map each node to a position in the graph
        for (int x = 0; x < MAP_WIDTH; x++) { 
            for (int y = 0; y < MAP_HEIGHT; y++) {
                graph[x, y] = new Node(x, y);
            }
        }

        //Tells each node who its adjacent to
        for (int x = 0; x < MAP_WIDTH; x++) {
            for (int y = 0; y < MAP_HEIGHT; y++) {
                
                //These set of if statements r to check to see if our current node is a boarder node
                if (x > 0)
                    graph[x, y].getAdjacent().Add(graph[x-1, y]);
                if (x < MAP_WIDTH - 1)
                    graph[x, y].getAdjacent().Add(graph[x+1, y]);
                if (y > 0)
                    graph[x, y].getAdjacent().Add(graph[x, y-1]);
                if (y < MAP_HEIGHT-1)
                    graph[x, y].getAdjacent().Add(graph[x, y+1]);
            }
        }
    }

    //Generates the map in unity base on how we constructed it in GenerateMap()
    private void GenerateMapVisual() {

        GameObject tileObjectVisual, selectedObjectVisual;
        visualForMovement = new GameObject[MAP_WIDTH, MAP_HEIGHT];
        visualForAttackRange = new GameObject[MAP_WIDTH, MAP_HEIGHT];

        for (int x = 0; x < MAP_WIDTH; x++) {
            for (int y = 0; y < MAP_HEIGHT; y++) {
                TileType tileInfo = tileTypes[tiles[x,y]]; 

                //generates the map tiles
                tileObjectVisual = (GameObject)Instantiate(tileInfo.getTilePrefab(), new Vector3(x, y, 0), Quaternion.identity);
                tileObjectVisual.name = "Tile: (" + x + ", " + y + ") Type: " + tileInfo.getName();

                SelectableTile st = tileObjectVisual.GetComponent<SelectableTile>(); //SEE IF THERE IS A BETTER WAY TO DO THIS PART LATER
                st.setTile(x, y, this);

                //generates the invisible movement tiles that can be made visible when we need to display them
                selectedObjectVisual = (GameObject)Instantiate(selectedVisualPrefab[(int)VisualTiles.Move], new Vector3(x, y, 0), Quaternion.identity);
                selectedObjectVisual.name = "Movement visual (" + x + ", " + y + ")";
                visualForMovement[x, y] = selectedObjectVisual;
                visualForMovement[x, y].SetActive(false);

                //generates the invisible atk tiles that can be made visible when we need to display them
                selectedObjectVisual = (GameObject)Instantiate(selectedVisualPrefab[(int)VisualTiles.Atk], new Vector3(x, y, 0), Quaternion.identity);
                selectedObjectVisual.name = "Attack visual (" + x + ", " + y + ")";
                visualForAttackRange[x, y] = selectedObjectVisual;
                visualForAttackRange[x, y].SetActive(false);
            }
        }
    }

    //Makes tiles that selected unit can move to visible
    public void showMovableTiles(List<Node> reachable) {
        
        //makes each tile visable
        foreach(Node n in reachable) {
            visualForMovement[n.getX(), n.getY()].SetActive(true);
        }
    }

    //Makes tiles with enemy units withing our selected unit's attack range visible
    public void showAtkTiles(List<Node> enemyUnits) {
        
        //makes each tile visable
        foreach(Node n in enemyUnits) {
            visualForAttackRange[n.getX(), n.getY()].SetActive(true);
        }
    }

    //Makes tiles that selected unit can move to invisible
    public void hideMovableTiles(List<Node> oldReachable) {
        
        //makes each tile visable
        foreach(Node n in oldReachable) {
            visualForMovement[n.getX(), n.getY()].SetActive(false);
        }
    }

    //Makes tiles with enemy units withing our selected unit's attack range invisible
    public void hideAtkTiles(List<Node> enemyUnits) {
        
        //makes each tile visable
        foreach(Node n in enemyUnits) {
            visualForAttackRange[n.getX(), n.getY()].SetActive(false);
        }
    }

    //returns infinity the unit is unable to walk through tile at position (x,y) otherwise returns 1
    public float UnitCanWalkThroughTile (int x, int y, Unit selectedUnit) {

        //if the tile can have a unit on it
        if (tileTypes[tiles[x, y]].getIsWalkable()) {

            if (graph[x, y].isEmpty()) //if no enemy unit exists on the that tile
                return 1f;

            else if (selectedUnit.sameTeam(graph[x, y].getUnit())) //if the unit is on the same team as the unit on the tile they can pass through
                return 1f;
        }

        return Mathf.Infinity; //if the tile cant have a unit or the tile has an enemy unit
    }

    //getter for a node in the graph
    public Node getGraphNode(int x, int y) {
        return graph[x,y];
    }

    //getter for the graph
    public Node[,] getGraph() {
        return graph;
    }

    //getter for map height
    public int getMapHeight() {
        return MAP_HEIGHT;
    }

    //getter for map width
    public int getMapWidth() {
        return MAP_WIDTH;
    }

    //enum for the index of TileTypes
    enum Tiles {
        Grass = 0,
        FortressFloor = 1,
        BrickWall = 2,
        BaseTile1 = 3,
        BaseTile2 = 4,
        BaseTile3 = 5,
        BaseTile4 = 6
    }

    //enum for index of selectedVisualPrefabs
    enum VisualTiles { 
        Move = 0,
        Atk = 1
    }
}
