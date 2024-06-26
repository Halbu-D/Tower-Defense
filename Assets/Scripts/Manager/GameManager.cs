using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int Money;
    public int startMoney = 500;
    public static int Lives;
    public int startLives = 30;

    public Text livesUI;
    public Text moneyUI;

    public static bool GameIsOver;

    public GameObject gameOverUI;
    public static int Rounds;

    private void Start()
    {
        GameIsOver = false;
        Money = startMoney;
        Lives = startLives;

        Rounds = 0;
    }

    private void Update()
    {
        livesUI.text = Lives.ToString();
        moneyUI.text = Money.ToString();

        if (GameIsOver)
            return;

        if(Input.GetKeyDown("i"))
        {
            EndGame();
        }

        if(Lives <= 0)
        {
            EndGame();
        }
    }


    void EndGame()
    {
        GameIsOver = true;

        gameOverUI.SetActive(true);
    }
}
