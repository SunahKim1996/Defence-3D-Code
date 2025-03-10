using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemCharacter : Character
{
    public override void Upgrade()
    {
        base.Upgrade();

        UpgradeData upgradeData = SpawnManager.Instance.charPrefabDataList[data.Index].sheetData.upgradeDatas[data.Level - 2];
        data.Power += upgradeData.powerUpgradeValue;
        data.AttackSpeed += upgradeData.attackSpeedUpgradeValue;
        data.AttackRange += upgradeData.attackRangeUpgradeValue;
    }

    protected override void FireProjectile(Monster targetMonster)
    {
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.GolemCrack, targetMonster.transform.position, Quaternion.identity, true, 1f);
        targetMonster.Damage(data.Power);
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Index = 2;
        data.Power = 20;
        data.AttackSpeed = 2;
        data.AttackRange = 4;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
