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

public abstract class Character : MonoBehaviour
{
    public CharacterData data = new CharacterData();
    private float attackTimer = 0;

    protected private Animator animator;
    private Monster targetMonster = null;
    //private List<Monster> monsterList = new List<Monster>();

    [HideInInspector] public int[] upgradePriceList = { 50, 60, 100 };

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
        attackTimer += Time.deltaTime;

        //Collider collider = null;
        bool isInTargetMonster = false;

        for (int i = 0; i < colls.Length; i++)
        {
            string monsterTag = GameManager.Instance.GetStringTagByEnum(Tag.Monster);
            if (colls[i].tag.Equals(monsterTag))
            {
                targetMonster = colls[i].GetComponent<Monster>();
                isInTargetMonster = true; 

                //���� ó��
                if (attackTimer >= data.AttackSpeed)
                {
                    attackTimer = 0;
                    AttackAnimation();
                }

                break;
            }                
        }

        if (!isInTargetMonster)
            targetMonster = null;

        if (targetMonster == null)
        {
            IdleAnimation();
            return;
        }
        //�ش� ���� �ٶ󺸱�
        else
        {
            try
            {
                Transform targetTrans = targetMonster.transform;
                transform.LookAt(targetTrans, Vector3.up);
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e);
                //monsterList[0] �� missing �� ��쿡�� �� �ڵ尡 ������� �ʰ� ó�� 
            }
        }
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
        if (targetMonster != null)
            FireProjectile(targetMonster);

        targetMonster = null;
    }

    /// <summary>
    /// ���Ÿ� ĳ���� (������, �Ѽ���) : Projectile �߻� -> Projectile �� Monster �� �¾��� �� Damage ó�� 
    /// �ٰŸ� ĳ���� (�˻�, ��)     : ��� Damage ó�� 
    /// </summary>
    protected abstract void FireProjectile(Monster targetMonster);

    //TODO:MAX ġ ����ó��
    public virtual void Upgrade() 
    {
        data.Level++;

        //FX
        Vector3 spawnPos = transform.position;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CharUpgradeFx, spawnPos, transform.rotation, true, 1f);

        //���׷��̵� ��ġ
        data.Power += (2 + data.Level);
        data.AttackSpeed += (0.2f * data.Level);
        data.AttackRange += (0.2f * data.Level);
    }
}
