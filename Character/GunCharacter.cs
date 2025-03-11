using System.Collections.Generic;
using UnityEngine;

public class GunCharacter : Character
{
    [SerializeField] private Transform firePos;

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
        Bullet bullet = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.GunnerBullet, firePos.position, Quaternion.identity)
            .GetComponent<Bullet>();
        bullet.SetTarget(targetMonster.transform);
        bullet.SetPower(data.Power);
    }

    public override void ChangeTraceState()
    {
        base.ChangeTraceState();
        animator.SetTrigger("reload");
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Index = 3;
        data.Power = 2;
        data.AttackSpeed = 1.2f;
        data.AttackRange = 6;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
