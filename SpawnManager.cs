using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public class CharacterSpawnData
    {
        public GameObject character;
        public Transform pad;
        public int upgrade = 0;
    }

    public List<GameObject> monsters;
    public MonsterHP monsterHPUI;
    public Transform monsterHPParent;
    public List<Flag> flags;

    public float delay;
    private float delayTimer;

    private int spawnCount;
    private int spawnMaxCount;

    List<CharacterSpawnData> characterSpawnList = new List<CharacterSpawnData>();
    public List<Wave> waveData = new List<Wave>();
    public int waveCount = 0;
    public float waveDelayTimer = 0;

    // Start is called before the first frame update

    public void Initialize()
    {
        waveCount = UI.Instance.Wave = 1;
        spawnCount = 0;
        spawnMaxCount = waveData[waveCount - 1].EnemyList.Count;
        monsters = waveData[waveCount - 1].EnemyList;
    }

    // Update is called once per frame
    void Update()
    {
        if (waveDelayTimer > 0)
        {
            waveDelayTimer -=Time.deltaTime;
            return;
        }

        delayTimer += Time.deltaTime;

        if (delayTimer >= delay && spawnCount <= spawnMaxCount)
        {
            delayTimer = 0;

            Monster m = Instantiate(monsters[spawnCount], flags[0].transform.position, Quaternion.identity).GetComponent<Monster>();

            MonsterHP hp = Instantiate(monsterHPUI, monsterHPParent);
            hp.SetTarget(m.transform);
            m.HpUI = hp;

            m.SetFlag(flags);
            m.SetDirecion(flags[0].dir);

            spawnCount++;
        }
        else if (spawnCount >= spawnMaxCount)
        {
            UI.Instance.Wave++;

            if (UI.Instance.Wave > waveData.Count)
                waveCount = 1;

            waveDelayTimer = 5;
            spawnCount = 0;
            spawnMaxCount = waveData[waveCount - 1].EnemyList.Count;
            monsters = waveData[waveCount - 1].EnemyList;
        }
    }

    public void SpawnCharacter(GameObject spawnObj, Transform parent)
    {
        Transform trans = Instantiate(spawnObj, parent).transform;
        trans.localPosition = Vector3.zero;
        trans.localPosition = new Vector3(0f, 0.5f, 0f);

        CharacterSpawnData data = new CharacterSpawnData();
        data.character = trans.gameObject;
        data.pad = parent;

        characterSpawnList.Add(data);
    }

    //TODO:MAX 치 예외처리
    public void UpgradeCharacter(Transform pad)
    {
        foreach (var data in characterSpawnList)
        {
            if (data.pad.Equals(pad))
            {
                data.upgrade++;
                data.character.GetComponent<Character>().Upgrade();

                break;
            }
        }
    }
}
