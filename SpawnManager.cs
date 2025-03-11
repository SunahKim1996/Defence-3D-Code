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
    [SerializeField] private List<MonsterData> monsterDataList;

    public List<GameObject> monsterList;
    public MonsterHP monsterHPUI;
    public Transform monsterHPParent;
    public List<GameObject> flags;

    public float delay;
    private float delayTimer;

    private int spawnCount;
    private int spawnMaxCount;
    private float waveDelayTime = 2f;
    private float waveDelayTimer;
    [HideInInspector] public bool isBossTime = false;

    [Header("Character")]
    [SerializeField] private Transform charSpawnFxParent;

    // ĳ���� �⺻ ���� ( ���׷��̵� ���, ���� ���, ���׷��̵� �� ���ݷ� ��� ���� ��)
    public List<CharacterPrefabData> charPrefabDataList;
        
    // ������ ĳ���� ���� ( ���� ���׷��̵� ����, ���� ���ݷ� ��)  
    [HideInInspector] public List<CharacterSpawnData> characterSpawnList = new List<CharacterSpawnData>();


    [Header("Bullet FX")]
    [SerializeField] private Bullet wizardBulletPrefab;
    [SerializeField] private Bullet gunnerBulletPrefab;
    [SerializeField] private GameObject golemCrackPrefab;
    [SerializeField] private GameObject swordEffectPrefab;

    [SerializeField] private Transform bulletParent;
    [SerializeField] private Transform swordEffectParent;

    private void Start()
    {
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

    // ���� ���� ------------------------------------------------------------
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
            // ���� Wave ���, �Ϲ� �� ��ȯ ���� �� ���� �� ��ȯ ó�� 
            Wave waveData = GetCurWaveData();
            if (waveData.IsBossWave)
            {
                isBossTime = true;
                SpawnEnemy(waveData.BossPrefab);
                UI.Instance.ShowBossUI();
            }
            else
            {
                waveDelayTimer += Time.deltaTime;

                if (waveDelayTimer >= waveDelayTime)
                {
                    waveDelayTimer = 0;
                    GameManager.Instance.ChangeNextWave();
                }                
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

    // ĳ���� ���� -----------------------------------------------------------
    public void SpawnCharacter(int index, Transform parent, int spawnPrice)
    {
        // �ݾ� ���� 
        bool isSuccessPay = PlayerData.Instance.PayGold(spawnPrice);
        if (!isSuccessPay) 
        {
            UI.Instance.ShowNoticeUI("�������� �����մϴ�", 1f);
            return;
        }

        // ���� 
        GameObject spawnObj = charPrefabDataList[index].prefab.gameObject;
        Transform trans = Instantiate(spawnObj, parent.position + new Vector3(0f, 0.2f, 0f), parent.rotation).transform;
        RefreshPadMat(parent, 1);

        // ������ �߰� 
        CharacterSpawnData data = new CharacterSpawnData();
        data.character = trans.gameObject;
        data.pad = parent;

        characterSpawnList.Add(data);

        //FX
        Vector3 spawnPos = parent.position;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CharSpawnFx, spawnPos, parent.rotation, true, 1f);
    }
    
    /// <summary>
    /// �� ĳ���ʹ� ���׷��̵� �� Gameobject �� ��ü�Ǿ�� �ؼ� Data �� ������ ���� ó��
    /// </summary>
    public void RefreshCharacterSpawnData(GameObject originGameObject, GameObject newGameObject)
    {
        for (int i = 0; i < characterSpawnList.Count; i++)
        {
            if (characterSpawnList[i].character == originGameObject)
            {
                characterSpawnList[i].character = newGameObject;
                break;
            }
        }
    }

    public void UpgradeCharacter(Transform pad)
    {
        CharacterSpawnData data = GetTargetCharData(pad);

        if (data == null)
        {
            Debug.LogError($"{pad}�� �ش��ϴ� ĳ���� ����");
            return;
        }

        Character targetChar = data.character.GetComponent<Character>();

        // �ݾ� ���� 
        int upgradePrice = GetUpgradePrice(pad);
        bool isSuccessPay = PlayerData.Instance.PayGold(upgradePrice);
        if (!isSuccessPay)
        {
            UI.Instance.ShowNoticeUI("�������� �����մϴ�", 1f);
            return;
        }

        targetChar.Upgrade();
        RefreshPadMat(pad, targetChar.data.Level);
    }

    /// <summary>
    /// �� ĳ���� Level ���� �е� ���� ���� 
    /// </summary>
    void RefreshPadMat(Transform pad, int level)
    {
        pad.gameObject.GetComponent<MeshRenderer>().material 
            = ResourceManager.Instance.rankMatList[level-1];
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
    /// ���� �������� �ʿ��� ���׷��̵� ��� return 
    /// </summary>
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
