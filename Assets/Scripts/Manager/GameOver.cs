using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text roundsText;

    private void OnEnable()
    {
        roundsText.text = GameManager.Rounds.ToString();
    }
    
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // buildmanager 인덱스에 있는 씬을 참조
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
