using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{

    private GameManager gm;
    public Node pointing;

    //initializes to hook up with GM
    public void initialize(GameManager gm, Node startingNode) {
        this.gm = gm;
        pointing = startingNode;
        transform.position = pointing.getWorldCoordinates();

        Debug.Log(gm);
        Debug.Log(pointing.toString());
    }

    //moves reticle up
    public void moveUp() {
        if (gm.getMap().GetComponent<TileMap>().getMapHeight() - 1 > pointing.getY()) {
            pointing = gm.getNode(pointing.getX(), pointing.getY() + 1);
            transform.position = pointing.getWorldCoordinates();

            Debug.Log("pointer at" + pointing.toString());
        }
    }

    //moves reticle down
    public void moveDown() {
        if (0 < pointing.getY()) {
            pointing = gm.getNode(pointing.getX(), pointing.getY() - 1);
            transform.position = pointing.getWorldCoordinates();

            Debug.Log("pointer at" + pointing.toString());
        }
    }

    //moves reticle right
    public void moveRight() {
        if (gm.getMap().GetComponent<TileMap>().getMapWidth() - 1 > pointing.getX()) {
            pointing = gm.getNode(pointing.getX() + 1, pointing.getY());
            transform.position = pointing.getWorldCoordinates();

            Debug.Log("pointer at" + pointing.toString());
        }
    }

    //moves reticle left
    public void moveLeft() {
        if (0 < pointing.getX()) {
            pointing = gm.getNode(pointing.getX() - 1, pointing.getY());
            transform.position = pointing.getWorldCoordinates();

            Debug.Log("pointer at" + pointing.toString());
        }
    }

    //grabs unit from node and give it to the game manager to select
    public Unit selectUnit() {
        return pointing.getUnit();
    }
}
