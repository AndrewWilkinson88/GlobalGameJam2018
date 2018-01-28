using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void OnLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnLevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }

    public void OnLevelThree()
    {
        SceneManager.LoadScene("Level3");
    }

    public void OnLevelFour()
    {
        SceneManager.LoadScene("Level4");
    }

    public void OnLevelFive()
    {
        SceneManager.LoadScene("Level5");
    }
}
