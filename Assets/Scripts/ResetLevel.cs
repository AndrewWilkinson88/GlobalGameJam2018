using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{
    public string currentLevel;

    public void OnResetLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }
}
