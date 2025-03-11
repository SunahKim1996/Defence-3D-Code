using System.Collections.Generic;
using UnityEngine;

public class WarrirorCharacter : Character
{
    public override void Upgrade()
    {
        base.Upgrade();

        // 업그레이드 레벨에 맞는 오브젝트 보이게 처리
        // ex 업그레이드 레벨이 2일 때 upgradeObjList 의 1 에 해당하는 데이터를 적용 
        List<GameObject> enableObjList = upgradeObjList[data.Level - 1].objList;

        for (int i = 0; i < enableObjList.Count; i++)
            enableObjList[i].SetActive(true);
    }

    protected override void FireProjectile(Monster targetMonster)
    {
        SwordEffect effect = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.SwordEffect, Vector3.one, Quaternion.identity, true, 0.5f)
            .GetComponent<SwordEffect>();
        effect.SetTarget(targetMonster.transform);
        effect.SetHeightOffset(true, new Vector3(0, 0.5f, 0));

        targetMonster.Damage(data.Power);
    }

    public override void ChangeTraceState()
    {
        base.ChangeTraceState();
        animator.SetTrigger("idle");
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Index = 0;
        data.Power = 10;
        data.AttackSpeed = 1;
        data.AttackRange = 3;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
