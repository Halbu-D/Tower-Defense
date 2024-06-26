using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    public Grid grid;

    private GameObject tower;
    private Turret turret;
    //private HexTile tile;
    //private TileManager tileManager;

    public Text upgradeText1;
    public Text upgradeText2;

    private Vector3Int currentPos;

    public void SetTarget(Vector3Int pos) //좌표를 받음.
    {
        //tile = TileManager.instance.hexTileDict[pos];
        currentPos = pos;
        transform.position = grid.CellToWorld(pos);
        tower = TileManager.instance.hexTileDict[pos].turret; //해당 위치의 터렛 가져옴.
        turret = tower.GetComponent<Turret>();

        SetText();

        ui.SetActive(true);
    }

    public void Hide()
    {
        ui.SetActive(false);
    }

    public void RangeUpgrade()
    {
        if(GameManager.Money > turret.upgradeCost && turret.rangeUpgraded < 4)
        {
            turret.UpgradeRange();

            GameManager.Money -= turret.upgradeCost;

            turret.upgradeCost *= 2;
        }

        SetText();
    }

    public void SpeedUpgrade()
    {
        if(GameManager.Money > turret.upgradeCost2 && turret.secondUpgraded < 4)
        {
            if(turret.useLaser || turret.mageTower) // 레이저 타워혹은 번개타워
                turret.UpgradeDamage(); // 공격력 증가
            else
                turret.UpgradeSpeed(); //발사속도 증가

            GameManager.Money -= turret.upgradeCost2;

            turret.upgradeCost2 *= 2;
        }

        SetText();
    }


    private void SetText()
    {
        string text1;
        string text2;

        int currentRange = turret.rangeUpgraded - 1;
        int currentSecond = turret.secondUpgraded - 1;

        if (currentRange >= 3)
            text1 = "최대 사거리\n" + currentRange.ToString() + " 단계";
        else
            text1 = turret.upgradeCost + "$\n" + "사거리 증가\n" + currentRange.ToString() + " 단계";

        if (currentSecond >= 3)
        {
            if (turret.useLaser || turret.mageTower)
                text2 = "최대 공격력\n" + currentSecond.ToString() + " 단계";
            else
                text2 = "최대 발사속도\n" + currentSecond.ToString() + " 단계";
        }
        else
        {
            if (turret.useLaser || turret.mageTower)
                text2 = turret.upgradeCost2 + "$\n" + "공격력 증가\n" + currentSecond.ToString() + " 단계";
            else
                text2 = turret.upgradeCost2 + "$\n" + "발사속도 증가\n" + currentSecond.ToString() + " 단계";
        }

        upgradeText1.text = text1;
        upgradeText2.text = text2;
    }

    public void RemoveTurret()
    {
        TileManager.instance.hexTileDict[currentPos].turret = null;
        TileManager.instance.hexTileDict[currentPos].isWalkable = true;

        MonsterPath.Instance.change = true; // 경로 재탐색 요청
        this.Hide();

        Destroy(turret.gameObject);
    }
}
