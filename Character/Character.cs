using UnityEngine;

public class CharacterData
{
    public int Index { get; set; }

    // 공격력
    public int Power { get; set; } = 0;

    // 공격 속도
    public float AttackSpeed { get; set; } = 1f;

    // 공격 거리
    public float AttackRange { get; set; } = 1f;

    // 레벨
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
                //공격 처리
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

                //공격 처리
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
        //해당 몬스터 바라보기
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
                //monsterList[0] 이 missing 인 경우에는 위 코드가 실행되지 않게 처리 
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
    /// 애니메이션의 이벤트로 함수가 호출됨 
    /// </summary>
    public void Attack()
    {
        if (targetMonster != null)
            FireProjectile(targetMonster);

        targetMonster = null;
    }

    /// <summary>
    /// 원거리 캐릭터 (마법사, 총술사) : Projectile 발사 -> Projectile 이 Monster 에 맞았을 때 Damage 처리 
    /// 근거리 캐릭터 (검사, 골렘)     : 즉시 Damage 처리 
    /// </summary>
    protected abstract void FireProjectile(Monster targetMonster);

    //TODO:MAX 치 예외처리
    public virtual void Upgrade() 
    {
        data.Level++;

        //FX
        Vector3 spawnPos = transform.position;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CharUpgradeFx, spawnPos, transform.rotation, true, 1f);

        //업그레이드 수치
        data.Power += (2 + data.Level);
        data.AttackSpeed += (0.2f * data.Level);
        data.AttackRange += (0.2f * data.Level);
    }
}
