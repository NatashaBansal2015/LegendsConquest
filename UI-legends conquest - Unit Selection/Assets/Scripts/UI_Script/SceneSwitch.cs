using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void playgame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    public void Back()
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
             
            }

    
    public void credits(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void backTwo()
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);
             
            }

    public void backThree()
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -3);
             
            }

        
}