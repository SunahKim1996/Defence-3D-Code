using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WizardCharacter : Character
{
    [SerializeField] private Transform firePos;

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
        Bullet bullet = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.WizardBullet, firePos.position, Quaternion.identity)
            .GetComponent<Bullet>();
        bullet.SetTarget(targetMonster.transform);
        bullet.SetPower(data.Power);
    }

    public override void ChangeTraceState()
    {
        base.ChangeTraceState();
        animator.SetTrigger("idle");
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Index = 1;
        data.Power = 8;
        data.AttackSpeed = 1.5f;
        data.AttackRange = 6;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
