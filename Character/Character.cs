using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int Index { get; set; }

    // ���ݷ�
    public int Power { get; set; } = 0;

    // ���� �ӵ�
    public float AttackSpeed { get; set; } = 1f;

    // ���� �Ÿ�
    public float AttackRange { get; set; } = 1f;

    // ����
    public int MaxLevel { get; set; } = 3;
    private int level = 1;
    public int Level 
    {
        get { return level; }
        set 
        { 
            level = value;

            if (level > MaxLevel)
                level = MaxLevel;
        }
    }
}

public enum CharacterState
{
    Idle, 
    Trace,
    Attack,
}

public abstract class Character : MonoBehaviour
{
    public CharacterData data = new CharacterData();
    private float attackTimer = 0;

    protected private Animator animator;
    private Monster targetMonster = null;

    [SerializeField] private CharacterState charState = CharacterState.Idle;

    [System.Serializable]
    public class UpgradeObj
    {
        public List<GameObject> objList;
    }

    [SerializeField] protected List<UpgradeObj> upgradeObjList;
    [SerializeField] protected List<GolemCharacter> golemPrefabList;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Game)
            return;
                

        /*
         Collider[] colls = Physics.OverlapSphere(transform.position, data.AttackRange);
        attackTimer += Time.deltaTime;

        float attackDistance = data.AttackRange;
        Collider collider = null;

        foreach (var monsterCollider in colls)
        {
            string monsterTag = GameManager.Instance.GetStringTagByEnum(Tag.Monster);
            if (monsterCollider.tag.Equals(monsterTag))
            {
                //���� ó��
                if (attackTimer >= data.AttackSpeed)
                {
                    float distance = Vector3.Distance(transform.position, monsterCollider.transform.position);

                    if (distance <= attackDistance)
                    {
                        attackDistance = distance;
                        collider = monsterCollider;
                    }

                    if (collider != null) 
                    {
                        monsterList.Clear();
                        monsterList.Add(collider.GetComponent<Monster>());

                        attackTimer = 0;
                        AttackAnimation();
                    }
                }
            }                
        }
         */ //LEGACY

        Collider[] colls = Physics.OverlapSphere(transform.position, data.AttackRange);        
        bool isInTargetMonster = false;

        for (int i = 0; i < colls.Length; i++)
        {
            string monsterTag = GameManager.Instance.GetStringTagByEnum(Tag.Monster);
            if (colls[i].tag.Equals(monsterTag))
            {
                targetMonster = colls[i].GetComponent<Monster>();
                isInTargetMonster = true; 

                break;
            }                
        }

        if (!isInTargetMonster)
            targetMonster = null;

        if (charState != CharacterState.Attack)
        {
            attackTimer += Time.deltaTime;

            if (targetMonster == null)
                ChangeIdleState();

            //���� ó��
            else if (attackTimer >= data.AttackSpeed)
            {
                attackTimer = 0;
                ChangeAttackState();
            }

            //�ش� ���� �ٶ󺸱�
            else
                ChangeTraceState();
        }
    }
    
    public void ChangeIdleState()
    {
        charState = CharacterState.Idle;
        IdleAnimation();
    }

    public virtual void ChangeTraceState()
    {
        try
        {
            charState = CharacterState.Trace;
            LookAtTarget();
        }

        //monsterList[0] �� missing �� ���
        catch (MissingReferenceException e)
        {
            targetMonster = null;
            ChangeIdleState();
            Debug.Log(e);
        }
    }
    public void ChangeAttackState()
    {
        LookAtTarget();

        charState = CharacterState.Attack;
        animator.SetTrigger("attack");
    }

    void LookAtTarget()
    {
        if (targetMonster == null)
            return;

        Transform targetTrans = targetMonster.transform;
        transform.LookAt(targetTrans, Vector3.up);
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.AttackRange);
    }

    #region Animation 
    public void IdleAnimation()
    {
        animator.SetTrigger("idle");
    }

    public void AttackAnimation()
    {
        animator.SetTrigger("attack");
    }

    public void ReloadAnimation()
    {
        animator.SetTrigger("reload");
    }
    #endregion

    /// <summary>
    /// �ִϸ��̼��� �̺�Ʈ�� �Լ��� ȣ��� 
    /// </summary>
    public void Attack()
    {
        LookAtTarget();

        if (targetMonster != null)
            FireProjectile(targetMonster);

        //targetMonster = null;
    }

    /// <summary>
    /// ���Ÿ� ĳ���� (������, �Ѽ���) : Projectile �߻� -> Projectile �� Monster �� �¾��� �� Damage ó�� 
    /// �ٰŸ� ĳ���� (�˻�, ��)     : ��� Damage ó�� 
    /// </summary>
    protected abstract void FireProjectile(Monster targetMonster);
    
    /// <summary>
    /// ĳ���� ���׷��̵� 
    /// </summary>
    public virtual void Upgrade() 
    {
        data.Level++;

        // FX
        Vector3 spawnPos = transform.position;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CharUpgradeFx, spawnPos, transform.rotation, true, 1f);

        // ���׷��̵� ��ġ ����
        // ex ���׷��̵� ������ 2�� �� sheetData �� 0 �� �ش��ϴ� �����͸� ����
        UpgradeData upgradeData = SpawnManager.Instance.charPrefabDataList[data.Index].sheetData.upgradeDatas[data.Level - 2];
        data.Power += upgradeData.powerUpgradeValue;
        data.AttackSpeed += upgradeData.attackSpeedUpgradeValue;
        data.AttackRange += upgradeData.attackRangeUpgradeValue;
    }
}
