using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour
{
    public void plusOne()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void minusOne()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }

    public void plusTwo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void plusThree()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void plusThreeCheck1()
    {
        if ((updateNum.totalSum1 < 3) || (updateNum.totalSum2 < 3))
        {
            Debug.Log("Select 3!!!");
        }
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void plusThreeCheck2()
    {
        if ((updateNum.totalSum1 < 3) || (updateNum.totalSum2 < 3))
        {
            Debug.Log("Select 3!!!");
        }
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void plusThreeCheck3()
    {
        if ( (updateNum.totalSum3 < 3) || (updateNum.totalSum4 < 3))
        {
            Debug.Log("Select 3!!!");
        }
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void minusTwo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);

    }

    public void minusThree()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);

    }
    public void minusFour()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);

    }

    public void minusFive()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 5);

    }

    public void getLink()
    {
        Application.OpenURL("https://gunpreet-dhillon.com/Tutorial.html");
    }

}