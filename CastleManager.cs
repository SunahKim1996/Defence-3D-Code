using System.Collections.Generic;
using UnityEngine;

public class CastleManager : Singleton<CastleManager>
{
    bool isInit = true;

    public int MaxLife { get; set; } = 10;

    private int life;
    public int Life
    {
        get { return life; }
        set
        {
            int addLife = value - life;

            if (isInit)
                // 초기 Gold 세팅에서는 Effect 를 보이지 않기 위한 처리 
                isInit = false;
            else
            {
                float heightDis = (addLife < 0) ? -90f : 30f;

                if (addLife != 0)
                    UI.Instance.ShowTextEffect(PoolKey.LifeTextEffect, addLife, ResourceManager.Instance.lifeIcon, heightDis);
            }
                

            life = value;

            if (life <= 0)
            {
                life = 0;
                GameManager.Instance.EndGame();
                RefreshCastle(3, true);
                UI.Instance.ToggleRepairUI(false);
            }
            else if (life == 4)
                RefreshCastle(2);
            else if (life == 7)
                RefreshCastle(1);

            bool state = (life < MaxLife) ? true : false;
            UI.Instance.ToggleRepairUI(state);

            UI.Instance.RefreshLifeUI(life, MaxLife);
        }
    }

    [HideInInspector] public int repairPrice = 30;

    void RefreshCastle(int castleLevel, bool isAllFalse = false)
    {
        List<GameObject> castleList = ResourceManager.Instance.castleList;

        for (int i = 0; i < castleList.Count; i++)
        {
            bool state = isAllFalse ? false : (i == castleLevel) ? true : false;
            castleList[i].SetActive(state);
        }

        if (castleLevel <= 0)
            return;

        Vector3 pos = castleList[0].transform.position + new Vector3(0, 1.8f, 0);
        Quaternion rot = castleList[0].transform.rotation;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CastleDestroyFX, pos, rot, true, 1.5f);
    }

    public void RepairCastle()
    {
        bool isSuccessPay = PlayerData.Instance.PayGold(repairPrice);
        if (!isSuccessPay)
        {
            UI.Instance.ShowNoticeUI("소지금이 부족합니다", 1f);
            return;
        }

        Life++;
    }

    public void Initialize()
    {
        Life = MaxLife;
        RefreshCastle(0);
    }
}
