using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : MonoBehaviour {

    private int xCoord;
    private int yCoord;
    private TileMap map;


    void OnMouseUp() {
        Debug.Log("clicked (" + xCoord + ", " + yCoord + ")");

        //map.MoveUnit(xCoord, yCoord);
    }

    //Sets where the collider is because we cant use a constructor with GetComponent<SelectableTile>
    public void setTile (int x, int y, TileMap map) {
        xCoord = x;
        yCoord = y;
        this.map = map;
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
