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
                UI.Instance.ShowTextEffect(PoolKey.GoldTextEffect, addGold, ResourceManager.Instance.coinIcon, 30f); 

            gold = value;
            UI.Instance.RefreshGoldUI(gold);
            UI.Instance.RefreshPriceColor_SelectUI();
            UI.Instance.RefreshPriceColor_UpgradeUI();
            UI.Instance.RefreshRepairPriceText();
        }
    }
    
    

    // -----------------------------------------------------------------------
    [HideInInspector] public PlayerData playerData;

    public void Initialize()
    {
        Gold = 100;        
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
