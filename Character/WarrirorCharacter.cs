using System.Collections.Generic;
using UnityEngine;

public class WarrirorCharacter : Character
{
    public override void Upgrade()
    {
        base.Upgrade();

        // ���׷��̵� ������ �´� ������Ʈ ���̰� ó��
        // ex ���׷��̵� ������ 2�� �� upgradeObjList �� 1 �� �ش��ϴ� �����͸� ���� 
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
