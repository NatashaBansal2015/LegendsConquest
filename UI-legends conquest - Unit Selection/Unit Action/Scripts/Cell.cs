using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool moveabel;//If unit can move
    public GameObject moveCell;
    public GameObject attackCell;
    public bool attackabel;//If unit can attack


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// we can add movement part here
    /// </summary>
    public void OnMouseDown()
    {
        if (moveabel)
        {
        }
    }
}