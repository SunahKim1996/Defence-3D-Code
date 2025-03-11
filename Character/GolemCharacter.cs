using UnityEngine;

public class GolemCharacter : Character
{
    bool isDataUpdated = false;

    public override void Upgrade()
    {
        base.Upgrade();

        // ¥Ÿ¿Ω ∑π∫ß¿« ∞Ò∑Ω º“»Ø
        int levelInt = data.Level - 1;
        GolemCharacter newGolem = Instantiate(golemPrefabList[levelInt], transform.position, transform.rotation);

        // ªı∑ŒøÓ ∞Ò∑Ωø° «ˆ¿Á µ•¿Ã≈Õ∑Œ µ§æÓæ∫øÏ±‚ 
        SpawnManager.Instance.RefreshCharacterSpawnData(gameObject, newGolem.gameObject);
        newGolem.RefreshData(this);

        // ±‚¡∏ ∞Ò∑Ω ªË¡¶
        Destroy(gameObject);
    }

    protected override void FireProjectile(Monster targetMonster)
    {
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.GolemCrack, targetMonster.transform.position, Quaternion.identity, true, 1f);
        targetMonster.Damage(data.Power);
    }

    public override void ChangeTraceState()
    {
        base.ChangeTraceState();
        animator.SetTrigger("idle");
    }

    void RefreshData(GolemCharacter originGolem)
    {
        isDataUpdated = true;

        data.Index = originGolem.data.Index;
        data.Power = originGolem.data.Power;
        data.AttackSpeed = originGolem.data.AttackSpeed;
        data.AttackRange = originGolem.data.AttackRange;
        data.Level = originGolem.data.Level;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isDataUpdated)
        {
            data.Index = 2;
            data.Power = 20;
            data.AttackSpeed = 0.5f;
            data.AttackRange = 2;
            data.Level = 1;
        }        

        animator = GetComponent<Animator>();
    }
}
