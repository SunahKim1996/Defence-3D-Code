using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private List<GameObject> flags;
    private int pointIdx = 0;

    public MonsterHP HpUI { get; set; }
    private int hp;

    [SerializeField] private bool isBossMonster;
    [SerializeField] private int monsterIndex;
    private MonsterData monsterData;
       

    // Start is called before the first frame update
    void Start()
    {
        hp = monsterData.MaxHp;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Game)
            return;

        // 값이 없을 때 예외처리 
        if (flags == null || flags.Count == 0)
            return;

        //transform.Translate(Vector3.right * Time.deltaTime * speed);
        transform.position = Vector3.MoveTowards(
            transform.position, 
            flags[pointIdx].transform.position, 
            Time.deltaTime * monsterData.Speed);

        float distance = Vector3.Distance(
            transform.position, 
            flags[pointIdx].transform.position);

        if (distance <= 0)
        {
            pointIdx++;

            if (pointIdx >= flags.Count)
            {
                int damageValue = (isBossMonster) ? 3 : 1;
                CastleManager.Instance.Life -= damageValue;

                DestroyMonster();
            }                
            else
                SetDirecion(flags[pointIdx]);
        }
    }

    // targetFlag : 다음 목표 지점 깃발 
    public void SetDirecion(GameObject targetFlag)
    {
        transform.LookAt(targetFlag.transform.position, Vector3.up);
    }

    public void SetFlag(List<GameObject> flags) 
    { 
        this.flags = flags;
    }

    public void SetData(List<MonsterData> monterDataList)
    {
        monsterData = monterDataList[monsterIndex];
    }

    public void Damage(int damage)
    {
        hp -= damage;
        UI.Instance.ShowDamageValueText(damage, transform);

        if (hp <= 0)
        {
            PlayerData.Instance.Gold += monsterData.RewardGold;
            ObjectPoolManager.Instance.ShowObjectPool(
                PoolKey.MonsterDeadFX, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, true, 1f);

            if (isBossMonster)
                GameManager.Instance.ChangeNextWave();

            DestroyMonster();

            return;
        }    
        
        HpUI.SetSize(hp, monsterData.MaxHp);
    }

    void DestroyMonster()
    {
        try
        {
            Destroy(HpUI.gameObject);
            Destroy(gameObject);                
        }
        catch (MissingReferenceException e) 
        {
            Debug.Log(e);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();

        if (bullet != null)
        {
            Damage(bullet.power);

            PoolKey targetPoolKey = (bullet.bulletType == BulletType.Wizard) ? PoolKey.WizardBullet : PoolKey.GunnerBullet;
            bullet.ClearTrailRenderer();
            ObjectPoolManager.Instance.Hide(targetPoolKey, bullet.gameObject);
        }
    }
}
