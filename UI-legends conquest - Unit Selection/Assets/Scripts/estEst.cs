using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class estEst : MonoBehaviour
{
    
    public static int numOfPlayers;
    public static int infantryP1;
    public static int archerP1;
    public static int cavalryP1;
    public static int infantryP2;
    public static int archerP2;
    public static int cavalryP2;
    public static int infantryP3;
    public static int archerP3;
    public static int cavalryP3;
    public static int infantryP4;
    public static int archerP4;
    public static int cavalryP4;


    // Start is called before the first frame update
    /*   void Start()
       {

       }*/

    public void setGlobals()
    {
        if (updateNum.totalSum3 > 0 || updateNum.totalSum4 > 0)
            numOfPlayers = 4;
        else
            numOfPlayers = 2;

        //infantryP1 = updateNum.count;
        //archerP1 = updateNum.archerCount;
        //cavalryP1 = updateNum.cavCount;

        infantryP1 = updateNum.unitsPlayer1[0];
        archerP1   = updateNum.unitsPlayer1[1];
        cavalryP1  = updateNum.unitsPlayer1[2];

        infantryP2 = updateNum.unitsPlayer2[0];
        archerP2 = updateNum.unitsPlayer2[1];
        cavalryP2 = updateNum.unitsPlayer2[2];




        Debug.Log(" Num of Players:" + numOfPlayers.ToString());
        Debug.Log(" Player 1 - Infantry:"+ infantryP1.ToString());
        Debug.Log(" Player 1 - Archers:" + archerP1.ToString());
        Debug.Log(" Player 1 - Cavalry:" + cavalryP1.ToString());
        Debug.Log(" Player 2 - Infantry:" + infantryP2.ToString());
        Debug.Log(" Player 2 - Archers:" + archerP2.ToString());
        Debug.Log(" Player 2 - Cavalry:" + cavalryP2.ToString());

        if (numOfPlayers == 4)
        {
            infantryP3 = updateNum.unitsPlayer3[0];
            archerP3 = updateNum.unitsPlayer3[1];
            cavalryP3 = updateNum.unitsPlayer3[2];

            infantryP4 = updateNum.unitsPlayer4[0];
            archerP4 = updateNum.unitsPlayer4[1];
            cavalryP4 = updateNum.unitsPlayer4[2];


            Debug.Log(" Player 3 - Infantry:" + infantryP3.ToString());
            Debug.Log(" Player 3 - Archers:" + archerP3.ToString());
            Debug.Log(" Player 3 - Cavalry:" + cavalryP3.ToString());
            Debug.Log(" Player 4 - Infantry:" + infantryP4.ToString());
            Debug.Log(" Player 4 - Archers:" + archerP4.ToString());
            Debug.Log(" Player 4 - Cavalry:" + cavalryP4.ToString());
        }


    }


}
