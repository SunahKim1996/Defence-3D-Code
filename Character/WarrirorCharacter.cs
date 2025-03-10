using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarrirorCharacter : Character
{
    [SerializeField] private List<GameObject> upgradeObj;
    public override void Upgrade()
    {
        base.Upgrade();

        for (int i = 0; i < upgradeObj.Count; i++) 
        {
            upgradeObj[i].SetActive(true);
        }

        UpgradeData upgradeData = SpawnManager.Instance.charPrefabDataList[data.Index].sheetData.upgradeDatas[data.Level - 2];
        data.Power += upgradeData.powerUpgradeValue;
        data.AttackSpeed += upgradeData.attackSpeedUpgradeValue;
        data.AttackRange += upgradeData.attackRangeUpgradeValue;
    }

    protected override void FireProjectile(Monster targetMonster)
    {
        SwordEffect effect = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.SwordEffect, Vector3.one, Quaternion.identity, true, 0.5f)
            .GetComponent<SwordEffect>();
        effect.SetTarget(targetMonster.transform);
        effect.SetHeightOffset(true, new Vector3(0, 0.5f, 0));

        targetMonster.Damage(data.Power);
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Index = 0;
        data.Power = 10;
        data.AttackSpeed = 1;
        data.AttackRange = 5;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
