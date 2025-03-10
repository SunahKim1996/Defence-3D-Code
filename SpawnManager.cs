using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawnData
{
    public GameObject character;
    public Transform pad;
}

[System.Serializable]
public class CharacterPrefabData
{
    public Character prefab;
    public CharacterSheetData sheetData;
}

public class SpawnManager : Singleton<SpawnManager>
{    
    [Header("Monster")]
    public List<GameObject> monsterList;
    public MonsterHP monsterHPUI;
    public Transform monsterHPParent;
    public List<GameObject> flags;

    public float delay;
    private float delayTimer;

    private int spawnCount;
    private int spawnMaxCount;

    bool isBossTime = false;

    [SerializeField] private List<MonsterData> monsterDataList;

    [Header("Character")]
    [SerializeField] private Transform charSpawnFxParent;

    // 캐릭터 기본 정보 ( 업그레이드 비용, 생성 비용, 업그레이드 시 공격력 상승 비율 등)
    public List<CharacterPrefabData> charPrefabDataList;
        
    // 스폰된 캐릭터 정보 ( 현재 업그레이드 레벨, 현재 공격력 등)  
    [HideInInspector] public List<CharacterSpawnData> characterSpawnList = new List<CharacterSpawnData>();

    [Header("Bullet")]
    [SerializeField] private Bullet wizardBulletPrefab;
    [SerializeField] private Bullet gunnerBulletPrefab;
    [SerializeField] private GameObject golemCrackPrefab;
    [SerializeField] private GameObject swordEffectPrefab;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private Transform swordEffectParent;

    void Start()
    {
        GameObject charSpawnFX = ResourceManager.Instance.charSpawnFX;
        ObjectPoolManager.Instance.Init(PoolKey.CharSpawnFx, charSpawnFxParent, charSpawnFX);

        ObjectPoolManager.Instance.Init(PoolKey.WizardBullet, bulletParent, wizardBulletPrefab.gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.GunnerBullet, bulletParent, gunnerBulletPrefab.gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.GolemCrack, bulletParent, golemCrackPrefab);
        ObjectPoolManager.Instance.Init(PoolKey.SwordEffect, swordEffectParent, swordEffectPrefab);
    }

    public void InitCharPrefabData(List<CharacterSheetData> characterSheetData)
    {
        for (int i = 0; i < charPrefabDataList.Count; i++)
            charPrefabDataList[i].sheetData = characterSheetData[i];

        UI.Instance.InitCharSelectUI();
    }

    // -----------------------------------------------------------------------
    public void SetCurWaveData()
    {
        spawnCount = 0;

        Wave waveData = GetCurWaveData();
        spawnMaxCount = waveData.EnemySpawnCount;
        monsterList = waveData.EnemyList;
    }

    Wave GetCurWaveData()
    {
        int waveIndex = GameManager.Instance.Wave - 1;
        Wave waveData = GameManager.Instance.waveData[waveIndex];

        return waveData;
    }

    // 몬스터 스폰 ------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Game)
            return;

        if (isBossTime)
            return;

        delayTimer += Time.deltaTime;

        if (delayTimer >= delay && spawnCount <= spawnMaxCount)
        {
            delayTimer = 0;

            int randomIndex = Random.Range(0, monsterList.Count);
            SpawnEnemy(monsterList[randomIndex]);            
            spawnCount++;
        }
        else if (spawnCount >= spawnMaxCount)
        {
            //TODO: Wave 사이사이 쉬는 시간

            // 보스 Wave 라면, 일반 몹 소환 끝난 후 보스 몹 소환 처리 
            Wave waveData = GetCurWaveData();
            if (waveData.IsBossWave)
            {
                isBossTime = true;
                SpawnEnemy(waveData.BossPrefab);
                UI.Instance.ShowBossUI();
            }
            else
            {
                GameManager.Instance.Wave++;
                SetCurWaveData();
            }
        }
    }

    void SpawnEnemy(GameObject prefab)
    {
        Monster m = Instantiate(prefab, flags[0].transform.position, Quaternion.identity).GetComponent<Monster>();

        MonsterHP hp = Instantiate(monsterHPUI, monsterHPParent);
        hp.SetTarget(m.transform);
        hp.SetHeightOffset(true, new Vector3(0, 1f, 0));
        m.HpUI = hp;

        m.SetFlag(flags);
        m.SetDirecion(flags[1]);
        m.SetData(monsterDataList);
    }

    // 캐릭터 스폰 -----------------------------------------------------------
    public void SpawnCharacter(int index, Transform parent, int spawnPrice)
    {
        // 금액 지불 
        bool isSuccessPay = PlayerData.Instance.PayGold(spawnPrice);
        if (!isSuccessPay) 
        {
            UI.Instance.ShowNoticeUI("소지금이 부족합니다");
            return;
        }

        // 스폰 
        GameObject spawnObj = charPrefabDataList[index].prefab.gameObject;
        Transform trans = Instantiate(spawnObj, parent.position + new Vector3(0f, 0.2f, 0f), parent.rotation).transform;
        RefreshPadMat(parent, 1);

        // 데이터 추가 
        CharacterSpawnData data = new CharacterSpawnData();
        data.character = trans.gameObject;
        data.pad = parent;

        characterSpawnList.Add(data);

        //FX
        Vector3 spawnPos = parent.position;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CharSpawnFx, spawnPos, parent.rotation, true, 1f);
    }
        
    public void UpgradeCharacter(Transform pad)
    {
        CharacterSpawnData data = GetTargetCharData(pad);

        if (data == null)
        {
            Debug.LogError($"{pad}에 해당하는 캐릭터 없음");
            return;
        }

        Character targetChar = data.character.GetComponent<Character>();
        int level = targetChar.data.Level;

        int upgradePrice = GetUpgradePrice(pad);
        int curGold = PlayerData.Instance.Gold;

        // 금액 지불 
        bool isSuccessPay = PlayerData.Instance.PayGold(upgradePrice);
        if (!isSuccessPay)
        {
            UI.Instance.ShowNoticeUI("소지금이 부족합니다");
            return;
        }

        targetChar.Upgrade();
        RefreshPadMat(pad, level);
    }

    /// <summary>
    /// 각 캐릭터 Level 별로 패드 색상 변경 
    /// </summary>
    void RefreshPadMat(Transform pad, int level)
    {
        pad.gameObject.GetComponent<MeshRenderer>().material = ResourceManager.Instance.rankMatList[level-1];
    }

    // Util -----------------------------------------------------------
    public CharacterSpawnData GetTargetCharData(Transform pad)
    {
        foreach (var data in characterSpawnList)
        {
            if (data.pad.Equals(pad))
                return data;
        }

        return null;
    }

    /// <summary>
    /// 현재 레벨에서 필요한 업그레이드 비용 return 
    /// </summary>
    /// <returns></returns>
    public int GetUpgradePrice(Transform pad)
    {
        CharacterSpawnData data = GetTargetCharData(pad.transform);

        Character targetChar = data.character.GetComponent<Character>();
        int level = targetChar.data.Level;
        int index = targetChar.data.Index;

        UpgradeData upgradeData = charPrefabDataList[index].sheetData.upgradeDatas[level - 1];
        int upgradePrice = upgradeData.price;

        return upgradePrice;
    }
}
