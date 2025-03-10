using UnityEngine;

public class PlayerData : Singleton<PlayerData>
{
    bool isInit = true;

    private int gold = 0;
    public int Gold
    {
        get { return gold; }
        set
        {
            int addGold = value - gold;

            if (isInit)
                // �ʱ� Gold ���ÿ����� Effect �� ������ �ʱ� ���� ó�� 
                isInit = false;
            else if (addGold > 0)
                UI.Instance.ShowGoldValueText(addGold); 

            gold = value;
            UI.Instance.RefreshGoldUI(gold);
            UI.Instance.RefreshPriceColor_SelectUI();
            UI.Instance.RefreshPriceColor_UpgradeUI();
        }
    }
    
    public int MaxLife { get; set; } = 10;

    private int life;
    public int Life
    {
        get { return life; }
        set
        {
            life = value;

            if (life <= 0)
            {
                life = 0;
                GameManager.Instance.EndGame();
            }
            else if (life == 4)
                GameManager.Instance.RefreshCastle(2);
            else if (life == 7)
                GameManager.Instance.RefreshCastle(1);
            else
                //GameManager.Instance.CameraShake(1, 0.5f); //TODO

            UI.Instance.RefreshLifeUI(life, MaxLife);
        }
    }

    // -----------------------------------------------------------------------
    [HideInInspector] public PlayerData playerData;

    public void Initialize()
    {
        Gold = 100;
        Life = MaxLife;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Gold �Һ� (�Һ� ���� ���θ� ��ȯ)
    /// </summary>
    public bool PayGold(int price)
    {
        if (Gold < price)
            return false;

        Gold -= price;

        return true;
    }
}
