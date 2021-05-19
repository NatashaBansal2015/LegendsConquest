using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] cells;//map cell

    public GameObject selected;// record which piece has been selected

    private List<GameObject> moveList = new List<GameObject>();//record move range
    private List<GameObject> attackList = new List<GameObject>();// record attack range

    //private List<GameObject> obstacles = new List<GameObject>(); 
    //record obstacles

    public List<GameObject> attackBtn = new List<GameObject>();//all attack options

    public List<Piece> enemyPiece = new List<Piece>();//enemy
    void Start()
    {
        cells = GameObject.FindGameObjectsWithTag("Cell");
        //foreach (var cell in cells)
        //{
        //    if (cell.transform.childCount < 2)
        //    {
        //        if (cell.transform.position.x >= -1.86 && cell.transform.position.x <= 1.86)
        //            obstacles.Add(cell);
        //    }
        //}
    }

    public void ShowMoveRange()
    {
        CloseMoveRange();//clear record
        foreach (var cell in cells)
        {
            float range = selected.GetComponent<Piece>().moveRange;// record current piece's moverange
            if (Mathf.Abs(cell.transform.position.x - selected.transform.position.x) + Mathf.Abs(cell.transform.position.y - selected.transform.position.y) <= range)
            //get all celles' position and check if they are inside the moverange
            {
                if (cell.transform.childCount < 3)
                {
                    cell.GetComponent<Cell>().moveabel = true;//moveable
                    cell.GetComponent<Cell>().moveCell.SetActive(true);//show moverange
                    moveList.Add(cell);//add moveable celles to the list
                }
            }
        }
    }
    public void AttackMoveRange()//same logice as showmoverange
    {
        CloseMoveRange();
        foreach (var cell in cells)
        {
            float range = selected.GetComponent<Piece>().attackRange;
            if (Mathf.Abs(cell.transform.position.x - selected.transform.position.x) + Mathf.Abs(cell.transform.position.y - selected.transform.position.y) <= range)
            {
                if (cell.transform.childCount < 3)
                {
                    cell.GetComponent<Cell>().attackabel = true;
                    cell.GetComponent<Cell>().attackCell.SetActive(true);
                    attackList.Add(cell);
                }
            }
        }
    }

    public void CloseMoveRange()//hide range display, clean up records
    {
        foreach (var cell in moveList)
        {
            cell.GetComponent<Cell>().moveabel = false;
            cell.GetComponent<Cell>().moveCell.SetActive(false);
        }
        foreach (var cell in attackList)
        {
            cell.GetComponent<Cell>().attackabel = false;
            cell.GetComponent<Cell>().attackCell.SetActive(false);
        }
        moveList.Clear();
        attackList.Clear();
    }

}
