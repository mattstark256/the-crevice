using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName;

    public void StartTheGame() // Called that cause it can't be StartGame and I cba finding a better name
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
