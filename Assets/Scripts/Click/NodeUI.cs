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

    public void SetTarget(Vector3Int pos) //��ǥ�� ����.
    {
        //tile = TileManager.instance.hexTileDict[pos];
        currentPos = pos;
        transform.position = grid.CellToWorld(pos);
        tower = TileManager.instance.hexTileDict[pos].turret; //�ش� ��ġ�� �ͷ� ������.
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
            if(turret.useLaser || turret.mageTower) // ������ Ÿ��Ȥ�� ����Ÿ��
                turret.UpgradeDamage(); // ���ݷ� ����
            else
                turret.UpgradeSpeed(); //�߻�ӵ� ����

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
            text1 = "�ִ� ��Ÿ�\n" + currentRange.ToString() + " �ܰ�";
        else
            text1 = turret.upgradeCost + "$\n" + "��Ÿ� ����\n" + currentRange.ToString() + " �ܰ�";

        if (currentSecond >= 3)
        {
            if (turret.useLaser || turret.mageTower)
                text2 = "�ִ� ���ݷ�\n" + currentSecond.ToString() + " �ܰ�";
            else
                text2 = "�ִ� �߻�ӵ�\n" + currentSecond.ToString() + " �ܰ�";
        }
        else
        {
            if (turret.useLaser || turret.mageTower)
                text2 = turret.upgradeCost2 + "$\n" + "���ݷ� ����\n" + currentSecond.ToString() + " �ܰ�";
            else
                text2 = turret.upgradeCost2 + "$\n" + "�߻�ӵ� ����\n" + currentSecond.ToString() + " �ܰ�";
        }

        upgradeText1.text = text1;
        upgradeText2.text = text2;
    }

    public void RemoveTurret()
    {
        TileManager.instance.hexTileDict[currentPos].turret = null;
        TileManager.instance.hexTileDict[currentPos].isWalkable = true;

        MonsterPath.Instance.change = true; // ��� ��Ž�� ��û
        this.Hide();

        Destroy(turret.gameObject);
    }
}
