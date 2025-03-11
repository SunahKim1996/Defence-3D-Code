using System.Collections.Generic;
using UnityEngine;

public class WizardCharacter : Character
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

        // ���� ������ ���׷��̵� ������Ʈ�� �Ⱥ��̰� ó�� 
        int preLevelIndex = data.Level - 2;
        if (preLevelIndex >= 0)
        {
            List<GameObject> disableObjList = upgradeObjList[preLevelIndex].objList;

            for (int i = 0; i < disableObjList.Count; i++)
                disableObjList[i].SetActive(false);
        }
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
