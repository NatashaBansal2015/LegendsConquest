using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateNum : MonoBehaviour
{
    public Text TextField;
    public Text Result;
    public static int count = 0;
    public static int archerCount = 0;
    public static int cavCount = 0;
    public static int cap =3;
    public static int totalSum1=0;

    public static int count3 = 0;
    public static int archerCount3 = 0;
    public static int cavCount3 = 0;
    public static int totalSum3 = 0;

    public static int count4 = 0;
    public static int archerCount4 = 0;
    public static int cavCount4 = 0;
    public static int totalSum4 = 0;

    public static int count2 = 0;
    public static int archerCount2 = 0;
    public static int cavCount2 = 0;
    public static int totalSum2 = 0;

    public static int[] unitsPlayer1 = new int[5];
    public static int[] unitsPlayer2 = new int[5];
    public static int[] unitsPlayer3 = new int[5];
    public static int[] unitsPlayer4 = new int[5];

    // Start is called before the first frame update
    void Start()
    {
        Result.text = PlayerData.victoryMessage;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void add()
    {
        if (totalSum1 < cap)
        {
            count++;
            totalSum1++;
        }
        change(count);
    }

    public void remove()
    {
        if (count > 0)
        {
            count--;
            totalSum1--;
        }

        change(count);
    }

    public void addArcher()
    {
        if (totalSum1 < cap)
        {
            archerCount++;
            totalSum1++;
        }
        change(archerCount);
    }

    public void removeArcher()
    {
        if (archerCount > 0)
        {
            archerCount--;
            totalSum1--;
        }
        change(archerCount);
    }

    public void addCav()
    {
        if (totalSum1 < cap)
        {
            cavCount++;
            totalSum1++;
        }
        change(cavCount);
    }

    public void removeCav()
    {
        if (cavCount > 0)
        {
            cavCount--;
            totalSum1--;
        }

        change(cavCount);
    }
    //---------------------------- p2 -------------------------------------------------
    public void add2()
    {
        if (totalSum2 < cap)
        {
            count2++;
            totalSum2++;
        }
        change(count2);
    }

    public void remove2()
    {
        if (count2 > 0)
        {
            count2--;
            totalSum2--;
        }

        change(count2);
    }

    public void addArcher2()
    {
        if (totalSum2 < cap)
        {
            archerCount2++;
            totalSum2++;
        }
        change(archerCount2);
    }

    public void removeArcher2()
    {
        if (archerCount2 > 0)
        {
            archerCount2--;
            totalSum2--;
        }

        change(archerCount2);
    }

    public void addCav2()
    {

        if (totalSum2 < cap)
        {
            cavCount2++;
            totalSum2++;
        }    
        change(cavCount2);
    }

    public void removeCav2()
    {
        if (cavCount2 > 0)
        {
            cavCount2--;
            totalSum2--;
        }

        change(cavCount2);
    }

    // ------------------------------------- p3 -----------------------------------
    public void add3()
    {
        if (totalSum3 < cap)
        {
            count3++;
            totalSum3++;
        }
        change(count3);
    }

    public void remove3()
    {
        if (count3 > 0)
        {
            count3--;
            totalSum3--;
        }

        change(count3);
    }

    public void addArcher3()
    {
        if (totalSum3 < cap)
        {
            archerCount3++;
            totalSum3++;
        }
        change(archerCount3);
    }

    public void removeArcher3()
    {
        if (archerCount3 > 0)
        {
            archerCount3--;
            totalSum3--;
        }

        change(archerCount3);
    }

    public void addCav3()
    {

        if (totalSum3 < cap)
        {
            cavCount3++;
            totalSum3++;
        }
        change(cavCount3);
    }

    public void removeCav3()
    {
        if (cavCount3 > 0)
        {
            cavCount3--;
            totalSum3--;
        }

        change(cavCount3);
    }
    // ------------------------------------- p4 -----------------------------------
    public void add4()
    {
        if (totalSum4 < cap)
        {
            count4++;
            totalSum4++;
        }
        change(count4);
    }

    public void remove4()
    {
        if (count4 > 0)
        {
            count4--;
            totalSum4--;
        }

        change(count4);
    }

    public void addArcher4()
    {
        if (totalSum4 < cap)
        {
            archerCount4++;
            totalSum4++;
        }
        change(archerCount4);
    }

    public void removeArcher4()
    {
        if (archerCount4 > 0)
        {
            archerCount4--;
            totalSum4--;
        }

        change(archerCount4);
    }

    public void addCav4()
    {

        if (totalSum4 < cap)
        {
            cavCount4++;
            totalSum4++;
        }
        change(cavCount4);
    }

    public void removeCav4()
    {
        if (cavCount4 > 0)
        {
            cavCount4--;
            totalSum4--;
        }

        change(cavCount4);
    }
    // ----------------------------------------------------------------------------


    public void change(int num)
    {
        TextField.text = num.ToString();
    }

    public void makeArray()
    {
        unitsPlayer1[0] = count;
        unitsPlayer1[1] = archerCount;
        unitsPlayer1[2] = cavCount;

        unitsPlayer2[0] = count2;
        unitsPlayer2[1] = archerCount2;
        unitsPlayer2[2] = cavCount2;

        PlayerData.setGlobals();
    }

    public void makeArray2()
    {
        unitsPlayer3[0] = count3;
        unitsPlayer3[1] = archerCount3;
        unitsPlayer3[2] = cavCount3;

        unitsPlayer4[0] = count4;
        unitsPlayer4[1] = archerCount4;
        unitsPlayer4[2] = cavCount4;

        PlayerData.setGlobals();

    }

    public void printArrays()
    {
        TextField.text += " Player 1 Units: (";
        for (int i =0; i<=2; i++)
        {
            TextField.text += unitsPlayer1[i].ToString();
            TextField.text += ",";
        }
        TextField.text += ") \n Player 2 Units: (";
        for (int i = 0; i <= 2; i++)
        {
            TextField.text += unitsPlayer2[i].ToString();
            TextField.text += ",";
        }
        TextField.text += ")";


        if (totalSum3 >0 || totalSum4 > 0)
        {
            TextField.text += "\n Player 3 Units: (";
            for (int i = 0; i <= 2; i++)
            {
                TextField.text += unitsPlayer3[i].ToString();
                TextField.text += ",";
            }
            TextField.text += ") \n Player 4 Units: (";
            for (int i = 0; i <= 2; i++)
            {
                TextField.text += unitsPlayer4[i].ToString();
                TextField.text += ",";
            }
            TextField.text += ")";

        }

    }

    public void clearUnits()
    {

    count = 0;
    archerCount = 0;
    cavCount = 0;
    
    totalSum1 = 0;

    count3 = 0;
    archerCount3 = 0;
    cavCount3 = 0;
    totalSum3 = 0;

    count4 = 0;
    archerCount4 = 0;
    cavCount4 = 0;
    totalSum4 = 0;

    count2 = 0;
    archerCount2 = 0;
    cavCount2 = 0;
    totalSum2 = 0;

}


}
